using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Systems.GridSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private WormPass _wormPass;
        [SerializeField] private CustomGrid _grid;
        [SerializeField] private int _dimensions;

        [SerializeField] private Tiles_SO _tileData;

        private Dictionary<GroundTile, Tile> _tileDataDic = new();

        [Button]
        private void Generate()
        {
            _tileDataDic.Clear();

            foreach (var data in _tileData.Tiles)
            {
                _tileDataDic.Add(data.GroundTile, data.Tile);
            }

            float[,] map = new float[_dimensions, _dimensions];

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    map[i, j] = -1f;
                }
            }

            _wormPass.MakePass(_dimensions, map);

            Vector3Int[,] positions = new Vector3Int[_dimensions, _dimensions];
            TileBase[,] tileData = new TileBase[_dimensions, _dimensions];

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    positions[i, j] = new Vector3Int(i, j, 0);

                    float noiseValue = Helper.NoiseTo01Bound(map[i, j]);

                    tileData[i, j] = noiseValue < 0.5f ? _tileDataDic[GroundTile.Stone] : null;
                }
            }

            _tilemap.SetTiles(positions.Cast<Vector3Int>().ToArray(), tileData.Cast<TileBase>().ToArray());
        }

    }
}