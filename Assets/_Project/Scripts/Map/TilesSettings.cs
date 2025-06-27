using Coimbra;
using Core.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Core.Map
{



    [ProjectSettings("Core/Map")]
    public class TilesSettings : ScriptableSettings
    {
        [field: SerializeField] public TileBaseMappingSO TileToTileBase { get; private set; }
        [field: SerializeField] public TileToTileBase[] TileToFloorTileBase { get; private set; }
        [field: SerializeField] public TileDefinition[] TileDefinitions { get; private set; }
        [field: SerializeField] public InteractableTile[] InteractableTiles { get; private set; }


        private TileDefinition[] _tileDefinitionLookup;
        private TileBase[] _tileToTileBaseLookup;
        private TileBase[] _tileToFloorTileBaseLookup;
        private IInteractable[] _interactableTilesLookup;

        protected override void OnLoaded()
        {
            SetTileDefinitions();
            SetTileToTileBase();
            SetTileToFloorTileBase();
        }

        private void SetTileDefinitions()
        {
            int maxTileType = Enum.GetValues(typeof(Tile)).Cast<ushort>().Max();
            _tileDefinitionLookup = new TileDefinition[maxTileType + 1];

            foreach (var def in TileDefinitions)
            {
                _tileDefinitionLookup[(int)def.TileType] = def;
            }
        }

        private void SetTileToTileBase()
        {
            int maxTileType = Enum.GetValues(typeof(Tile)).Cast<ushort>().Max();
            _tileToTileBaseLookup = new TileBase[maxTileType + 1];

            foreach (var def in TileToTileBase.TileBaseTiles)
            {
                _tileToTileBaseLookup[(int)def.Tile] = def.TileBase;
            }
        }

        private void SetTileToFloorTileBase()
        {
            int maxTileType = Enum.GetValues(typeof(Tile)).Cast<ushort>().Max();
            _tileToFloorTileBaseLookup = new TileBase[maxTileType + 1];

            foreach (var def in TileToFloorTileBase)
            {
                _tileToFloorTileBaseLookup[(int)def.TileType] = def.TileBase;
            }
        }

        private void SetInteractableTiles()
        {
            int maxTileType = Enum.GetValues(typeof(Tile)).Cast<ushort>().Max();
            _interactableTilesLookup = new IInteractable[maxTileType + 1];

            foreach (var def in InteractableTiles)
            {
                _interactableTilesLookup[(int)def.TileType] = def.Interactable;
            }
        }

        public ref readonly TileDefinition GetDefinition(Tile tile)
        {
            return ref _tileDefinitionLookup[(int)tile];
        }

        public TileBase GetTileBase(Tile tile)
        {
            return _tileToTileBaseLookup[(int)tile];
        }

        public TileBase GetFloorTileBase(Tile tile)
        {
            return _tileToFloorTileBaseLookup[(int)tile];
        }

        public IInteractable GetInteractable(Tile tile)
        {
            return _interactableTilesLookup[(int)tile];
        }
    }
}