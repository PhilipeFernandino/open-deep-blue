using Coimbra;
using Coimbra.Services;
using Core.Debugger;
using Core.Map;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Level
{
    public class ChemicalGridManager : Actor, IChemicalGridService
    {
        [Header("Chemical Definitions")]
        [SerializeField] private List<ChemicalDefinition> _chemicalDefs = new();

        [Header("Chemical Emission")]
        [SerializeField] private List<TileChemicalEmission> _tileEmissions = new();

        [Header("Chemical Propagation")]
        [Range(1, 32)][SerializeField] private int _propagationPasses = 8;

        [Header("Debugging")]
        [SerializeField] private bool _debug = true;
        [SerializeField] private DebugChannelSO _debugChannel;
        List<ChemicalStrength> _strenghts = new();

        private readonly Dictionary<Chemical, ChemicalMap> _chemicalMaps = new();
        private readonly Dictionary<Tile, List<ChemicalEmission>> _chemicalEmission = new();

        private IGridService _gridService;
        private IObstacleService _obstacleService;

        private NativeArray<float> _readBuffer;
        private NativeArray<float> _writeBuffer;

        private bool _isInitialized = false;
        private int _dimensions;

        public int Dimensions => _dimensions;

        public event Action Initialized;

        public bool TryGetMap(Chemical chemical, out ChemicalMap value)
        {
            return _chemicalMaps.TryGetValue(chemical, out value);
        }

        public ChemicalMap GetMap(Chemical chemical) => _chemicalMaps.GetValueOrDefault(chemical);
        public float Get(int x, int y, Chemical chemical) => GetMap(chemical).Get(x, y);
        public void Drop(int x, int y, Chemical chemical, float value) => GetMap(chemical)?.Sum(x, y, value);
        public void Remove(int x, int y, Chemical chemical, float value) => GetMap(chemical)?.Sum(x, y, -value);
        public void Clean(int x, int y, Chemical chemical) => GetMap(chemical).Set(x, y, 0);

        public void SumAllOnMap(Chemical chemicalType, float value)
        {
            ChemicalMap targetMap = GetMap(chemicalType);
            if (targetMap == null)
                return;

            var job = new SumAllJob
            {
                Grid = targetMap.Grid,
                ValueToAdd = value
            };

            job.Schedule(targetMap.Grid.Length, 64).Complete();
        }


        protected override void OnInitialize()
        {
            ServiceLocator.Set<IChemicalGridService>(this);

            foreach (var tileEmission in _tileEmissions)
            {
                _chemicalEmission[tileEmission.Tile] = tileEmission.ChemicalEmissions;
            }
        }

        protected override void OnSpawn()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _obstacleService = ServiceLocatorUtilities.GetServiceAssert<IObstacleService>();
            _gridService.Initialized += Setup;
        }

        private void Setup()
        {
            OnDestroyed();

            _dimensions = _gridService.Dimensions;

            int gridSize = _dimensions * _dimensions;

            foreach (Chemical chemicalType in Enum.GetValues(typeof(Chemical)))
            {
                _chemicalMaps.Add(chemicalType, new ChemicalMap(_dimensions, Allocator.Persistent));
            }

            _readBuffer = new NativeArray<float>(gridSize, Allocator.Persistent);
            _writeBuffer = new NativeArray<float>(gridSize, Allocator.Persistent);

            _isInitialized = true;

            UnityEngine.Debug.Log("PheromoneGrid Manager Initialized.");

            Initialized?.Invoke();
        }

        protected override void OnDestroyed()
        {
            foreach (var map in _chemicalMaps.Values)
            {
                map.Dispose();
            }

            if (_readBuffer.IsCreated)
            {
                _readBuffer.Dispose();
            }

            if (_writeBuffer.IsCreated)
            {
                _writeBuffer.Dispose();
            }

            _chemicalMaps.Clear();
        }

        private void Update()
        {
            RaiseDebug();
        }

        [System.Diagnostics.Conditional(conditionString: "DEBUG"), System.Diagnostics.Conditional(conditionString: "UNITY_EDITOR")]
        private void RaiseDebug()
        {
            if (!_debug)
                return;

            _strenghts.Clear();

            var mousePos = Mouse.current.position.ReadValue();
            var pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            var (x, y) = IGridService.ToGridPosition(pos);


            foreach (var (key, map) in _chemicalMaps)
            {
                _strenghts.Add(new() { ChemicalType = key, Strength = map.Get(x, y) });
            }

            _debugChannel.RaiseEvent("chemical", new ChemicalDebugData()
            {
                Chemicals = _strenghts
            });
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
                return;

            UpdateTileBasedChemicals();

            foreach (var def in _chemicalDefs)
            {
                PropagateAndDecay(def);
            }
        }

        private void UpdateTileBasedChemicals()
        {
            if (_gridService?.Grid == null)
                return;


            var grid = _gridService.Grid;
            for (int x = 0; x < _dimensions; x++)
            {
                for (int y = 0; y < _dimensions; y++)
                {
                    TileInstance tile = grid[x, y];

                    if (_chemicalEmission.TryGetValue(tile.TileType, out var emissions))
                    {
                        foreach (var emission in emissions)
                        {
                            _chemicalMaps[emission.Chemical].Sum(x, y, emission.Strength * Time.deltaTime);
                        }
                    }
                }
            }
        }


        private void PropagateAndDecay(ChemicalDefinition def)
        {
            ChemicalMap map = GetMap(def.ChemicalType);

            if (map == null)
                return;

            NativeArray<float> targetGrid = map.Grid;

            RunPropagateAndDecayJob(targetGrid, def);
        }

        private void RunPropagateAndDecayJob(NativeArray<float> map, ChemicalDefinition def)
        {
            _readBuffer.CopyFrom(map);
            var obstacleGrid = _obstacleService.GetObstacleGrid();

            JobHandle jobHandle = default;

            for (int i = 0; i < _propagationPasses; i++)
            {
                var job = new PropagateAndDecayJob
                {
                    ReadGrid = _readBuffer,
                    WriteGrid = _writeBuffer,
                    ObstacleGrid = obstacleGrid,
                    GridDimensions = _dimensions,
                    DiffusionFactor = def.DiffusionFactor,
                    PropagationDecayMultiplier = def.PropagationFactor,
                    EvaporationMultiplier = (i == _propagationPasses - 1) ? def.DecayRate : 1.0f
                };
                jobHandle = job.Schedule(map.Length, 64);
                jobHandle.Complete();

                (_readBuffer, _writeBuffer) = (_writeBuffer, _readBuffer);
            }

            map.CopyFrom(_readBuffer);
        }
    }
}