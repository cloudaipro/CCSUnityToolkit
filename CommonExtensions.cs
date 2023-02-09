using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonExtensions
{
    public static T[] column<T>(this T[,] multidimArray, int wanted_column)
    {
        int l = multidimArray.GetLength(0);
        T[] columnArray = new T[l];
        for (int i = 0; i < l; i++)
        {
            columnArray[i] = multidimArray[i, wanted_column];
        }
        return columnArray;
    }

    public static T[] row<T>(this T[,] multidimArray, int wanted_row)
    {
        int l = multidimArray.GetLength(1);
        T[] rowArray = new T[l];
        for (int i = 0; i < l; i++)
        {
            rowArray[i] = multidimArray[wanted_row, i];
        }
        return rowArray;
    }

    public static string LeadingZero(this int n, int totalWidth) => n.ToString().PadLeft(totalWidth, '0');
}
