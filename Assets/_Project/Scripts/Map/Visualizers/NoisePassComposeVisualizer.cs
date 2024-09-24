using NaughtyAttributes;
using System.Linq;
using UnityEngine;

public class NoisePassComposeVisualizer : MonoBehaviour
{
    [SerializeField] private FilterMode _filterMode;

    [Header("References")]
    [SerializeField] private PassComposer _noisePassComposer;
    [SerializeField] private MeshRenderer _meshRenderer;

    private Texture2D _texture;


    [Button("Apply and Show")]
    private void ApplyNoisePassesToTexture()
    {
        int dimensions = _noisePassComposer.Dimensions;
        float[,] values = _noisePassComposer.CombinePasses();
        Color[,] colors = new Color[dimensions, dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                float value = Helper.NoiseTo01Bound(values[i, j]);
                colors[i, j] = ColorExtension.FromValue(value);
            }
        }

        _texture = new(dimensions, dimensions);
        _texture.SetPixels(colors.Cast<Color>().ToArray());
        _texture.filterMode = _filterMode;
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