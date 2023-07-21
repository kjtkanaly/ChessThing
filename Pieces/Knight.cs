using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Knight
{
    /*
public static List<Vector2Int> getPossibleKnightMoves(ChessPiece knightPiece)
{
    List<Vector2Int> knightMoves = new List<Vector2Int>();

    int row = knightPiece.pos.y;
    int col = knightPiece.pos.x;
    int[] possibleIndexOffsets = {-2, -1, 1, 2};

    for (int i = 0; i < possibleIndexOffsets.Length; i++)
    {
        for (int j = 0; j < possibleIndexOffsets.Length; j++)
        {
            if (Mathf.Abs(possibleIndexOffsets[i]) != 
                Mathf.Abs(possibleIndexOffsets[j]))
            {
                Vector2Int moveCords = new Vector2Int(
                                                col + possibleIndexOffsets[i], 
                                                row + possibleIndexOffsets[j]);

                // Check if Move is off board
                bool offBoardCheck = MovementLogic.checkIfOffBoard(moveCords);

                if (!offBoardCheck)
                {
                    // Check if Move is on ally pieces
                    bool allyPieceCheck = knightPiece.checkIfAllyPiece(
                                                moveCords);

                    if (!allyPieceCheck)
                    {
                        knightMoves.Add(moveCords);
                    }
                }
            }
        }
    }

    return knightMoves;
}
    /**/
}
