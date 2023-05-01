using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public Type type;
    public Color color;
    public Vector2Int pos;
    public int value;

    public void InitChessPiece(Type typeValue, 
                               Color colorValue, 
                               Vector2Int posValue)
    {
        this.type = typeValue;
        this.color = colorValue;
        this.pos = posValue;

        this.value = (int) type + (int) color;
    }
 
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


    public static ChessPiece.Color getPieceColorFromInt(int pieceColorValue)
    {
        ChessPiece.Color pieceColor = ChessPiece.Color.White;

        if ((pieceColorValue - 8) > 6)
        {
            pieceColor = ChessPiece.Color.Black;
        }

        return pieceColor;
    }


    public static ChessPiece.Type getPieceTypeFromInt(int pieceTypeValue)
    {
        while (pieceTypeValue > 6)
        {
            pieceTypeValue -= 8;
        }
        
        return (ChessPiece.Type)pieceTypeValue;
    }


    public static void setPieceSprite(GameObject piece, ChessPiece pieceInfo)
    {
        MainGameDriver GameDrive = 
            GameObject.FindGameObjectWithTag("Game Control").
            GetComponent<MainGameDriver>();

        SpriteRenderer pieceSR = piece.GetComponent<SpriteRenderer>();
        
        if (pieceInfo.color == ChessPiece.Color.White)
        {
            pieceSR.sprite = GameDrive.WhiteSpriteList[(int)pieceInfo.type - 1];
        }

        else
        {
            pieceSR.sprite = GameDrive.BlackSpriteList[(int)pieceInfo.type - 1];
        }
    }
    

    public static int getPieceValue(char pieceChar)
    {
        int piece = (int) ChessPiece.Type.Null +  
                    (int) ChessPiece.Color.Null;

        switch (pieceChar)
        {
            case 'p':
                piece = (int) ChessPiece.Type.Pawn +
                        (int) ChessPiece.Color.Black;
                break;
            case 'n':
                piece = (int) ChessPiece.Type.Knight +  
                        (int) ChessPiece.Color.Black;
                break;
            case 'b':
                piece = (int) ChessPiece.Type.Bishop +
                        (int) ChessPiece.Color.Black;
                break;
            case 'r':
                piece = (int) ChessPiece.Type.Rook +
                        (int) ChessPiece.Color.Black;
                break;
            case 'q':
                piece = (int) ChessPiece.Type.Queen +
                        (int) ChessPiece.Color.Black;
                break;
            case 'k':
                piece = (int) ChessPiece.Type.King + 
                        (int) ChessPiece.Color.Black;
                break;

            case 'P':
                piece = (int) ChessPiece.Type.Pawn +
                        (int) ChessPiece.Color.White;
                break;
            case 'N':
                piece = (int) ChessPiece.Type.Knight +  
                        (int) ChessPiece.Color.White;
                break;
            case 'B':
                piece = (int) ChessPiece.Type.Bishop +
                        (int) ChessPiece.Color.White;
                break;
            case 'R':
                piece = (int) ChessPiece.Type.Rook +
                        (int) ChessPiece.Color.White;
                break;
            case 'Q':
                piece = (int) ChessPiece.Type.Queen +
                        (int) ChessPiece.Color.White;
                break;
            case 'K':
                piece = (int) ChessPiece.Type.King + 
                        (int) ChessPiece.Color.White;
                break;
            
            default:
                piece = -1;
                break;
        }

        return piece;

    }
}
