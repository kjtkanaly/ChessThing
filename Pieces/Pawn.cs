using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pawn
{
    public static List<Vector2Int> getPossiblePawnMoves(ChessPiece pawnPiece)
    {
        List<Vector2Int> pawnMoves = new List<Vector2Int>();
        int row = pawnPiece.pos.y;

        int rowIncrement = 1;
        if (pawnPiece.color == ChessPiece.Color.Black)
        {
            rowIncrement = -1;
        }

        int maxPawnSteps = 1;

        if (pawnHasNotMoved(pawnPiece))
        {
            maxPawnSteps = 2;
        }

        int[,] miniGameBoard = pawnPiece.mainGameDriver.miniGameBoard;

        // Finding forward movement options
        for(int i = 0; i < maxPawnSteps; i++)
        {
            row += rowIncrement;

            if ((row < 0) || (row > 7) || pawnIsBlocked(pawnPiece, 
                                                        miniGameBoard, 
                                                        row))
            {
                break;
            }

            pawnMoves.Add(new Vector2Int(pawnPiece.pos.x, row));
        }

        // Finding possible kill options
        row = pawnPiece.pos.y;
        pawnMoves.AddRange(getPawnKillMoves(pawnPiece, 
                                            miniGameBoard, 
                                            row + rowIncrement));

        return pawnMoves;
    }

    public static bool pawnHasNotMoved(ChessPiece pawnPiece)
    {
        bool check = false;

        if ((pawnPiece.color == ChessPiece.Color.White) && (pawnPiece.pos.y == 1))
        {
            check = true;
        }

        if ((pawnPiece.color == ChessPiece.Color.Black) && (pawnPiece.pos.y == 6))
        {   
            check = true;
        }

        return check;
    }

    public static bool pawnIsBlocked(ChessPiece pawnPiece, 
                                     int[,] miniGameBoard, 
                                     int row)
    {
        // Exception Check, pawn is add board's edge
        if ((row < 0) || (row > 7))
        {
            return false;
        }

        bool check = false;

        if(miniGameBoard[pawnPiece.pos.x, row] != 0)
        {
            check = true;
        }

        return check;
    }

    public static List<Vector2Int> getPawnKillMoves(ChessPiece pawnPiece, 
                                                    int[,] miniGameBoard, 
                                                    int row)
    {
        List<Vector2Int> killMoves = new List<Vector2Int>();

        // Exception Check, pawn is add board's edge
        if ((row < 0) || (row > 7))
        {
            return killMoves;
        }

        // Check for kill left
        if (pawnPiece.pos.x > 0)
        {
            int enemyValue = miniGameBoard[pawnPiece.pos.x - 1, row];
            if (pawnPiece.pieceCanBeKilled(enemyValue))
            {
                killMoves.Add(new Vector2Int(pawnPiece.pos.x - 1, row));
            }
        }

        // Check for kill right
        if (pawnPiece.pos.x < 7)
        {
            int enemyValue = miniGameBoard[pawnPiece.pos.x + 1, row];
            if (pawnPiece.pieceCanBeKilled(enemyValue))
            {
                killMoves.Add(new Vector2Int(pawnPiece.pos.x + 1, row));
            }
        }

        return killMoves;
    }
}
