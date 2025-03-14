using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialsHandle
{
    public static float GetEmissionMultiplier(Material mat)
    {
        if (mat == null) return -1;
        var colour = mat.GetColor("_EmissionColor");
        return (colour.r + colour.g + colour.b) / 3 + 1;
    }
}
