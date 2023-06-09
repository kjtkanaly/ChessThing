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

        // Check if the king is in knightly danger

        // Check if the king is in pawn danger

        // Update the appropriate king check status
        updateKingCheckStatus(teamColor, checkingPieces.Count);
    }

    // Child fx that will check if a given king is in check from the angle
    public List<ChessPiece> findAngularChecks(ChessPiece king)
    {
        List<Vector2Int> possibleMoves = MovementLogic.getAngledMoves(king);
        List<ChessPiece> piecesCommittingCheck = new List<ChessPiece>();

        // Check for opposing queens and bishops
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            int row = possibleMoves[i].y;
            int col = possibleMoves[i].x;

            int posValue = mainGameDriver.miniGameBoard[col, row];

            if (posValue > 0)
            {
                ChessPiece posPiece = mainGameDriver.getPieceAtPos(
                    possibleMoves[i]
                    );

                if ((posPiece.color != king.color) && 
                   ((posPiece.type == ChessPiece.Type.Queen) || 
                    (posPiece.type == ChessPiece.Type.Bishop)))
                {
                    print((posPiece.value, posPiece.pos.x, posPiece.pos.y));
                    piecesCommittingCheck.Add(posPiece);
                }
            }
        }

        return piecesCommittingCheck;
    }

    public void updateKingCheckStatus(
        ChessPiece.Color teamColor,
        int checkPieceCount
        )
    {
        if (checkPieceCount > 0)
        {
            if (teamColor == ChessPiece.Color.White)
            {
                whiteKingInCheck = true;
            }
            else
            {
                blackKingInCheck = true;
            }
        }
    }
}
