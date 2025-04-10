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

        public bool TryFindPath(Vector2 from, Vector2 to, out List<Vector2Int> path)
        {
            Vector2Int start = Vector2Int.RoundToInt(from);
            Vector2Int end = Vector2Int.RoundToInt(to);

            var toVisit = new PriorityQueue<Vector2Int, float>();
            var visited = new HashSet<Vector2Int>();

            Dictionary<Vector2Int, float> gCost = new();
            Dictionary<Vector2Int, Vector2Int> parent = new();

            gCost[start] = 0;
            toVisit.Enqueue(start, Vector2Int.Distance(start, end));

            while (toVisit.Count > 0)
            {
                var current = toVisit.Dequeue();

                if (current == end)
                {
                    path = new();
                    while (parent.TryGetValue(current, out var pr))
                    {

                        path.Add(current);
                        current = pr;
                    }
                    path.Reverse();
                    return true;
                }

                visited.Add(current);

                foreach (var nb in GeneratePermutations(current))
                {
                    float currG = gCost[current] + Vector2Int.Distance(current, nb);

                    if (!visited.Contains(nb) || currG < gCost.GetValueOrDefault(nb, int.MaxValue))
                    {
                        parent[nb] = current;
                        gCost[nb] = currG;
                        float est = currG + Vector2Int.Distance(nb, end);
                        toVisit.Enqueue(nb, est);
                    }
                }
            }

            path = null;
            return false;
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
        public bool TryFindPath(Vector2 start, Vector2 end, out List<Vector2Int> path);
    }
}