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

        [field: SerializeField] public TileDefinition[] TileDefinitions { get; private set; }


        private TileDefinition[] _tileDefinitionLookup;
        private TileBase[] _tileToTileBaseLookup;

        protected override void OnLoaded()
        {
            TileToTileBase = TileToTileBase.ToArray();
            TileDefinitions = TileDefinitions.ToArray();

            SetTileDefinitions();
            SetTileToTileBase();
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
                Debug.Log(def);
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
    }
}