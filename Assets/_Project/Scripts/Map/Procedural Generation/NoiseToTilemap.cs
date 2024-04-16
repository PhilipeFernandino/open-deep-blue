using NaughtyAttributes;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class NoiseToTilemap : MonoBehaviour
{
    [SerializeField] private PassComposer _noisePassComposer;
    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private TileBase _tileBaseA;
    [SerializeField] private TileBase _tileBaseB;

    [SerializeField, Range(0f, 1f)] private float _step = 0.5f;

    [Button("Make")]
    private void Make()
    {
        Stopwatch sw = Stopwatch.StartNew();

        float[,] noiseValues = _noisePassComposer.CombinePasses();
        int dimensions = _noisePassComposer.Dimensions;

        Vector3Int[,] positions = new Vector3Int[dimensions, dimensions];
        TileBase[,] tileData = new TileBase[dimensions, dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                positions[i, j] = new Vector3Int(i, j, 0);

                float noiseValue = Helper.NoiseTo01Bound(noiseValues[i, j]);
                tileData[i, j] = noiseValue > _step ? _tileBaseA : _tileBaseB;
            }
        }


        _tilemap.SetTiles(positions.Cast<Vector3Int>().ToArray(), tileData.Cast<TileBase>().ToArray());

        sw.Stop();
        Debug.Log($"{GetType()} - {sw.ElapsedMilliseconds} elapsed miliseconds to generate tilemap");
    }
}
