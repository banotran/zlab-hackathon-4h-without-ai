
using System;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorDatabse", menuName = "Game/ColorDB")]
public class ColorDatabase : ScriptableObject
{
    public List<ColorMaterialEntry> _colorMats;


    public Material GetMatById(ColorId id)
    {
        foreach(var c in _colorMats)
        {
            if (id == c.colorId)
                return c.material;
        }
        return default;
    }

    public Color GetColorById(ColorId id)
    {
        foreach(var c in _colorMats)
        {
            if (id == c.colorId)
                return c.Color;
        }
        return default;
    }

    public ColorId GetRandomColor()
    {
        int ran = UnityEngine.Random.Range(0, 6);
        return (ColorId)ran;
    }
}

[Serializable]
public class ColorMaterialEntry
{
    public ColorId colorId;
    public Color Color;
    public Material material;
}

