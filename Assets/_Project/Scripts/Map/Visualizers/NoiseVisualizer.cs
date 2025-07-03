using Core.ProcGen;
using NaughtyAttributes;
using System.Linq;
using TNRD;
using UnityEngine;

public class NoiseVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SerializableInterface<IMapCreator> _mapCreator;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private int _dimensions;
    [SerializeField] private int _seed;

    private System.Random _random;

    private Texture2D _texture;


    [Button("Apply and Show")]
    private void ApplyNoisePassesToTexture()
    {
        _random = new System.Random(_seed);

        float[,] values = _mapCreator.Value.CreateMap(_dimensions, _random);

        Color[,] colors = new Color[_dimensions, _dimensions];

        for (int i = 0; i < _dimensions; i++)
        {
            for (int j = 0; j < _dimensions; j++)
            {
                float value = Helper.NoiseTo01Bound(values[i, j]);
                colors[i, j] = ColorExtension.FromValue(value);
            }
        }

        _texture = new(_dimensions, _dimensions);
        _texture.SetPixels(colors.Cast<Color>().ToArray());
        _texture.filterMode = FilterMode.Point;
        _texture.Apply();

        Material tempMaterial = new(_meshRenderer.sharedMaterial);
        _meshRenderer.sharedMaterial = tempMaterial;
        _meshRenderer.sharedMaterial.mainTexture = _texture;
    }

    /// <summary>
    ///  Is the material an asset? 
    /// </summary>
    /// <param name="material"></param>
    /// <returns></returns>
    private bool IsAsset(Material material) => material.GetInstanceID() < 0;
}