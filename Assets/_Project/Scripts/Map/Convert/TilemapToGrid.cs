using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Core.Map
{

    public class TilemapToGrid : MonoBehaviour
    {
        [SerializeField] private TileBaseMappingSO _tileBaseMapping;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private int _dimensions;
        [SerializeField] private string _path;

        [Button("Create Tilemap Asset From This Tilemap")]
        private void CreateAsset()
        {
#if UNITY_EDITOR
            if (_tilemap == null)
            {
                Debug.LogError("Please assign a Tilemap to read from.");
                return;
            }

            var tilesDict = new Dictionary<TileBase, Tile>();
            foreach (var tile in _tileBaseMapping.TileBaseTiles)
            {
                tilesDict.Add(tile.TileBase, tile.Tile);
            }

            var tiles = new Tile[_dimensions, _dimensions];

            for (int x = 0; x < _dimensions; x++)
            {
                for (int y = 0; y < _dimensions; y++)
                {
                    var tilebase = _tilemap.GetTile(new Vector3Int(x, y, 0));
                    if (tilebase != null && tilesDict.ContainsKey(tilebase))
                    {
                        tiles[x, y] = tilesDict[tilebase];
                    }
                    else
                    {
                        tiles[x, y] = Tile.None;
                    }
                }
            }

            var mapMetadata = new MapMetadata(tiles, new Tile[_dimensions, _dimensions], new List<PointOfInterest>(), _dimensions);

            var newAsset = ScriptableObject.CreateInstance<TilemapAsset>();
            newAsset.PopulateFrom(mapMetadata);

            string path = $"{_path}/{_tilemap.gameObject.name}.asset";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(newAsset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;

            Debug.Log($"Successfully created asset at {path}");
#else
            Debug.LogWarning("This functionality is only available in the Unity Editor.");
#endif
        }
    }


}