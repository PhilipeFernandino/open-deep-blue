using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Map
{
    public class AssetMapLoader : MonoBehaviour
    {
        [SerializeField] private TilemapAsset _mapToLoad;
        [SerializeField] private bool _loadOnStart = true;

        private void Start()
        {
            if (_loadOnStart)
            {
                LoadAndInvoke(_mapToLoad);
            }
        }

        public void LoadAndInvoke(TilemapAsset mapToLoad)
        {
            if (mapToLoad == null)
            {
                Debug.LogError("Map to load is null", this);
                return;
            }

            var mapMetadata = new MapMetadata(
                mapToLoad.Tiles,
                mapToLoad.BiomeTiles,
                new List<PointOfInterest>(),
                mapToLoad.Dimensions
            );

            new MapMetadataGeneratedEvent(mapMetadata).Invoke(this);

            Debug.Log($"Invoked {nameof(MapMetadataGeneratedEvent)} from asset '{mapToLoad.name}'.", this);
        }
    }
}