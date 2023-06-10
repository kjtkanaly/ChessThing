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

        if (pawnPiece.movementCount)
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

        // Check for enPassing
        if (enPassCheck(pawnPiece))
        {
            List<MainGameDriver.GameMoves> previousMoves = 
                pawnPiece.mainGameDriver.gameMoves;
            MainGameDriver.GameMoves lastMove = 
                previousMoves[previousMoves.Count - 1];

            pawnMoves.Add(new Vector2Int(lastMove.newPos.x, 
                                         lastMove.newPos.y + rowIncrement
                                         ));

            pawnPiece.enPassing = true;
            pawnPiece.enPassingPos = new Vector2Int(lastMove.newPos.x, 
                                                    lastMove.newPos.y
                                                    );
        }

        return pawnMoves;
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

    public static bool enPassCheck(ChessPiece pawnPiece)
    {
        MainGameDriver mainGameDriver = pawnPiece.mainGameDriver;
        List<MainGameDriver.GameMoves> previousMoves = mainGameDriver.gameMoves;

        if (previousMoves.Count == 0)
        {
            return false;
        }

        MainGameDriver.GameMoves lastMove = previousMoves[previousMoves.Count - 1];

        if (lastMove.piece.type == ChessPiece.Type.Pawn)
        {
            int enemyMovementDelta = Mathf.Abs(lastMove.newPos.y - 
                                          lastMove.previousPos.y
                                          );
            int colDelta = Mathf.Abs(pawnPiece.pos.x - lastMove.newPos.x);
            int rowDelta = Mathf.Abs(pawnPiece.pos.y - lastMove.newPos.y);

            if ((enemyMovementDelta == 2) && (colDelta == 1) && (rowDelta == 0))
            {
                return true;
            }
        }

        return false;
    }
}
