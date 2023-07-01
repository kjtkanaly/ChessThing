using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    MainGameDriver mainGameDriver;

    public List<Vector2Int> getMoves(Vector2Int piecePos, 
                                     ChessPiece.Color allyColor,
                                     int moveCount)
    {
        getMoves(piecePos, 
                 allyColor,
                 mainGameDriver.miniGameBoard,
                 moveCount
                 );
    }

    public List<Vector2Int> getMoves(Vector2Int piecePos, 
                                     ChessPiece.Color allyColor,
                                     int[,] miniGameBoard,
                                     int moveCount)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        for (int row = piecePos.y - 1; row <= piecePos.y + 1; row++)
        {
            for (int col = piecePos.x - 1; col <= piecePos.x + 1; col++)
            {
                bool currentSpot = (row == piecePos.y) & (col == piecePos.x);
                bool inBounds = (row < 8) & (col < 8) 
                                   & (row > -1) & (col > -1);

                if (!currentSpot & inBounds)
                {
                    ChessPiece.Color posColor = 
                        ChessPiece.getPieceColorFromInt(miniGameBoard[col, row]);

                    if (posColor != allyColor)
                    {
                        moves.Add(new Vector2Int(col, row));
                    }
                }
            }
        }

        // Check for castling
        if (moveCount == 0)
        {
            if (checkIfKingCanCastleLeft(chessPiece))
            {
                chessPiece.canCastleLeft = true;
                possibleMoves.Add(new Vector2Int(piecePos.x - 2, 
                                                    piecePos.y
                                                    ));
            }

            if (checkIfKingCanCastleRight(chessPiece))
            {
                chessPiece.canCastleRight = true;
                possibleMoves.Add(new Vector2Int(piecePos.x + 2, 
                                                    piecePos.y
                                                    ));
            }
        }

        return moves;
    }

}