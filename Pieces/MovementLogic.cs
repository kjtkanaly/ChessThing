using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementLogic
{

public static List<Vector2Int> gatherPossibleMoves(ChessPiece chessPiece)
{
    gatherPossibleMoves(chessPiece.type, 
                        chessPiece.color, 
                        chessPiece.pos,
                        mainGameDriver.miniGameBoard);
}

public static List<Vector2Int> gatherPossibleMoves(ChessPiece.Type pieceType,
                                                   ChessPiece.Color allyColor,
                                                   Vector2Int piecePos,
                                                   int[,] miniGameBoard,
                                                   int moveCount)
{   
    List<Vector2Int> possibleMoves = new List<Vector2Int>();
    
    // Add the default move of placing the piece back down
    possibleMoves.Add(new Vector2Int(piecePos.x, piecePos.y));

    if (pieceType == ChessPiece.Type.Pawn)
    {
        possibleMoves.AddRange(Pawn.getPossiblePawnMoves(chessPiece));
    }

    else if (pieceType == ChessPiece.Type.Knight)
    {
        possibleMoves.AddRange(Knight.getPossibleKnightMoves(chessPiece));
    }

    else if (pieceType == ChessPiece.Type.King)
    {
        possibleMoves.AddRange(getKingMoves(chessPiece));

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
    }

    else
    {
        if (canMoveAtAngle(chessPiece))
        {
            possibleMoves.AddRange(getAngledMoves(chessPiece));
        }

        if (canMoveCartesianly(chessPiece))
        {
            possibleMoves.AddRange(getCartesianMoves(chessPiece));
        }
    }

    return possibleMoves;
}

public static bool canMoveAtAngle(ChessPiece chessPiece)
{
    ChessPiece.Type[] possibleTypes = {ChessPiece.Type.Bishop, 
                                        ChessPiece.Type.Queen};

    for (int i = 0; i < possibleTypes.Length; i++)
    {
        if (chessPiece.type == possibleTypes[i])
        {
            return true;
        }
    }

    return false;
}

public static bool canMoveCartesianly(ChessPiece chessPiece)
{
    ChessPiece.Type[] possibleTypes = {ChessPiece.Type.Rook, 
                                        ChessPiece.Type.Queen};

    for (int i = 0; i < possibleTypes.Length; i++)
    {
        if (chessPiece.type == possibleTypes[i])
        {
            return true;
        }
    }

    return false;
}

public static bool checkIfOffBoard(Vector2Int cords)
{
    if ((cords.x < 0) || (cords.x > 7))
    {
        return true;
    }

    if ((cords.y < 0) || (cords.y > 7))
    {
        return true; 
    }  

    return false;
}

public static List<Vector2Int> getAngledMoves(ChessPiece chessPiece)
{
    List<Vector2Int> angledMoves = new List<Vector2Int>();

    for (int rowIncrement = -1; rowIncrement <= 1; rowIncrement += 2)
    {
        for (int colIncrement = -1; colIncrement <= 1; colIncrement += 2)
        {
            Vector2Int slope = new Vector2Int(colIncrement, rowIncrement);

            angledMoves.AddRange(getLongMoves(chessPiece, slope));
        }
    }

    return angledMoves;
}

public static List<Vector2Int> getCartesianMoves(ChessPiece chessPiece)
{
    List<Vector2Int> cartesianMoves = new List<Vector2Int>();

    for (int increment = -1; increment <= 1; increment += 2)
    {
        Vector2Int slope = new Vector2Int(0, increment);
        cartesianMoves.AddRange(getLongMoves(chessPiece, slope));

        slope = new Vector2Int(increment, 0);
        cartesianMoves.AddRange(getLongMoves(chessPiece, slope));
    }

    return cartesianMoves;
}

public static List<Vector2Int> getLongMoves(ChessPiece chessPiece, 
                                            Vector2Int slope
                                            )
{
    List<Vector2Int> longMoves = new List<Vector2Int>();

    int row = chessPiece.pos.y + slope.y;
    int col = chessPiece.pos.x + slope.x;

    while ((row < 8) & (col < 8) & (row > -1) & (col > -1))
    {
        if (chessPiece.mainGameDriver.miniGameBoard[col, row] > 0)
        {
            if (!chessPiece.checkIfAllyPiece(new Vector2Int(col, row)))
            {
                longMoves.Add(new Vector2Int(col, row));
            }

            break;
        }

        longMoves.Add(new Vector2Int(col, row));

        row += slope.y;
        col += slope.x;
    }

    return longMoves;
}


public static List<Vector2Int> getKingMoves(
    Vector2Int piecePos,
    ChessPiece.Color allyColor,
    int[,] miniGameBoard)
{
    List<Vector2Int> kingMoves = new List<Vector2Int>();

    for (int row = piecePos.y - 1; row <= piecePos.y + 1; row++)
    {
        for (int col = piecePos.x - 1; col <= piecePos.x + 1; col++)
        {
            bool currentSpot = (row == piecePos.y) & (col == piecePos.x);
            bool outOfBounds = (row < 8) & (col < 8) & (row > -1) & (col > -1);

            if (!currentSpot & outOfBounds)
            {
                ChessPiece.Color posColor = 
                    ChessPiece.getPieceColorFromInt(miniGameBoard[col, row]);

                if (posColor != allyColor)
                {
                    kingMoves.Add(new Vector2Int(col, row));
                }
            }
        }
    }

    return kingMoves;
}


public static void removeInvalidMoves(List<Vector2Int> possibleMoves, 
                                      ChessPiece piece)
{
    MainGameDriver mainGameDriver = piece.mainGameDriver;
    ChessPiece kingPiece = mainGameDriver.getTeamKing(piece.color);
    int[,] miniGameBoard = mainGameDriver.miniGameBoard;

    Vector2Int originalPos = piece.pos;

    for (int i = possibleMoves.Count - 1; i >= 0; i--)
    {
        mainGameDriver.updateMiniBoard(possibleMoves[i].x, 
                                       possibleMoves[i].y, 
                                       piece.value);

        piece.pos = new Vector2Int(possibleMoves[i].x, possibleMoves[i].y);

        bool check = CheckingSystem.checkIfKingIsInCheck(kingPiece.pos, 
                                                         piece.color,
                                                         miniGameBoard);

        mainGameDriver.updateMiniBoard(possibleMoves[i].x, 
                                       possibleMoves[i].y, 
                                       0);

        if (check)
        {
            possibleMoves.RemoveAt(i);
        }
    }

    piece.pos = originalPos;
}


public static bool checkIfKingCanCastleRight(ChessPiece king)
{
    int[,] miniGameBoard = king.mainGameDriver.miniGameBoard;
    CheckingSystem checkingSystem = king.checkingSystem;
    int row = king.pos.y;

    // Check if king is currently in check
    if (checkingSystem.getTeamCheckStatus(king.color))
    {
        return false;
    }

    for (int col = king.pos.x + 1; col <= 6; col++)
    {
        bool spaceCheck = checkingSystem.checkIfKingIsInCheck(
            new Vector2Int(col, row),
            king.color
            );

        if ((miniGameBoard[col, row] != 0) || spaceCheck)
        {
            return false;
        }
    }

    // Check if the rook is avaliable to castle
    ChessPiece rook = king.mainGameDriver.getPieceAtPos(
        new Vector2Int(7, row)
        );

    if (rook == null)
    {
        return false;
    }

    if ((rook.movementCount > 0) || (rook.type != ChessPiece.Type.Rook))
    {
        return false;
    }

    return true;
}


public static bool checkIfKingCanCastleLeft(ChessPiece king)
{
    int[,] miniGameBoard = king.mainGameDriver.miniGameBoard;
    CheckingSystem checkingSystem = king.checkingSystem;
    int row = king.pos.y;

    // Check if king is currently in check
    if (checkingSystem.getTeamCheckStatus(king.color))
    {
        return false;
    }

    for (int col = king.pos.x - 1; col > 1; col--)
    {
        bool spaceCheck = checkingSystem.checkIfKingIsInCheck(
            new Vector2Int(col, row),
            king.color
            );

        if ((miniGameBoard[col, row] != 0) || spaceCheck)
        {
            return false;
        }
    }


    // Check if the rook is avaliable to castle
    ChessPiece rook = king.mainGameDriver.getPieceAtPos(
        new Vector2Int(0, row)
        );

    if (rook == null)
    {
        return false;
    }

    if (rook.movementCount > 0 || rook.type != ChessPiece.Type.Rook)
    {
        return false;
    }

    return true;
}


public static void castleTheRook(ChessPiece rook, Vector2Int newPos)
{
    rook.mainGameDriver.updateMiniBoard(rook.pos.x, rook.pos.y, 0);

    // Move the rook
    rook.snapPieceToGrid(newPos.x, newPos.y);

    rook.mainGameDriver.updateMiniBoard(newPos.x, newPos.y, rook.value);
}


}
