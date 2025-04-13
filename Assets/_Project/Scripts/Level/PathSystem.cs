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
        private bool _debug;

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
            new(1, 1),
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

            while (_open.Count > 0)
            {
                iterations++;

                if (iterations > maxIterations)
                {
                    return false;
                }

                var current = _open.Dequeue();

                _gridService.ClearDrawAt(current);
                DebugDraw(current, new Color(255, 0, 0, 0.3f), true);

                if (current == end)
                {
                    while (_parent.TryGetValue(current, out var pr))
                    {
                        path.Add(current);
                        DebugDraw(current, Color.green, true);
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

                foreach (var perm in GeneratePermutations(current))
                {
                    float currG = _gCost[current] + GCost(current, perm);

                    if (!_closed.Contains(perm) || currG < _gCost.GetValueOrDefault(perm, int.MaxValue))
                    {
                        DebugDraw(current, new Color(0, 0, 255, 0.3f));

                        _parent[perm] = current;
                        _gCost[perm] = currG;
                        float est = currG + Heuristic(perm, end);

                        _open.Enqueue(perm, est);
                    }
                }
            }

            return false;
        }

        private float GCost(Vector2Int a, Vector2Int b)
        {
            Vector2Int delta = a - b;
            bool isDiagonal = Math.Abs(delta.x) == 1 && Math.Abs(delta.y) == 1;
            return isDiagonal ? 1.4142f : 1f;
        }

        private float Heuristic(Vector2Int a, Vector2Int b)
        {
            int dx = Math.Abs(a.x - b.x);
            int dy = Math.Abs(a.y - b.y);
            int minD = Math.Min(dx, dy);
            int maxD = Math.Max(dx, dy);
            return maxD + (minD * 0.4142f);
        }

        private List<Vector2Int> GeneratePermutations(Vector2Int node)
        {
            List<Vector2Int> perms = new(_neighboors.Length);

            for (int i = 0; i < _neighboors.Length; i++)
            {
                Vector2Int pos = node + _neighboors[i];
                if (_gridService.IsTileLoaded(pos)
                    && !_gridService.HasTileAt(pos.x, node.y)
                    && !_gridService.HasTileAt(node.x, pos.y))
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

        private void DebugDraw(Vector2Int pos, Color color, bool clear = false)
        {
            if (_debug)
            {
                if (clear)
                {
                    _gridService.ClearDrawAt(pos);
                }
                _gridService.DrawInGrid(pos, color);
            }
        }
    }

    [DynamicService]
    public interface IPathService : IService
    {
        public bool TryFindPath(Vector2 start, Vector2 end, in List<Vector2Int> path, int maxIterations = int.MaxValue);
    }
}