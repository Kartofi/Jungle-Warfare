using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath
{
    public static float Round(float number, float numbersAfter)
    {
        float after = Mathf.Round(numbersAfter);
        float power = Mathf.Pow(10f, after);
        return Mathf.Round(number * power) / power;
    }
    public static Vector3 RoundVector(Vector3 vector, float numbersAfter)
    {
        Vector3 result = new Vector3(Round(vector.x, numbersAfter), Round(vector.y, numbersAfter), Round(vector.z, numbersAfter));
        return result;
    }
}
