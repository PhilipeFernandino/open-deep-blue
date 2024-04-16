using System.Collections.Generic;
using UnityEngine;

namespace Systems.GridSystem
{
    public class CustomGrid : MonoBehaviour
    {
        [SerializeField] private int _spritesPoolSize;
        [SerializeField] private Vector2 _gridDrawOffset = Vector2.zero;

        [SerializeField] private SpriteRenderer _gridSpriteRendererPrefab;

        private List<SpriteRenderer> _gridSprites;

        private TileData[,] _grid;
        private int _offset;
        private int _gridSize;
        private int _lastGridDrawSize = 0;
        private int _gridAxisRange;

        public int GridAxisRange => _gridAxisRange;

        private void Update()
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 10f;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;

            DrawInGrid(pos, new Vector2Int(1, 1));
        }

        public void Setup(int gridSize)
        {
            _gridSize = gridSize;
            _grid = new TileData[_gridSize, _gridSize];
        }

        public void DrawInGrid(Vector2 position, in Vector2Int size)
        {
            var rounded = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int x = rounded.x + i;
                    int y = rounded.y + j;

                    int index = j + i * size.y;

                    _gridSprites[index].transform.position = new Vector2(x, y) + _gridDrawOffset;

                    if (TryGetTileAt(x, y, out TileData tile))
                    {
                        _gridSprites[index].color = Color.white;
                    }
                    else
                    {
                        _gridSprites[index].color = Color.red;
                    }

                    _gridSprites[index].enabled = true;
                }
            }

            int gridDrawSize = size.x * size.y;
            for (int i = gridDrawSize; i < _lastGridDrawSize; i++)
            {
                _gridSprites[i].enabled = false;
            }
        }

        public bool TryGetTileAt(int x, int y, out TileData tile)
        {
            int xGrid = x + _offset;
            int yGrid = y + _offset;

            if ((xGrid < 0 || xGrid >= _gridSize) || (yGrid < 0 || yGrid >= _gridSize))
            {
                tile = TileData.None;
                return false;
            }

            tile = _grid[xGrid, yGrid];
            return true;
        }

        private void Awake()
        {
            InitializeDrawGridSprites();
        }

        private void InitializeDrawGridSprites()
        {
            _gridSprites = new(_spritesPoolSize);

            for (int i = 0; i < _spritesPoolSize; i++)
            {
                _gridSprites.Add(Instantiate(_gridSpriteRendererPrefab, transform));
                _gridSprites[i].enabled = false;
            }
        }
    }
}