using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceMaster : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Variables

    // Objects
    public ChessPiece[] chessPieceArray;
    [SerializeField] private ChessBoard CB;
    [SerializeField] private MainGameDriver MGD;
    [SerializeField] private FEN Fen;
    [SerializeField] private GameObject chessPiecePreFab;
    [SerializeField] private Sprite[] WhiteSpriteList, BlackSpriteList;

    // Unity Types

    // Types

    // -------------------------------------------------------------------------
    // Game Events
    void Start() {
        CB = this.GetComponent<ChessBoard>();
    }

    // -------------------------------------------------------------------------
    // Piece Related Methods
    public void createChessObjects(int objectCount) {
        chessPieceArray = new ChessPiece[objectCount];

        for (int i = 0; i < objectCount; i++) {
            GameObject newPiece = Instantiate(chessPiecePreFab, 
                                              new Vector3Int(0, 0, 0), 
                                              Quaternion.identity);

            newPiece.SetActive(false);

            chessPieceArray[i] = newPiece.GetComponent<ChessPiece>();
        }
    }

    public void updateChessObjectsInfo(int[,] grid) {
        int pieceIndex = 0;

        for (int y = 0; y < grid.GetLength(0); y++) {
            for (int x = 0; x < grid.GetLength(1); x++) {

                if (grid[x, y] > 0) {   
                    updateChessObjectInfo(chessPieceArray[pieceIndex], 
                                          grid[x, y], 
                                          new Vector2Int(x, y)
                                          );

                    pieceIndex += 1;
                }
            }
        }
    }

    public void updateChessObjectInfo(ChessPiece piece, int val, Vector2Int pos) {
        // Assign the object the appropriate piece info
        piece.InitChessPiece(val, pos, MGD);

        // Move the object to the texture position
        Vector3 worldPos = new Vector3(CB.worldXPos[pos.x],
                                       CB.worldYPos[pos.y],
                                       0);
        piece.gameObject.transform.position = worldPos;

        // Set the piece's appropriate sprite
        setPieceSprite(piece);

        // Finally, activate the piece's gameobject
        piece.gameObject.SetActive(true);
    }

    public void setPieceSprite(ChessPiece piece) {
        SpriteRenderer pieceSR = piece.GetComponent<SpriteRenderer>();
        
        if (piece.color == ChessPiece.Color.White) {
            pieceSR.sprite = WhiteSpriteList[(int)piece.type - 1];
        }
        else {
            pieceSR.sprite = BlackSpriteList[(int)piece.type - 1];
        }
    }

    public void deactivateThePieceAtPos(Vector2Int pos)
    {
        for (int i = 0; i < chessPieceArray.GetLength(0); i++)
        {
            Vector2Int piecePos = chessPieceArray[i].pos;

            if ((piecePos.x == pos.x) && (piecePos.y == pos.y))
            {
                chessPieceArray[i].gameObject.SetActive(false);
                chessPieceArray[i].pos = new Vector2Int(-1, -1);
            }
        }
    }

    //--------------------------------------------------------------------------
    // Debug Methods
}
