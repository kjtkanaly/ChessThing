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
    private const string pieceChars = "kpnbrq";

    // -------------------------------------------------------------------------
    // Chess Piece Enums

    public enum Type
    {
        Null = 0,
        King = 1,
        Pawn = 2,
        Knight = 3,
        Bishop = 4,
        Rook = 5,
        Queen = 6
    }

    public enum Color
    {
        Null = 0,
        White = 8,
        Black = 16
    }

    // -------------------------------------------------------------------------
    // Game Events
    void Start() {
        CB = this.GetComponent<ChessBoard>();
    }

    // -------------------------------------------------------------------------
    // Master Piece Related Methods
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
                    activateAllPieces(chessPieceArray[pieceIndex]);

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
    }

    public void activateAllPieces(ChessPiece piece) {
        // Finally, activate the piece's gameobject
        piece.gameObject.SetActive(true);
    }

    public void setPieceSprite(ChessPiece piece) {
        SpriteRenderer pieceSR = piece.GetComponent<SpriteRenderer>();
        
        if (piece.color == Color.White) {
            pieceSR.sprite = WhiteSpriteList[(int)piece.type - 1];
        }
        else {
            pieceSR.sprite = BlackSpriteList[(int)piece.type - 1];
        }
    }

    public void deactivateThePieceAtPos(Vector2Int pos) {
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

    public int getPieceValue(char pieceChar) {
        int pieceValue = (int) Type.Null 
                          + (int) Color.Null;

        char pieceCharLower = char.ToLower(pieceChar);

        for (int typeIndex = 0; typeIndex < pieceChars.Length; typeIndex++)
        {
            if (pieceCharLower == pieceChars[typeIndex])
            {
                pieceValue = 1 + typeIndex;

                if (char.IsUpper(pieceChar))
                {
                    pieceValue += (int) Color.White;
                }
                else
                {
                    pieceValue += (int) Color.Black;
                }

                break;
            }
        }

        return pieceValue;
    }

    public Color getPieceColorFromInt(int pieceColorValue)
    {
        Color pieceColor = Color.White;

        if ((pieceColorValue - 8) > 6)
        {
            pieceColor = Color.Black;
        }

        return pieceColor;
    }

    public Type getPieceTypeFromInt(int pieceTypeValue)
    {
        while (pieceTypeValue > 6)
        {
            pieceTypeValue -= 8;
        }
        
        return (Type)pieceTypeValue;
    }

    public string getPieceLetterFromValue(int pieceVal) {
        // Defualt
        if (pieceVal == 0) {
            return "0";
        }

        Type pieceType = getPieceTypeFromInt(pieceVal);
        Color pieceColor = getPieceColorFromInt(pieceVal);
        string pieceLetter = "";

        string[] pieceLetters = {"K", "P", "N", "B", "R", "Q"};
        Type[] pieceTypes = {Type.King, Type.Pawn, Type.Knight, 
                            Type.Bishop, Type.Rook, Type.Queen};

        for (int i = 0; i < pieceTypes.Length; i++) {
            if (pieceType == pieceTypes[i]) {
                pieceLetter += pieceLetters[i];
                break;
            }
        }

        // Team Check
        if (pieceColor == Color.Black) {
            pieceLetter = pieceLetter.ToLower();
        }

        return pieceLetter;
    }

    public Color getEnemyColor(Color allyColor)
    {
        if (allyColor == Color.White) {
            return Color.Black;
        }
        else {
            return Color.White;
        }
    }

    //--------------------------------------------------------------------------
    // Debug Methods
}
