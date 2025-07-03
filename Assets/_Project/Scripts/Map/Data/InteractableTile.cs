using Core.Interaction;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    [System.Serializable]
    public class InteractableTile
    {
        [field: SerializeField] public Tile TileType { get; private set; }
        [field: SerializeField] public IInteractable Interactable { get; private set; }
    }
}