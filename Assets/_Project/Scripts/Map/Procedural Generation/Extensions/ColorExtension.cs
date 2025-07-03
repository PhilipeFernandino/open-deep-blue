using UnityEngine;

public static class ColorExtension
{
    public static Color FromValue(float value, float alpha = 1f)
    {
        return new Color(value, value, value, alpha);
    }

    public static Color Divide(this Color thisColor, Color otherColor)
    {
        float r = thisColor.r / otherColor.r;
        float g = thisColor.g / otherColor.g;
        float b = thisColor.b / otherColor.b;
        float a = thisColor.a;

        Color resultColor = new Color(r, g, b, a);

        return resultColor;
    }

    public static Color Difference(this Color thisColor, Color otherColor)
    {
        float r = Mathf.Abs(thisColor.r - otherColor.r);
        float g = Mathf.Abs(thisColor.g - otherColor.g);
        float b = Mathf.Abs(thisColor.b - otherColor.b);
        float a = Mathf.Abs(thisColor.a - otherColor.a);

        Color resultColor = new Color(r, g, b, a);

        return resultColor;
    }

    public static Color DarkenOnly(this Color firstColor, Color secondColor)
    {
        float r = Mathf.Min(firstColor.r, secondColor.r);
        float g = Mathf.Min(firstColor.g, secondColor.g);
        float b = Mathf.Min(firstColor.b, secondColor.b);
        float a = firstColor.a;

        Color resultColor = new Color(r, g, b, a);

        return resultColor;
    }

    public static Color LightenOnly(this Color thisColor, Color otherColor)
    {
        float r = Mathf.Max(thisColor.r, otherColor.r);
        float g = Mathf.Max(thisColor.g, otherColor.g);
        float b = Mathf.Max(thisColor.b, otherColor.b);
        float a = thisColor.a;

        Color resultColor = new Color(r, g, b, a);

        return resultColor;
    }

    public static Color Overlay(this Color thisColor, Color otherColor)
    {
        float r, g, b;

        if (otherColor.r < 0.5f)
        {
            r = 2 * otherColor.r * thisColor.r;
        }
        else
        {
            r = 1 - 2 * (1 - otherColor.r) * (1 - thisColor.r);
        }

        if (otherColor.g < 0.5f)
        {
            g = 2 * otherColor.g * thisColor.g;
        }
        else
        {
            g = 1 - 2 * (1 - otherColor.g) * (1 - thisColor.g);
        }

        if (otherColor.b < 0.5f)
        {
            b = 2 * otherColor.b * thisColor.b;
        }
        else
        {
            b = 1 - 2 * (1 - otherColor.b) * (1 - thisColor.b);
        }

        return new Color(r, g, b, thisColor.a);
    }
}
