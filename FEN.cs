using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEN : MonoBehaviour
{
    // Objects
    private MainGameDriver MGD;
    
    // Types
    public string FENString;
    const string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    const string colCord = "abcdefgh";
    const string rowCord = "12345678";
    public int[,] grid = new int[8, 8];

    // -----------------------------------------------------------------------------
    // FEN Related Functions

    public void InitFenObj(MainGameDriver mainGameDriver) {
        MGD = mainGameDriver;

        FENString = startingFEN;
    }
    
    public void DecodeFENString(string fenString) {
        fenString = fenString.Split(' ')[0];

        int boardIndex = 0;
        int y = 0;
        int x = 0;

        for (int fenIndex = 0; fenIndex < fenString.Length; fenIndex++)
        {
            if (fenString[fenIndex] == '/') {
                continue;
            }

            if (!System.Char.IsNumber(fenString, fenIndex)) {
                y = (int)(boardIndex / 8);
                x = (int)(boardIndex % 8);

                // Convert Char to piece value
                grid[x, y] = ChessPiece.getPieceValue(fenString[fenIndex]);
                boardIndex += 1;
            }
            else {
                boardIndex += int.Parse("" + fenString[fenIndex]);
            }
        }

        printGrid(grid);
    }


    public void convertBoardToString() {
        string outputFEN = "";

        for (int y = 0; y < grid.GetLength(0); y++) {
            int[] rowValues = getRowValues(grid, y);
            string rowString = "";

            foreach (int val in rowValues) {
                string valString = ChessPiece.getPieceLetterFromValue(val);

                rowString += valString;
            }

            int count = 0;
            string newRowString = "";
            foreach (char val in rowString) {
                if (val == '0') {
                    count += 1;
                }
                else {
                    newRowString += count.ToString();
                    count = 0;
                }
                
                newRowString += val;
            }

            newRowString += count.ToString();
            newRowString = newRowString.Replace("0", "");
            outputFEN += newRowString + "/";
        }

        string[] FENStrings = FENString.Split(" ");
        FENStrings[0] = outputFEN;
        FENString = string.Join(" ", FENStrings);
    }

    public int[] getRowValues(int[,] array, int y) {
        int[] rowValues = new int[array.GetLength(1)];

        for (int x = 0; x < array.GetLength(1); x++) {
            rowValues[x] = array[x, y];
        }

        return rowValues;
    }

    // -------------------------------------------------------------------------
    // Old Functions

    public ChessPiece.Color getActiveColor() {
        ChessPiece.Color activeColor;

        string colorString = FENString.Split(' ')[1];

        if (colorString.ToLower() == "w") {
            activeColor = ChessPiece.Color.White;
        } 
        else {
            activeColor = ChessPiece.Color.Black;
        }

        return activeColor;
    }


    public void iterateActiveColor(ChessPiece.Color oldColor) {

        string[] FENStrings = FENString.Split(" ");

        if (oldColor == ChessPiece.Color.White) {
            FENString = FENStrings[1] = "b";
        }
        else {
            FENString = FENStrings[1] = "w";
        }

        FENString = string.Join(" ", FENStrings);
    }


    public void logPieceEnPassing(bool enPassing, Vector2Int movePos) {
        string[] FENStrings = FENString.Split(" ");

        if (enPassing) {
            char row = rowCord[movePos.y];
            char col = colCord[movePos.x];

            FENStrings[3] = "" + col + row;
        }
        else {
            FENStrings[3] = "-";
        }

        FENString = string.Join(" ", FENStrings);
    }


    public void checkRooksAndKings()
    {
        string[] FENs = FENString.Split(" ");
        string blackCastle = FENs[0].Split("/")[0];
        string whiteCastle = FENs[0].Split("/")[7];
        int[] cols = {0, 4, 7};

        // Black Castle Check
        string ctle = "";
        foreach (int col in cols) {
            ctle += blackCastle[col];
        }

        if (ctle.Substring(0, 2) != "rk") {
            FENs[2] = FENs[2].Replace("q", "");
        }
        if (ctle.Substring(1, 2) != "kr") {
            FENs[2] = FENs[2].Replace("k", "");
        }

        // White Castle Check
        ctle = "";
        foreach (int col in cols) {
            ctle += whiteCastle[col];
        }

        if (ctle.Substring(0, 2) != "RK") {
            FENs[2] = FENs[2].Replace("Q", "");
        }
        if (ctle.Substring(1, 2) != "KR") {
            FENs[2] = FENs[2].Replace("K", "");
        }
        
        if (FENs[2] == "") {
            FENs[2] = "-";
        }

        FENString = string.Join(" ", FENs);
    }


    //--------------------------------------------------------------------------
    // Debug Methods

    private void printGrid(int[,] grid) {
        string[] boardString = new string[8];

        for (int y = 0; y < grid.GetLength(0); y++) {
            for (int x = 0; x < grid.GetLength(1); x++) {
                if (grid[x, y] == 0) {
                    boardString[y] += " ";
                }

                boardString[y] += grid[x, y].ToString() + ",";
            }

            boardString[y] += "\n";
        }

        string printString = string.Join("", boardString);

        print(printString);
    }

}
