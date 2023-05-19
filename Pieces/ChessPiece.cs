using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public MainGameDriver mainGameDriver;
    public SpriteRenderer pieceSprite;
    public Type type;
    public Color color;
    public Vector2Int pos;
    public int value;
    public static string pieceChars = "kpnbrq";

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
        int pieceValue = (int) ChessPiece.Type.Null +  
                         (int) ChessPiece.Color.Null;

        char pieceCharLower = char.ToLower(pieceChar);

        for (int typeIndex = 0; typeIndex < pieceChars.Length; typeIndex++)
        {
            if (pieceCharLower == pieceChars[typeIndex])
            {
                pieceValue = 1 + typeIndex;

                if (char.IsUpper(pieceChar))
                {
                    pieceValue += (int) ChessPiece.Color.White;
                }
                else
                {
                    pieceValue += (int) ChessPiece.Color.Black;
                }

                break;
            }
        }

        return pieceValue;
    }


    //-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-

    private bool isSelected = false;

    void Start()
    {
        mainGameDriver = GameObject.FindGameObjectWithTag("Game Control").
                         GetComponent<MainGameDriver>();

        pieceSprite = this.GetComponent<SpriteRenderer>();

        if (mainGameDriver == null)
        {
            Debug.LogError("Main Game Driver couldn't be found. Check for" + 
                           "updated tage");
        }
    }

    void FixedUpdate()
    {
        if (isSelected)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
                                        Input.mousePosition);
            mousePosition.z = 0;
            this.transform.position = mousePosition;
        }
    }

    void OnMouseDown()
    {
        if (!mainGameDriver.aPieceIsSelected)
        {
            print((type, color));

            pieceSprite.sortingOrder = mainGameDriver.selectedSpriteLayer;

            isSelected = true;
            mainGameDriver.aPieceIsSelected = true;
        }

        else if (mainGameDriver.aPieceIsSelected && isSelected)
        {
            print("Setting the piece down.");

            pieceSprite.sortingOrder = mainGameDriver.normalSpriteLayer;

            isSelected = false;
            mainGameDriver.aPieceIsSelected = false;

            int gridX =  getIndexOfCloseValue(this.transform.position.x, 
                                          mainGameDriver.piecePositions);
            int gridY =  getIndexOfCloseValue(this.transform.position.y, 
                                          mainGameDriver.piecePositions);

            snapPieceToGrid(gridX, gridY);
        }
    }

    public void snapPieceToGrid(int posXIndex, int posYIndex)
    {
        Vector3 currentPosition = this.transform.position;

        currentPosition.x = mainGameDriver.piecePositions[posXIndex];
        currentPosition.y = mainGameDriver.piecePositions[posYIndex];

        this.transform.position = currentPosition;
    }

    public int getIndexOfCloseValue(float pos, List<int> posArray)
    {
        int closeIndex = 0;

        for (int i = 1; i < posArray.Count; i++)
        {   
            if (Mathf.Abs(pos - posArray[i]) < Mathf.Abs(pos - posArray[closeIndex]))
            {
                closeIndex = i;
            }
        }

        return closeIndex;
    }
}
