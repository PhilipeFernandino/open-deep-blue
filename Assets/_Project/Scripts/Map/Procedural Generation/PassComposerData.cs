using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pass Composer")]
public class PassComposerData : ScriptableObject
{
    [SerializeField]
    private List<PassComposerItemData> _passes;

    public List<PassComposerItemData> Passes => _passes;

    [Serializable]
    public class PassComposerItemData
    {
        [SerializeField]
        private bool _makePass = true;

        [Header("References")]
        [SerializeField]
        [Expandable]
        private PassDataBase _passData;

        public bool Active => _makePass;

        public float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null)
        {
            return _passData.MakePass(dimensions, random, map);
        }
    }
}

