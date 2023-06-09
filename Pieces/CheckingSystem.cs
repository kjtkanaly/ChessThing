using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckingSystem : MonoBehaviour
{
    private MainGameDriver mainGameDriver;
    public bool whiteKingInCheck = false;
    public bool blackKingInCheck = false;

    void Start()
    {
        mainGameDriver = this.gameObject.GetComponent<MainGameDriver>();
    }

    // Parent fx that will check if a given team's king is in check
    public void checkIfKingIsInCheck(ChessPiece.Color teamColor)
    {
        // Get the team's king
        ChessPiece king = mainGameDriver.getTeamKing(teamColor);

        // Init a list to hold enemy pieces that checked the king
        List<ChessPiece> checkingPieces = new List<ChessPiece>();

        // Check if the king is in angular danger
        checkingPieces.AddRange(findAngularChecks(king));

        // Check if the king is in cartisianal danger
        checkingPieces.AddRange(findCartesianChecks(king));

        // Check if the king is in knightly danger
        checkingPieces.AddRange(findKnightlyChecks(king));

        // Check if the king is in pawn danger

        // Update the appropriate king check status
        updateKingCheckStatus(teamColor, checkingPieces.Count);
    }

    public List<ChessPiece> getCheckingPieces(
        List<Vector2Int> positions,
        ChessPiece.Color kingColor,
        ChessPiece.Type[] types)
    {
        List<ChessPiece> piecesCommittingCheck = new List<ChessPiece>();

        // Check for opposing knights
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

    public void updateKingCheckStatus(
        ChessPiece.Color teamColor,
        int checkPieceCount
        )
    {
        bool updatedValue = false;

        if (checkPieceCount > 0)
        {
            updatedValue = true;
        }

        if (teamColor == ChessPiece.Color.White)
        {
            whiteKingInCheck = updatedValue;
        }
        else
        {
            blackKingInCheck = updatedValue;
        }
    }
}
