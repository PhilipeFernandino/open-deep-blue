using Coimbra;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Core.Map
{
    [ProjectSettings("Game Settings/Map")]
    public class TilesSettings : ScriptableSettings
    {
        [field: SerializeField] public TileToTileBase[] TileToTileBase { get; private set; }
        [field: SerializeField] public TileToTileBase[] TileToFloorTileBase { get; private set; }
        [field: SerializeField] public TileDefinition[] TileDefinitions { get; private set; }


        private TileDefinition[] _tileDefinitionLookup;
        private TileBase[] _tileToTileBaseLookup;
        private TileBase[] _tileToFloorTileBaseLookup;

        protected override void OnLoaded()
        {
            // Workaround unity serialization issue
            TileToTileBase = TileToTileBase.ToArray();
            TileDefinitions = TileDefinitions.ToArray();
            TileToFloorTileBase = TileToFloorTileBase.ToArray();

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

            foreach (var def in TileToTileBase)
            {
                _tileToTileBaseLookup[(int)def.TileType] = def.TileBase;
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
    }
}