using NaughtyAttributes;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NoiseToTexture : MonoBehaviour
{
    [SerializeField] private PassComposer _noisePassComposer;
    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private Color32 _colorA;
    [SerializeField] private Color32 _colorB;

    private Texture2D _texture;

    [Button("Make")]
    private void Make()
    {
        Stopwatch sw = Stopwatch.StartNew();

        float[,] noiseValues = _noisePassComposer.CombinePasses();
        int dimensions = _noisePassComposer.Dimensions;
        _texture = new(dimensions, dimensions);

        Color32[,] colorData = new Color32[dimensions, dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                float noiseValue = noiseValues[i, j];
                colorData[i, j] = noiseValue > 0.5 ? _colorA : _colorB;
            }
        }

        Color32[] flattenMap = colorData.Cast<Color32>().ToArray();

        _texture.SetPixels32(flattenMap);
        _texture.filterMode = FilterMode.Point;
        _texture.Apply();

        _meshRenderer.material.mainTexture = _texture;

        sw.Stop();
        Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to generate tilemap");
    }
}
