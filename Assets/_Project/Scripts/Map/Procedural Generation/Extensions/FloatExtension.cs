using System.Runtime.CompilerServices;
using UnityEngine;

public static class FloatExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Difference(this float thisFloat, float otherFloat)
    {
        return Mathf.Abs(thisFloat - otherFloat);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DarkenOnly(this float thisFloat, float otherFloat)
    {
        return Mathf.Min(thisFloat, otherFloat);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float LightenOnly(this float thisFloat, float otherFloat)
    {
        return Mathf.Max(thisFloat, otherFloat);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Overlay(this float thisFloat, float otherFloat)
    {
        float result;
        if (otherFloat < 0.5f)
        {
            result = 2 * otherFloat * thisFloat;
        }
        else
        {
            result = 1 - 2 * (1 - otherFloat) * (1 - thisFloat);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Blend(this float thisFloat, float otherFloat, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Add:
                thisFloat += otherFloat;
                break;
            case BlendMode.Multiply:
                thisFloat *= otherFloat;
                break;
            case BlendMode.Subtract:
                thisFloat -= otherFloat;
                break;
            case BlendMode.Divide:
                thisFloat /= otherFloat;
                break;
            case BlendMode.Modulo:
                thisFloat %= otherFloat;
                break;
            case BlendMode.Difference:
                thisFloat = thisFloat.Difference(otherFloat);
                break;
            case BlendMode.DarkenOnly:
                thisFloat = thisFloat.DarkenOnly(otherFloat);
                break;
            case BlendMode.LightenOnly:
                thisFloat = thisFloat.LightenOnly(otherFloat);
                break;
            case BlendMode.Overlay:
                thisFloat = thisFloat.Overlay(otherFloat);
                break;
            case BlendMode.Overwrite:
                thisFloat = otherFloat;
                break;
        }

        return thisFloat;
    }

    /// <summary>
    /// Blend the values with the other values. Does not alloc.
    /// </summary>
    /// <param name="theseFloats"></param>
    /// <param name="otherFloats"></param>
    /// <param name="dimensions"></param>
    /// <param name="blendMode"></param>
    /// <returns></returns>
    public static float[,] Blend(this float[,] theseFloats, float[,] otherFloats, int dimensions, BlendMode blendMode)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                float noisePassValue = otherFloats[i, j];
                switch (blendMode)
                {
                    case BlendMode.Add:
                        theseFloats[i, j] += noisePassValue;
                        break;
                    case BlendMode.Multiply:
                        theseFloats[i, j] *= noisePassValue;
                        break;
                    case BlendMode.Subtract:
                        theseFloats[i, j] -= noisePassValue;
                        break;
                    case BlendMode.Divide:
                        theseFloats[i, j] /= noisePassValue;
                        break;
                    case BlendMode.Modulo:
                        theseFloats[i, j] %= noisePassValue;
                        break;
                    case BlendMode.Difference:
                        theseFloats[i, j] = theseFloats[i, j].Difference(noisePassValue);
                        break;
                    case BlendMode.DarkenOnly:
                        theseFloats[i, j] = theseFloats[i, j].DarkenOnly(noisePassValue);
                        break;
                    case BlendMode.LightenOnly:
                        theseFloats[i, j] = theseFloats[i, j].LightenOnly(noisePassValue);
                        break;
                    case BlendMode.Overlay:
                        theseFloats[i, j] = theseFloats[i, j].Overlay(noisePassValue);
                        break;
                    case BlendMode.Overwrite:
                        theseFloats[i, j] = noisePassValue;
                        break;
                }
            }
        }

        return theseFloats;
    }
}