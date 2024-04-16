using NaughtyAttributes;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PassComposer : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private int _dimensions;

    [Expandable]
    [SerializeField]
    private PassComposerData _passComposerData;

    public int Dimensions => _dimensions;

    public float[,] CombinePasses()
    {
        Stopwatch sw = Stopwatch.StartNew();
        float[,] passValues = new float[_dimensions, _dimensions];

        bool isFirstPass = true;

        foreach (var pass in _passComposerData.Passes)
        {
            if (pass.Active)
            {
                if (isFirstPass)
                {
                    passValues = pass.MakePass(_dimensions);
                    isFirstPass = false;
                }
                else
                {
                    passValues = pass.MakePass(_dimensions, passValues);
                }
            }
        }

        sw.Stop();
        Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to compose noise passes");

        return passValues;
    }
}
