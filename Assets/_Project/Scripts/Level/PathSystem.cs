using Coimbra;
using Coimbra.Services;
using Core.Collections;
using Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    public class PathSystem : Actor, IPathService
    {
        private IGridService _gridService;

        private PriorityQueue<Vector2Int, float> _open = new();
        private HashSet<Vector2Int> _closed = new();

        Dictionary<Vector2Int, float> _gCost = new();
        Dictionary<Vector2Int, Vector2Int> _parent = new();

        private readonly Vector2Int[] _neighboors = new Vector2Int[]
        {
            Vector2Int.down,
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.right,
            new(-1, -1),
            new(-1, 1),
            new(1, -1),
            new(1, -1),
        };

        public bool TryFindPath(Vector2 from, Vector2 to, in List<Vector2Int> path, int maxIterations = int.MaxValue)
        {
            Vector2Int start = Vector2Int.RoundToInt(from);
            Vector2Int end = Vector2Int.RoundToInt(to);

            _open.Clear();
            _closed.Clear();
            _gCost.Clear();
            _parent.Clear();

            _gCost[start] = 0;
            _open.Enqueue(start, Heuristic(start, end));
            int iterations = 0;

            float bestValue = 0;
            Vector2Int bestFound = start;

            while (_open.Count > 0)
            {
                iterations++;
                var current = _open.Dequeue();

                if (current == end || iterations == maxIterations)
                {
                    if (iterations == maxIterations)
                    {
                        current = bestFound;
                    }

                    while (_parent.TryGetValue(current, out var pr))
                    {
                        path.Add(current);
                        current = pr;
                    }

                    if (path.Count > 0)
                    {
                        path.Reverse();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                _closed.Add(current);

                foreach (var nb in GeneratePermutations(current))
                {
                    float currG = _gCost[current] + Heuristic(current, nb);

                    if (!_closed.Contains(nb) || currG < _gCost.GetValueOrDefault(nb, int.MaxValue))
                    {
                        _parent[nb] = current;
                        _gCost[nb] = currG;
                        float est = currG + Heuristic(nb, end);

                        if (currG > bestValue)
                        {
                            bestValue = currG;
                            bestFound = nb;
                        }

                        _open.Enqueue(nb, est);
                    }
                }
            }

            return false;
        }

        private float Heuristic(Vector2Int a, Vector2Int b)
        {
            int dx = Math.Abs(a.x - b.x);
            int dy = Math.Abs(a.y - b.y);
            return Math.Max(dx, dy); // Chebyshev for 8-direction grids
        }

        private List<Vector2Int> GeneratePermutations(Vector2Int node)
        {
            List<Vector2Int> perms = new(_neighboors.Length);

            for (int i = 0; i < _neighboors.Length; i++)
            {
                Vector2Int pos = node + _neighboors[i];
                if (_gridService.IsTileLoaded(pos) && !_gridService.HasTileAt(pos))
                {
                    perms.Add(pos);
                }
            }

            return perms;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ServiceLocator.Set<IPathService>(this);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
        }
    }

    [DynamicService]
    public interface IPathService : IService
    {
        public bool TryFindPath(Vector2 start, Vector2 end, in List<Vector2Int> path, int maxIterations = int.MaxValue);
    }
}