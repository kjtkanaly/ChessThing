using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckingSystem : MonoBehaviour
{
    public bool whiteKingInCheck = false;
    public bool blackKingInCheck = false;
    
    // Parent fx that will check if a given team's king is in check
    public static bool checkIfKingIsInCheck(Vector2Int piecePos, 
                                     ChessPiece.Color allyColor,
                                     int[,] gameMiniBoard)
    {
        // Init a list to hold enemy pieces that checked the king
        List<Vector2Int> checkingPieces = new List<Vector2Int>();

        // Check if the king is in angular danger
        checkingPieces.AddRange(findAngularChecks(piecePos, allyColor));

        // Check if the king is in cartisianal danger
        checkingPieces.AddRange(findCartesianChecks(piecePos, allyColor));

        // Check if the king is in knightly danger
        checkingPieces.AddRange(
            findKnightlyChecks(piecePos, allyColor, gameMiniBoard)
            );

        // Check if the king is in pawnly danger
        checkingPieces.AddRange(findPawnlyChecks(piecePos, allyColor, gameMiniBoard));

        // Check if the king is in kingly danger
        checkingPieces.AddRange(findKinglyChecks(piecePos, allyColor));

        if (checkingPieces.Count > 0)
        {
            return true;
        }

        return false;
    }

    public static List<Vector2Int> getCheckingPieces(
        List<Vector2Int> positions,
        ChessPiece.Color allyColor,
        ChessPiece.Type[] types,
        int[,] miniGameBoard)
    {
        List<Vector2Int> posCheckingTheKing = new List<Vector2Int>();

        for (int i = 0; i < positions.Count; i++)
        {
            int row = positions[i].y;
            int col = positions[i].x;
            int posValue = miniGameBoard[col, row];

            if (posValue == 0)
            {
                continue;
            }

            ChessPiece.Color posPieceColor = 
                ChessPiece.getPieceColorFromInt(posValue);
            ChessPiece.Type posPieceType = 
                ChessPiece.getPieceTypeFromInt(posValue);

            bool colorCheck = posPieceColor != allyColor;
            bool typeCheck = false;

            for (int j = 0; j < types.Length; j++)
            {
                if (posPieceType == types[j])
                {
                    typeCheck = true;
                    break;
                }
            }

            if (colorCheck && typeCheck)
            {
                posCheckingTheKing.Add(positions[i]);
            }
        }
        
        return posCheckingTheKing;
    }

    public static List<Vector2Int> findPawnlyChecks(Vector2Int piecePos, 
                                                    ChessPiece.Color allyColor,
                                                    int[,] miniGameBoard)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        int increment = 1;
        if (allyColor == ChessPiece.Color.Black)
        {
            increment = -1;
        }

        int rowIncrement = piecePos.y + increment;
        int[] colIncrements = {piecePos.x - 1, piecePos.x + 1};

        // Default
        if (!(rowIncrement >= 0 && rowIncrement <= 7))
        {
            return new List<Vector2Int>();
        }

        if (colIncrements[0] >= 0)
        {
            positions.Add(new Vector2Int(colIncrements[0], rowIncrement));
        }

        if (colIncrements[0] <= 7)
        {
            positions.Add(new Vector2Int(colIncrements[1], rowIncrement));
        }

        ChessPiece.Type[] typeCheck = {ChessPiece.Type.Pawn};
        return getCheckingPieces(positions, allyColor, typeCheck, miniGameBoard);
    }

    public static List<Vector2Int> findKinglyChecks(Vector2Int piecePos, 
                                             ChessPiece.Color allyColor)
    {
        List<Vector2Int> positions = MovementLogic.getKingMoves(piecePos, 
                                                                allyColor);

        ChessPiece.Type[] typeCheck = {ChessPiece.Type.King};

        return getCheckingPieces(positions, allyColor, typeCheck);
    }

    public static List<Vector2Int> findKnightlyChecks(Vector2Int piecePos, 
                                               ChessPiece.Color allyColor,
                                               int[,] gameMiniBoard)
    {
        List<Vector2Int> positions = Knight.getPossibleKnightMoves(piecePos, 
                                                                   allyColor,
                                                                   gameMiniBoard);
        ChessPiece.Type[] typeCheck = {ChessPiece.Type.Knight};

        return getCheckingPieces(positions, allyColor, typeCheck);
    }

    public static List<Vector2Int> findCartesianChecks(Vector2Int piecePos, 
                                                ChessPiece.Color allyColor)
    {
        List<Vector2Int> positions = MovementLogic.getCartesianMoves(piecePos, pieceColor);
        ChessPiece.Type[] typeCheck = {ChessPiece.Type.Queen, 
                                        ChessPiece.Type.Rook
                                        };

        return getCheckingPieces(positions, allyColor, typeCheck);
    }

    // Child fx that will check if a given king is in check from the angle
    public static List<Vector2Int> findAngularChecks(Vector2Int piecePos, 
                                              ChessPiece.Color pieceColor)
    {
        List<Vector2Int> positions = MovementLogic.getAngledMoves(piecePos, pieceColor);
        ChessPiece.Type[] typeCheck = {ChessPiece.Type.Queen, 
                                        ChessPiece.Type.Bishop
                                        };

        return getCheckingPieces(positions, allyColor, typeCheck);
    }

    public static void updateKingCheckStatus(ChessPiece.Color allyColor, bool check)
    {
        if (allyColor == ChessPiece.Color.White)
        {
            whiteKingInCheck = check;
        }
        else
        {
            blackKingInCheck = check;
        }
    }

    public static bool getTeamCheckStatus(ChessPiece.Color allyColor)
    {
        if (allyColor == ChessPiece.Color.White)
        {
            return whiteKingInCheck;
        }
        else
        {
            return blackKingInCheck;
        }
    }
}
