using Coimbra;
using Coimbra.Services;
using Core.Util;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Level
{

    public class ChemicalVisualizer : Actor
    {
        [Header("References")]
        [SerializeField] private RawImage _displayImage;

        [Header("Visualization Settings")]
        [Tooltip("The chemical type to display in the overlay.")]
        [SerializeField] private Chemical _chemicalToView;

        [Tooltip("A color gradient to map chemical strength. Black/transparent is zero, a solid color is max.")]
        [SerializeField] private Gradient _colorGradient;
        [SerializeField] private Vector2 _gridWorldOrigin;

        private Texture2D _texture;
        private IChemicalGridService _chemicalService;

        private NativeArray<Color32> _pixelData;
        private NativeArray<Color32> _gradientColors;
        private const int GradientResolution = 256;

        private RectTransform _displayRectTransform;
        private Camera _mainCamera;

        protected override void OnSpawn()
        {
            _chemicalService = ServiceLocatorUtilities.GetServiceAssert<IChemicalGridService>();
            _chemicalService.Initialized += InitializeVisualizer;
        }

        private void InitializeVisualizer()
        {
            _mainCamera = Camera.main;
            _displayRectTransform = _displayImage.GetComponent<RectTransform>();

            var dimensions = _chemicalService.Dimensions;

            _texture = new Texture2D(dimensions, dimensions, TextureFormat.RGBA32, false);
            _texture.filterMode = FilterMode.Point;
            _pixelData = new NativeArray<Color32>(dimensions * dimensions, Allocator.Persistent);

            _displayImage.texture = _texture;
            _displayImage.color = Color.white;

            _gradientColors = new NativeArray<Color32>(GradientResolution, Allocator.Persistent);
            for (int i = 0; i < GradientResolution; i++)
            {
                _gradientColors[i] = _colorGradient.Evaluate((float)i / (GradientResolution - 1));
            }

            _displayRectTransform.anchorMin = new Vector2(0, 0);
            _displayRectTransform.anchorMax = new Vector2(0, 0);
            _displayRectTransform.pivot = new Vector2(0, 0);
        }

        private void LateUpdate()
        {
            if (_texture == null)
                return;

            AlignVisualizerToGrid();

            var mapToView = _chemicalService.GetMap(_chemicalToView);
            if (mapToView == null)
                return;

            var job = new ColorizationJob
            {
                ChemicalValues = mapToView.Grid,
                PixelColors = _pixelData,
                ColorLookup = _gradientColors
            };

            JobHandle handle = job.Schedule(mapToView.Grid.Length, 64);
            handle.Complete();

            _texture.LoadRawTextureData(_pixelData);
            _texture.Apply(false);
        }

        private void AlignVisualizerToGrid()
        {
            Vector2 gridWorldOrigin = _gridWorldOrigin; // You'll need to expose this
            int dimensions = _chemicalService.Dimensions;

            Vector2 bottomLeftWorld = gridWorldOrigin;
            Vector2 topRightWorld = gridWorldOrigin + new Vector2(dimensions, dimensions);

            Vector2 bottomLeftScreen = _mainCamera.WorldToScreenPoint(bottomLeftWorld);
            Vector2 topRightScreen = _mainCamera.WorldToScreenPoint(topRightWorld);

            Vector2 screenSize = topRightScreen - bottomLeftScreen;

            _displayRectTransform.position = bottomLeftScreen;
            _displayRectTransform.sizeDelta = screenSize;
        }

        protected override void OnDestroyed()
        {
            if (_pixelData.IsCreated)
                _pixelData.Dispose();
            if (_gradientColors.IsCreated)
                _gradientColors.Dispose();
        }
    }


    /// <summary>
    /// A Burst-compiled job to convert chemical float values into Color32 pixels in parallel.
    /// </summary>
    [BurstCompile]
    public struct ColorizationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> ChemicalValues;
        [ReadOnly] public NativeArray<Color32> ColorLookup;
        [WriteOnly] public NativeArray<Color32> PixelColors;

        public void Execute(int index)
        {
            float value = ChemicalValues[index];
            int colorIndex = (int)Mathf.Clamp(value, 0, ColorLookup.Length - 1);

            PixelColors[index] = ColorLookup[colorIndex];
        }
    }
}