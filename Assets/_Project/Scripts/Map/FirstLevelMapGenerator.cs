using Coimbra;
using Coimbra.Services;
using Core.ProcGen;
using Core.Utils;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TNRD;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;
using static Helper;
using Debug = UnityEngine.Debug;
using ReadOnly = Unity.Collections.ReadOnlyAttribute;

namespace Core.Map
{
    public class FirstLevelMapGenerator : Actor, IFirstLevelMapGeneratorService
    {
        [SerializeField] private Tile[,] _map;

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

        #region Debug
        [SerializeField] private bool _debug;
        [SerializeField] private bool _parallel;
        [SerializeField] private bool _generateOnAwake;


        #endregion

        [BurstCompile]
        public struct ValueTileJob : IJobParallelFor
        {
            public NativeArray<Tile> Map;
            [ReadOnly] public NativeArray<float> CaveMap;
            [ReadOnly] public NativeArray<float> BiomeMap;
            [ReadOnly] public NativeArray<float> OreMap;
            public int Dimensions;
            public bool GenerateBiomes;
            public bool GenerateOres;
            [ReadOnly] public NativeArray<ValueToTile> StoneValueTiles;
            [ReadOnly] public NativeArray<ValueToTile> OreValueTiles;

            public void Execute(int index)
            {
                float caveValue = CaveMap[index];
                if (caveValue == 1f)
                {
                    Map[index] = Tile.None;
                }
                else
                {
                    Tile currentTile = Map[index]; // Initially BlueStone from InitMap

                    if (GenerateBiomes)
                    {
                        float biomeValue = BiomeMap[index];
                        for (int k = 0; k < StoneValueTiles.Length; k++)
                        {
                            ValueToTile vt = StoneValueTiles[k];
                            if (biomeValue >= vt.Range.x && biomeValue <= vt.Range.y)
                            {
                                currentTile = vt.Tile;
                            }
                        }
                    }

                    if (GenerateOres)
                    {
                        float oreValue = OreMap[index];
                        for (int k = 0; k < OreValueTiles.Length; k++)
                        {
                            ValueToTile vt = OreValueTiles[k];
                            if (oreValue >= vt.Range.x && oreValue <= vt.Range.y)
                            {
                                currentTile = vt.Tile;
                            }
                        }
                    }

                    Map[index] = currentTile;
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (_generateOnAwake)
            {
                GenerateMapLevel();
            }
        }

        [Button]
        public Tile[,] GenerateMapLevel()
        {
            Stopwatch sw = Stopwatch.StartNew();

            float[,] caveMap = _basePass.MakePass(_dimensions);
            float[,] oreMap = _oreNoiseMap.Value.CreateMap(_dimensions);
            float[,] biomeMap = _biomeNoiseMap.Value.CreateMap(_dimensions);

            if (_parallel)
            {

                int total = _dimensions * _dimensions;
                NativeArray<Tile> mapNative = new NativeArray<Tile>(total, Allocator.TempJob);
                NativeArray<float> caveMapNative = new NativeArray<float>(total, Allocator.TempJob);
                NativeArray<float> oreMapNative = new NativeArray<float>(total, Allocator.TempJob);
                NativeArray<float> biomeMapNative = new NativeArray<float>(total, Allocator.TempJob);

                // Copy data to NativeArrays
                for (int i = 0; i < _dimensions; i++)
                {
                    for (int j = 0; j < _dimensions; j++)
                    {
                        int index = i * _dimensions + j;
                        caveMapNative[index] = caveMap[i, j];
                        oreMapNative[index] = oreMap[i, j];
                        biomeMapNative[index] = biomeMap[i, j];
                    }
                }

                NativeArray<ValueToTile> stoneValueTilesNative = new NativeArray<ValueToTile>(_stoneValueTiles, Allocator.TempJob);
                NativeArray<ValueToTile> oreValueTilesNative = new NativeArray<ValueToTile>(_oreValueTiles, Allocator.TempJob);

                var job = new ValueTileJob
                {
                    Map = mapNative,
                    CaveMap = caveMapNative,
                    BiomeMap = biomeMapNative,
                    OreMap = oreMapNative,
                    Dimensions = _dimensions,
                    GenerateBiomes = _generateBiomes,
                    GenerateOres = _generateOres,
                    StoneValueTiles = stoneValueTilesNative,
                    OreValueTiles = oreValueTilesNative
                };

                JobHandle handle = job.Schedule(total, 128);
                handle.Complete();

                _map = new Tile[_dimensions, _dimensions];

                // Copy back to 2D array
                for (int i = 0; i < _dimensions; i++)
                {
                    for (int j = 0; j < _dimensions; j++)
                    {
                        int index = i * _dimensions + j;
                        _map[i, j] = mapNative[index];
                    }
                }

                // Cleanup
                mapNative.Dispose();
                caveMapNative.Dispose();
                oreMapNative.Dispose();
                biomeMapNative.Dispose();
                stoneValueTilesNative.Dispose();
                oreValueTilesNative.Dispose();
            }
            else
            {
                InitMap(ref _map, _dimensions, Tile.BlueStone);
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
                            if (_generateBiomes)
                            {
                                // Using the biome value tiles to fill the map with biome stones
                                ValueTileToTile(_stoneValueTiles, biomeMap[i, j], i, j);
                            }

                            if (_generateOres)
                            {
                                // Using the value tiles to convert the ore map to the tilemap 
                                ValueTileToTile(_oreValueTiles, oreMap[i, j], i, j);
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

            return _map;
        }

        private void ValueTileToTile(ValueToTile[] valueTiles, float tileValue, int i, int j)
        {
            for (int k = 0; k < valueTiles.Length; k++)
            {
                Vector2 range = valueTiles[k].Range;
                if (tileValue >= range.x && tileValue <= range.y)
                {
                    _map[i, j] = valueTiles[k].Tile;
                }
            }
        }

        private void SpawnChests(Tile[,] map, WormPass cavePass)
        {
            List<Vector2Int> rooms = cavePass.Rooms;

            for (int i = 0; i < rooms.Count; i++)
            {
                if (ChanceUtil.EventSuccess(_roomChestChance))
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
            int randomIndex = UnityEngine.Random.Range(0, antQueenPossiblePositions.Count);
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

    [DynamicService]
    public interface IFirstLevelMapGeneratorService : IMapLevelGeneratorService
    {
    }
}
