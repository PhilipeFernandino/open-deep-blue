using Coimbra;
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

        private Dictionary<Tile, Color> _colorTileDict;

        [Button]
        private void Generate()
        {
            Stopwatch sw = Stopwatch.StartNew();

            InitMap(ref _map, _dimensions, Tile.BlueStone);

            float[,] map = _basePass.MakePass(_dimensions);
            var oreMap = _oreNoiseMap.GetNoiseMap(_dimensions);
            Color[,] colors = new Color[_dimensions, _dimensions];

            _colorTileDict = new(_colorTile.Count);

            foreach (var ctl in _colorTile)
            {
                _colorTileDict.Add(ctl.Tile, ctl.Color);
            }

            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    if (map[i, j] == 1f)
                    {
                        _map[i, j] = Tile.None;
                    }
                    else
                    {
                        float v = oreMap[i, j];

                        for (int k = 0; k < _valueTile.Length; k++)
                        {
                            Vector2 range = _valueTile[k].Range;
                            if (v >= range.x && v < range.y)
                            {
                                _map[i, j] = _valueTile[k].Tile;
                            }
                        }

                        colors[i, j] = _colorTileDict[_map[i, j]];
                    }

                }
            }

            VisualizeColored(colors);

            if (_debug)
            {
                LogMapValueCount(map, "map");
                LogMapValueCount(oreMap, "oreMap");
            }

            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to complete first map gen");
        }

        private void VisualizeColored(Color[,] colors)
        {
            Texture2D _texture = new(_dimensions, _dimensions);
            _texture.SetPixels(colors.Cast<Color>().ToArray());
            _texture.filterMode = FilterMode.Point;
            _texture.Apply();


            Material tempMaterial = new(_meshRenderer.sharedMaterial);
            _meshRenderer.sharedMaterial = tempMaterial;
            _meshRenderer.sharedMaterial.mainTexture = _texture;
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

    [Serializable]
    public struct TileToColor
    {
        [field: SerializeField] public Tile Tile { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
    }

    [Serializable]
    public struct ValueToTile
    {
        [field: SerializeField, MinMaxSlider(-1f, 1f)] public Vector2 Range { get; private set; }
        [field: SerializeField] public Tile Tile { get; private set; }
    }
}
