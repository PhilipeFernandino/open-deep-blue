using Coimbra;
using Coimbra.Services;
using Core.ProcGen;
using Core.Util;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using TNRD;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using static Helper;
using Debug = UnityEngine.Debug;

namespace Core.Map
{
    public class FirstLevelMapGenerator : Actor, IMapLevelGeneratorService
    {
        [SerializeField] private WormPass _basePass;
        [SerializeField] private SerializableInterface<IMapCreator> _oreNoiseMap;
        [SerializeField] private SerializableInterface<IMapCreator> _biomeNoiseMap;

        [SerializeField] private bool _generateOres;
        [SerializeField] private bool _generateBiomes;

        [SerializeField] private int _dimensions;
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private List<TileToColor> _colorTile = new();

        [SerializeField] private ValueToTile[] _stoneValueTiles;

        [FormerlySerializedAs("_valueTile")]
        [SerializeField] private ValueToTile[] _oreValueTiles;

        [SerializeField] private int _antQueenRoomSize = 20;
        [SerializeField] private int _areaAroundQueenRoom = 3;
        [SerializeField] private int _antQueenRoomDoorSize = 2;

        [SerializeField] private float _roomChestChance = 0.5f;
        [SerializeField] private int _seed;

        #region Debug
        [SerializeField] private bool _debug;
        [SerializeField] private bool _generateOnAwake;


        #endregion

        private CancellationTokenSource _cts;
        private IProgress<float> _generationProgress;
        private Tile[,] _asyncGeneratedMap;
        private System.Random _rng;

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IMapLevelGeneratorService>(this);
        }

        [Button]
        public async void GenerateMapLevelTask()
        {
            Stopwatch sw = Stopwatch.StartNew();
            var map = await GenerateMapLevel();
            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to complete first map gen");

            if (map != null)
            {
                if (_debug)
                {
                    VisualizeColored(map.Tiles, _dimensions);
                }
            }
        }

        public async UniTask<MapMetadata> GenerateMapLevel()
        {
            _rng = new System.Random(_seed);
            _cts = new CancellationTokenSource();
            _generationProgress = new Progress<float>(p =>
            {
                Debug.Log($"Generation progress: {p:P0}");
            });

            try
            {
                var (caveMap, oreMap, biomeMap) = await GenerateAllNoiseMapsAsync(_rng);

                return await UniTask.RunOnThreadPool(() =>
                {
                    var map = GenerateMapLevel(caveMap, oreMap, biomeMap);
                    return map;
                });
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async UniTask<(float[,], float[,], float[,])> GenerateAllNoiseMapsAsync(System.Random rng)
        {
            // Create independent RNGs for each task from main seed
            int baseSeed = rng.Next();
            int caveSeed = rng.Next();
            int oreSeed = rng.Next();
            int biomeSeed = rng.Next();

            // Start all noise generations concurrently
            var caveTask = UniTask.RunOnThreadPool(() =>
            {
                var caveRng = new System.Random(caveSeed);
                return _basePass.MakePass(_dimensions, caveRng);
            });

            var oreTask = UniTask.RunOnThreadPool(() =>
            {
                var oreRng = new System.Random(oreSeed);
                return _oreNoiseMap.Value.CreateMap(_dimensions, oreRng);
            });

            var biomeTask = UniTask.RunOnThreadPool(() =>
            {
                var biomeRng = new System.Random(biomeSeed);
                return _biomeNoiseMap.Value.CreateMap(_dimensions, biomeRng);
            });

            var results = await UniTask.WhenAll(caveTask, oreTask, biomeTask);
            return results;
        }

        public MapMetadata GenerateMapLevel(float[,] caveMap, float[,] oreMap, float[,] biomeMap)
        {
            Tile[,] tiles = new Tile[_dimensions, _dimensions];
            Tile[,] biomeTiles = new Tile[_dimensions, _dimensions];

            InitMap(ref tiles, _dimensions, Tile.BlueStone);


            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    ValueTileToTile(biomeTiles, _stoneValueTiles, biomeMap[i, j], i, j);
                }
            }

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    if (caveMap[i, j] == 1f)
                    {
                        tiles[i, j] = Tile.None;
                    }
                    else
                    {
                        if (_generateBiomes)
                        {
                            tiles[i, j] = biomeTiles[i, j];
                        }

                        if (_generateOres)
                        {
                            // Using the value tiles to convert the ore map to the tilemap 
                            ValueTileToTile(tiles, _oreValueTiles, oreMap[i, j], i, j);
                        }
                    }
                }
            }

            MakeQueenLair(tiles);
            SpawnChests(tiles, _basePass);

            if (_debug)
            {
                LogMapValueCount(caveMap, "map");
                LogMapValueCount(oreMap, "oreMap");
            }

            _asyncGeneratedMap = tiles;

            List<PointOfInterest> pois = new List<PointOfInterest>(_basePass.Rooms.Count + _basePass.CaveDeadEnds.Count);

            foreach (var p in _basePass.Rooms)
            {
                pois.Add(new PointOfInterest(p.x, p.y, PointOfInterestType.Room));
            }

            foreach (var p in _basePass.CaveDeadEnds)
            {
                pois.Add(new PointOfInterest(p.x, p.y, PointOfInterestType.CaveDeadEnd));
            }

            return new MapMetadata(tiles, biomeTiles, pois, _dimensions);
        }


        private void ValueTileToTile(Tile[,] map, ValueToTile[] valueTiles, float tileValue, int i, int j)
        {
            for (int k = 0; k < valueTiles.Length; k++)
            {
                Vector2 range = valueTiles[k].Range;
                if (tileValue >= range.x && tileValue <= range.y)
                {
                    map[i, j] = valueTiles[k].Tile;
                    return;
                }
            }
        }

        private void SpawnChests(Tile[,] map, WormPass cavePass)
        {
            List<Vector2Int> rooms = cavePass.Rooms;

            for (int i = 0; i < rooms.Count; i++)
            {
                if (ChanceUtil.EventSuccess(_roomChestChance, _rng))
                {
                    Vector2Int pos = rooms[i];

                    (int x, int y) = (pos.x, pos.y);

                    if (IsWithinMapCoordinates(x, y))
                    {
                        map[x, y] = Tile.Chest;
                    }
                }
            }
        }

        private void MakeQueenLair(Tile[,] map)
        {

            List<Vector2Int> antQueenPossiblePositions = _basePass.CaveDeadEnds;
            int randomIndex = Util.Random.Range(_rng, 0, antQueenPossiblePositions.Count);
            Vector2Int antQueenRoomPosition = antQueenPossiblePositions[randomIndex];

            _antQueenRoomSize = 20;
            int halfRoom = _antQueenRoomSize / 2;

            // Clean the area
            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.None,
                new(-halfRoom - _areaAroundQueenRoom, halfRoom + _areaAroundQueenRoom),
                new(-halfRoom - _areaAroundQueenRoom, halfRoom + _areaAroundQueenRoom));

            // Mark the center as the ant queen spawner
            map[antQueenRoomPosition.x, antQueenRoomPosition.y] = Tile.AntQueenSpawn;

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

            MakeRectangle(
                map,
                antQueenRoomPosition,
                Tile.None,
                new(-_antQueenRoomDoorSize / 2, _antQueenRoomDoorSize / 2),
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

                    if (IsWithinMapCoordinates(x, y))
                    {
                        map[x, y] = tile;
                    }
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

        private bool IsWithinMapCoordinates(int x, int y)
        {
            return IsWithinCoordinates(x, y, 0, 0, _dimensions - 1, _dimensions - 1);
        }
    }
}
