using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckingSystem : MonoBehaviour
{
    private MainGameDriver mainGameDriver;
    private GameObject classHolder;
    private ChessPiece king;
    public bool whiteKingInCheck = false;
    public bool blackKingInCheck = false;

    void Start()
    {
        mainGameDriver = this.gameObject.GetComponent<MainGameDriver>();

        classHolder = new GameObject();
        king = classHolder.AddComponent<ChessPiece>();
    }

    // Parent fx that will check if a given team's king is in check
    public bool checkIfKingIsInCheck(Vector2Int piecePos, 
                                     ChessPiece.Color teamColor)
    {
        // Get the team's king
        king.InitChessPiece(ChessPiece.Type.King, 
                            teamColor, 
                            piecePos);

        // Init a list to hold enemy pieces that checked the king
        List<ChessPiece> checkingPieces = new List<ChessPiece>();

        // Check if the king is in angular danger
        checkingPieces.AddRange(findAngularChecks(king));

        // Check if the king is in cartisianal danger
        checkingPieces.AddRange(findCartesianChecks(king));

        // Check if the king is in knightly danger
        checkingPieces.AddRange(findKnightlyChecks(king));

        // Check if the king is in pawnly danger
        checkingPieces.AddRange(findPawnlyChecks(king));

        // Check if the king is in kingly danger
        checkingPieces.AddRange(findKinglyChecks(king));

        if (checkingPieces.Count > 0)
        {
            return true;
        }

        return false;
    }

    public List<ChessPiece> getCheckingPieces(
        List<Vector2Int> positions,
        ChessPiece.Color kingColor,
        ChessPiece.Type[] types)
    {
        List<ChessPiece> piecesCommittingCheck = new List<ChessPiece>();

        for (int i = 0; i < positions.Count; i++)
        {
            int row = positions[i].y;
            int col = positions[i].x;
            int posValue = mainGameDriver.miniGameBoard[col, row];

            if (posValue == 0)
            {
                continue;
            }

            ChessPiece posPiece = mainGameDriver.getPieceAtPos(positions[i]);

            bool colorCheck = posPiece.color != kingColor;
            bool typeCheck = false;

            for (int j = 0; j < types.Length; j++)
            {
                if (posPiece.type == types[j])
                {
                    typeCheck = true;
                    break;
                }
            }

            if (colorCheck && typeCheck)
            {
                print((posPiece.type, posPiece.color, 
                       posPiece.pos.x, posPiece.pos.y));

                piecesCommittingCheck.Add(posPiece);
            }
        }
        
        return piecesCommittingCheck;
    }

    public List<ChessPiece> findPawnlyChecks(ChessPiece king)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        int increment = 1;
        if (king.color == ChessPiece.Color.Black)
        {
            increment = -1;
        }

        int rowIncrement = king.pos.y + increment;
        int[] colIncrements = {king.pos.x - 1, king.pos.x + 1};

        // Default
        if (!(rowIncrement >= 0 && rowIncrement <= 7))
        {
            return new List<ChessPiece>();
        }

        if (colIncrements[0] >= 0)
        {
            positions.Add(new Vector2Int(colIncrements[0], rowIncrement));
        }

        if (colIncrements[0] <= 7)
        {
            positions.Add(new Vector2Int(colIncrements[1], rowIncrement));
        }

        ChessPiece.Type[] checkTypes = {ChessPiece.Type.Pawn};
        return getCheckingPieces(positions, king.color, checkTypes);
    }

    public List<ChessPiece> findKinglyChecks(ChessPiece king)
    {
        List<Vector2Int> positions = MovementLogic.getKingMoves(king);
        ChessPiece.Type[] checkTypes = {ChessPiece.Type.King};

        return getCheckingPieces(positions, king.color, checkTypes);
    }

    public List<ChessPiece> findKnightlyChecks(ChessPiece king)
    {
        List<Vector2Int> positions = Knight.getPossibleKnightMoves(king);
        ChessPiece.Type[] checkTypes = {ChessPiece.Type.Knight};

        return getCheckingPieces(positions, king.color, checkTypes);
    }

    public List<ChessPiece> findCartesianChecks(ChessPiece king)
    {
        List<Vector2Int> positions = MovementLogic.getCartesianMoves(king);
        ChessPiece.Type[] checkTypes = {ChessPiece.Type.Queen, 
                                        ChessPiece.Type.Rook
                                        };

        return getCheckingPieces(positions, king.color, checkTypes);
    }

    // Child fx that will check if a given king is in check from the angle
    public List<ChessPiece> findAngularChecks(ChessPiece king)
    {
        List<Vector2Int> positions = MovementLogic.getAngledMoves(king);
        ChessPiece.Type[] checkTypes = {ChessPiece.Type.Queen, 
                                        ChessPiece.Type.Bishop
                                        };

        return getCheckingPieces(positions, king.color, checkTypes);
    }

    public void updateKingCheckStatus(ChessPiece.Color teamColor, bool check)
    {
        if (teamColor == ChessPiece.Color.White)
        {
            whiteKingInCheck = check;
        }
        else
        {
            blackKingInCheck = check;
        }
    }

    public bool getTeamCheckStatus(ChessPiece.Color teamColor)
    {
        if (teamColor == ChessPiece.Color.White)
        {
            return whiteKingInCheck;
        }
        else
        {
            return blackKingInCheck;
        }
    }
}
