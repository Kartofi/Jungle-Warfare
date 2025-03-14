using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayUtilities
{
    public static bool Equal(int[] firstArray, int[] secondArray)
    {
        if (firstArray.Length != secondArray.Length)
            return false;
        
        for (int i = 0; i < firstArray.Length; i++)
        {
            if (!firstArray[i].Equals(secondArray[i]))
            {
                return false;
            }
                
        }

        return true;
    }
}
