using Core.ProcGen;
using NaughtyAttributes;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PassComposer : MonoBehaviour, IMapCreator
{
    [Header("Basic")]
    [SerializeField] private int _dimensions;

    [Expandable]
    [SerializeField]
    private PassComposerData _passComposerData;

    public int Dimensions => _dimensions;

    public float[,] CombinePasses()
    {
        return CombinePasses(_dimensions);
    }

    public float[,] CombinePasses(int dimensions, System.Random random = null)
    {
        Stopwatch sw = Stopwatch.StartNew();
        float[,] passValues = new float[dimensions, dimensions];

        bool isFirstPass = true;

        foreach (var pass in _passComposerData.Passes)
        {
            if (pass.Active)
            {
                if (isFirstPass)
                {
                    passValues = pass.MakePass(dimensions, random);
                    isFirstPass = false;
                }
                else
                {
                    passValues = pass.MakePass(dimensions, random, passValues);
                }
            }
        }

        sw.Stop();
        Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to compose noise passes");

        return passValues;
    }

    public float[,] CreateMap(int dimensions, System.Random random = null)
    {
        return CombinePasses(dimensions, random);
    }
}
