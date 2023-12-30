using System;
using UnityEngine;

[Serializable]
public struct PairColor
{
    public Color from;
    public Color to;

    public PairColor(Color from, Color to)
    {
        this.from = from;
        this.to = to;
    }
}

