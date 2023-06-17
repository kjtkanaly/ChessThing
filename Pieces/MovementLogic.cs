using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementLogic
{

public static List<Vector2Int> gatherPossibleMoves(ChessPiece chessPiece)
{   
    List<Vector2Int> possibleMoves = new List<Vector2Int>();
    
    // Add the default move of placing the piece back down
    possibleMoves.Add(new Vector2Int(chessPiece.pos.x, chessPiece.pos.y));

    if (chessPiece.type == ChessPiece.Type.Pawn)
    {
        possibleMoves.AddRange(Pawn.getPossiblePawnMoves(chessPiece));
    }

    else if (chessPiece.type == ChessPiece.Type.Knight)
    {
        possibleMoves.AddRange(Knight.getPossibleKnightMoves(chessPiece));
    }

    else if (chessPiece.type == ChessPiece.Type.King)
    {
        possibleMoves.AddRange(getKingMoves(chessPiece));

        // Check for castling
        if (chessPiece.movementCount == 0)
        {
            if (checkIfKingCanCastleLeft(chessPiece))
            {
                possibleMoves.Add(new Vector2Int(chessPiece.pos.x - 2, 
                                                 chessPiece.pos.y
                                                 ));
            }

            if (checkIfKingCanCastleRight(chessPiece))
            {
                possibleMoves.Add(new Vector2Int(chessPiece.pos.x + 2, 
                                                 chessPiece.pos.y
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

public static List<Vector2Int> getKingMoves(ChessPiece chessPiece)
{
    List<Vector2Int> kingMoves = new List<Vector2Int>();

    for (int row = chessPiece.pos.y - 1; row <= chessPiece.pos.y + 1; row++)
    {
        for (int col = chessPiece.pos.x - 1; col <= chessPiece.pos.x + 1; col++)
        {
            bool currentSpot = (row == chessPiece.pos.y) & (col == chessPiece.pos.x);
            bool outOfBounds = (row < 8) & (col < 8) & (row > -1) & (col > -1);

            if (!currentSpot & outOfBounds)
            {
                if (!chessPiece.checkIfAllyPiece(new Vector2Int(col, row)))
                {
                    kingMoves.Add(new Vector2Int(col, row));
                }
            }
        }
    }

    return kingMoves;
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

}
