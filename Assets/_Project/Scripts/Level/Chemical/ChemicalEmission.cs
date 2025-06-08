using Core.Level;
using System;
using UnityEngine;

[Serializable]
public struct ChemicalEmission
{
    public Chemical Chemical;
    [Range(0f, 255f)] public float Strength;
}