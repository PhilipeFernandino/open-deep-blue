using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.Level
{
    public class GridDrawer
    {
        private GameObject _parent;
        private SpriteRenderer _spriteRenderer;

        private ObjectPool<SpriteRenderer> _spritePool;
        private LinkedList<SpriteRenderer> _activeSprites;

        private Vector2Int _lastDrawBounds;
        private Dictionary<Vector2Int, SpriteRenderer> _spriteAt;

        public GridDrawer(GameObject parent, SpriteRenderer spriteRenderer)
        {
            _parent = parent;
            _spriteRenderer = spriteRenderer;

            _spritePool = new(
                () =>
                {
                    return Object.Instantiate(_spriteRenderer, _parent.transform);
                },
                actionOnRelease: (e) => { e.enabled = false; },
                defaultCapacity: 20,
                maxSize: 500);

            _activeSprites = new();
            _spriteAt = new();
        }

        public void ClearAt(Vector2 position)
        {
            var vi = Vector2Int.RoundToInt(position);
            if (_spriteAt.TryGetValue(vi, out var sprite))
            {
                _spritePool.Release(sprite);
                _activeSprites.Remove(sprite);
                _spriteAt.Remove(vi);
            }

        }

        public void DrawInGrid(Vector2 position, Color color)
        {
            DrawInGrid(position, new Vector2Int(1, 1), color);
        }

        public void DrawInGrid(Vector2 position, in Vector2Int size, Color color)
        {
            var rounded = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int x = rounded.x + i;
                    int y = rounded.y + j;

                    if (_spriteAt.TryGetValue(new Vector2Int(x, y), out var spriteAt))
                    {
                        spriteAt.color = spriteAt.color + color;
                    }
                    else
                    {
                        var gridSprite = _spritePool.Get();

                        gridSprite.transform.position = new Vector2(x, y);
                        gridSprite.color = color;
                        gridSprite.enabled = true;

                        _activeSprites.AddLast(gridSprite);
                    }
                }
            }

            _lastDrawBounds = size;
        }

        public void Clear()
        {
            for (int i = 0; i < _activeSprites.Count - (_lastDrawBounds.x * _lastDrawBounds.y); i++)
            {
                var deactivate = _activeSprites.First.Value;
                _spritePool.Release(deactivate);
                _activeSprites.RemoveFirst();
            }
        }
    }
}