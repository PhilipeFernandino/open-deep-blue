using Coimbra;
using Core.Utils;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

using static Helper;

using Debug = UnityEngine.Debug;

namespace Core.Map
{
    public class FirstLevelMapGenerator : MonoBehaviour
    {
        [SerializeField] private Tile[,] _map;

        [SerializeField] private WormPass _basePass;
        [SerializeField] private NoiseMapData _oreNoiseMap;

        [SerializeField] private int _dimensions;
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private List<TileToColor> _colorTile = new();

        // TODO: intervalos de cores para tiles 

        [SerializeField] private ValueToTile[] _valueTile;

        #region Debug
        [SerializeField] private bool _debug;
        #endregion

        [Button]
        private void Generate()
        {
            Stopwatch sw = Stopwatch.StartNew();

            InitMap(ref _map, _dimensions, Tile.BlueStone);

            float[,] caveMap = _basePass.MakePass(_dimensions);
            var oreMap = _oreNoiseMap.GetNoiseMap(_dimensions);

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    if (caveMap[i, j] == 1f)
                    {
                        _map[i, j] = Tile.None;
                    }
                    else
                    {
                        float v = oreMap[i, j];

                        // Using the value tiles to convert the ore map to the tilemap 
                        for (int k = 0; k < _valueTile.Length; k++)
                        {
                            Vector2 range = _valueTile[k].Range;
                            if (v >= range.x && v < range.y)
                            {
                                _map[i, j] = _valueTile[k].Tile;
                            }
                        }

                    }

                }
            }

            MakeQueenLair(_map);
            SpawnChests(_map, _basePass);

            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to complete first map gen");

            VisualizeColored(_map, _dimensions);

            if (_debug)
            {
                LogMapValueCount(caveMap, "map");
                LogMapValueCount(oreMap, "oreMap");
            }
        }

        private void SpawnChests(Tile[,] map, WormPass cavePass)
        {
            List<Vector2Int> rooms = cavePass.Rooms;

            float chestSpawnChance = 0.2f;

            for (int i = 0; i < rooms.Count; i++)
            {
                if (ChanceUtil.EventSuccess(chestSpawnChance))
                {
                    Vector2Int pos = rooms[i];
                    map[pos.x, pos.y] = Tile.Chest;
                }
            }
        }

        private void MakeQueenLair(Tile[,] map)
        {

            List<Vector2Int> antQueenPossiblePositions = _basePass.CaveDeadEnds;
            int randomIndex = UnityEngine.Random.Range(0, antQueenPossiblePositions.Count);
            Vector2Int antQueenRoomPosition = antQueenPossiblePositions[randomIndex];

            int antQueenRoomSize = 20;
            int halfRoom = antQueenRoomSize / 2;
            int areaAround = 3;

            // Clean the area
            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.None,
                new(-halfRoom - areaAround, halfRoom + areaAround),
                new(-halfRoom - areaAround, halfRoom + areaAround));

            // Mark the center as the ant queen spawner
            _map[antQueenRoomPosition.x, antQueenRoomPosition.y] = Tile.AntQueenSpawn;

            // Make walls by using the rectangle fn
            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.AntQueenRoomTile,
                new(-halfRoom, halfRoom),
                new(-halfRoom, -halfRoom));

            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.AntQueenRoomTile,
                new(-halfRoom, halfRoom),
                new(halfRoom, halfRoom));

            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.AntQueenRoomTile,
                new(halfRoom, halfRoom),
                new(-halfRoom, halfRoom));

            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.AntQueenRoomTile,
                new(-halfRoom, -halfRoom),
                new(-halfRoom, halfRoom));

            // Make doors
            int doorSize = 2;

            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.None,
                new(-doorSize / 2, doorSize / 2),
                new(-halfRoom, -halfRoom));
        }

        private void MakeRectangle(Tile[,] map, Vector2Int startingPosition, Tile tile, Vector2Int xBounds, Vector2Int yBounds)
        {
            for (int i = xBounds.x; i <= xBounds.y; i++)
            {
                for (int j = yBounds.x; j <= yBounds.y; j++)
                {
                    int x = i + startingPosition.x;
                    int y = j + startingPosition.y;

                    map[x, y] = tile;
                }
            }
        }

        private void VisualizeColored(Tile[,] map, int dimensions)
        {
            Dictionary<Tile, Color> colorTileDict = new(_colorTile.Count);

            foreach (var ctl in _colorTile)
            {
                colorTileDict.Add(ctl.Tile, ctl.Color);
            }

            Color[,] colors = new Color[dimensions, dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    colors[i, j] = colorTileDict[map[i, j]];
                }
            }

            Texture2D texture = new(dimensions, dimensions);
            texture.SetPixels(colors.Cast<Color>().ToArray());
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            Material tempMaterial = new(_meshRenderer.sharedMaterial);
            _meshRenderer.sharedMaterial = tempMaterial;
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }

        private void Visualize(float[,] map)
        {
            Color[,] colors = new Color[_dimensions, _dimensions];

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    colors[i, j] = ColorExtension.FromValue(NoiseTo01Bound(map[i, j]));
                }
            }

            Texture2D _texture = new(_dimensions, _dimensions);
            _texture.SetPixels(colors.Cast<Color>().ToArray());
            _texture.filterMode = FilterMode.Point;
            _texture.Apply();


            Material tempMaterial = new(_meshRenderer.sharedMaterial);
            _meshRenderer.sharedMaterial = tempMaterial;
            _meshRenderer.sharedMaterial.mainTexture = _texture;
        }

        private void LogMapValueCount(float[,] map, string append)
        {
            var valueCount = CountValues(map, _dimensions);
            StringBuilder sb = new($"{GetType()} - {append} - Map value count freq:\n");
            foreach (var (value, count) in valueCount)
            {
                sb.AppendLine($"{value} - {count}");
            }
            Debug.Log(sb.ToString());
        }
    }
}
