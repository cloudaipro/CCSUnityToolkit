using System.Linq;
using System.Collections;
using System.Collections.Generic;



public class LineIndicator
{
    public static LineIndicator Instance = new LineIndicator();
    public int[,] Line_data = new int[9, 9];
    public int[] Line_data_falt = new int[81];
    public int[,] Square_data = new int[9, 9];
    public LineIndicator()
    {
        int start_num = 0, iSquareRow = 0, iSquareRowSub = 0, iSquareCol = 0, iSquareColSub = 0;
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
            {
                Line_data[r, c] = start_num;
                Line_data_falt[start_num] = start_num;

                iSquareRow = r / 3;
                iSquareRowSub = r % 3;
                iSquareCol = c / 3;
                iSquareColSub = c % 3;
                Square_data[iSquareRow * 3 + iSquareCol, iSquareRowSub * 3 + iSquareColSub] = start_num;

                start_num++;
            }                  
    }

    private (int, int) GetSquarePosition(int square_index)
    {
        return ((int)(square_index / 9), square_index % 9);        
    }

    public int[] GetHorizontalLine(int square_index) => Line_data.row(GetSquarePosition(square_index).Item1);
    public int[] GetVerticalLine(int square_index) => Line_data.column(GetSquarePosition(square_index).Item2);
    public int[] GetSquare(int square_index)
    {
        var squarePos = GetSquarePosition(square_index);
        return Square_data.row(((squarePos.Item1 / 3) * 3 + squarePos.Item2 / 3));
    }
    public int[] GetAllRelatedSudokuCell(int square_index) =>
        GetSquare(square_index)
            .Union(GetHorizontalLine(square_index))
            .Union(GetVerticalLine(square_index)).Distinct().ToArray();

    public int[] GetAllSquaresIndexes() => Line_data_falt;
    
}
