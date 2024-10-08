using Coimbra;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ItemSystem
{
    [ProjectSettings("Game Settings")]
    public class UIItemRarityConfig : ScriptableSettings
    {
        [SerializeField] private List<ItemRarityColorWrapper> _itemRarityColor;

        private Dictionary<ItemRarity, Color> _itemRarityColorDic;

        public Dictionary<ItemRarity, Color> ItemRarityColor
        {
            get
            {
                if (_itemRarityColorDic == null)
                {
                    _itemRarityColorDic = new(_itemRarityColor.Count);

                    foreach (var rarityColor in _itemRarityColor)
                    {
                        _itemRarityColorDic.Add(rarityColor.Rarity, rarityColor.Color);
                    }

                }

                return _itemRarityColorDic;
            }
        }

        [Serializable]
        private class ItemRarityColorWrapper
        {
            public ItemRarity Rarity;
            public Color Color;
        }
    }

}