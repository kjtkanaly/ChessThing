using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class General
{
    public static T[,] vertVlipArray<T>(T[,] array)
    {
        T[,] output = new T[array.GetLength(0), array.GetLength(1)];
        int numRows = array.GetLength(0) - 1;

        for (int col = 0; col < array.GetLength(1); col++) {

            for (int row = 0; row < array.GetLength(0); row++) {
                output[col, numRows - row] = array[col, row];
            }
            
        }

        return output;
    }
}