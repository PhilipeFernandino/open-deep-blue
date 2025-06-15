using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    [CustomEditor(typeof(TilemapEditorHelper))]
    public class TilemapEditorHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var helper = (TilemapEditorHelper)target;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Load From Asset"))
            {
                LoadTilesToScene(helper);
            }

            if (GUILayout.Button("Save To Asset"))
            {
                if (EditorUtility.DisplayDialog("Confirm Save",
                    "This will overwrite the selected TilemapAsset with the current scene data. Are you sure?",
                    "Save", "Cancel"))
                {
                    SaveTilesToAsset(helper);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void LoadTilesToScene(TilemapEditorHelper helper)
        {
            if (helper.TargetTilemap == null || helper.SourceAsset == null)
            {
                Debug.LogError("Target Tilemap or Source Asset is not assigned in the helper.", helper);
                return;
            }

            var map = new Dictionary<Tile, TileBase>();
            foreach (var mapping in helper.TileMappings)
            {
                if (!map.ContainsKey(mapping.Tile))
                {
                    map.Add(mapping.Tile, mapping.TileBase);
                }
            }

            var tileData = helper.SourceAsset.Tiles;
            var dimensions = helper.SourceAsset.Dimensions;

            helper.TargetTilemap.ClearAllTiles();

            for (int x = 0; x < dimensions; x++)
            {
                for (int y = 0; y < dimensions; y++)
                {
                    Tile tileEnum = tileData[x, y];
                    if (map.TryGetValue(tileEnum, out TileBase tileBase))
                    {
                        helper.TargetTilemap.SetTile(new Vector3Int(x, y, 0), tileBase);
                    }
                }
            }

            Debug.Log($"Loaded '{helper.SourceAsset.name}' into the scene tilemap.", helper);
        }

        private void SaveTilesToAsset(TilemapEditorHelper helper)
        {
            if (helper.TargetTilemap == null || helper.SourceAsset == null)
            {
                Debug.LogError("Target Tilemap or Source Asset is not assigned in the helper.", helper);
                return;
            }

            var map = new Dictionary<TileBase, Tile>();
            foreach (var mapping in helper.TileMappings)
            {
                if (mapping.TileBase != null && !map.ContainsKey(mapping.TileBase))
                {
                    map.Add(mapping.TileBase, mapping.Tile);
                }
            }

            var dimensions = helper.SourceAsset.Dimensions;
            var newTiles = new Tile[dimensions, dimensions];
            var newBiomeTiles = new Tile[dimensions, dimensions];

            for (int x = 0; x < dimensions; x++)
            {
                for (int y = 0; y < dimensions; y++)
                {
                    TileBase tileBase = helper.TargetTilemap.GetTile(new Vector3Int(x, y, 0));
                    if (tileBase != null && map.TryGetValue(tileBase, out Tile tileEnum))
                    {
                        newTiles[x, y] = tileEnum;
                    }
                    else
                    {
                        newTiles[x, y] = Tile.None;
                    }
                }
            }

            var mapMetadata = new MapMetadata(newTiles, newBiomeTiles, new List<PointOfInterest>(), dimensions);
            helper.SourceAsset.PopulateFrom(mapMetadata);

            EditorUtility.SetDirty(helper.SourceAsset);
            AssetDatabase.SaveAssets();

            Debug.Log($"Saved scene tilemap changes to '{helper.SourceAsset.name}'.", helper);
        }
    }
}