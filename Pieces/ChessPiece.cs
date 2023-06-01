using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public MainGameDriver mainGameDriver;
    public ChessBoard chessBoard;
    public SpriteRenderer pieceSprite;
    public Type type;
    public Color color;
    public Vector2Int pos;
    public List<Vector2Int> possibleMoves;
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

        chessBoard = mainGameDriver.chessBoard;

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
        // Picking a piece up
        if (!mainGameDriver.aPieceIsSelected)
        {
            print((type, color));

            // Bring the pieces sprite level up
            pieceSprite.sortingOrder = mainGameDriver.selectedSpriteLayer;

            // Update control booleans
            isSelected = true;
            mainGameDriver.aPieceIsSelected = true;

            // Update the mini board
            mainGameDriver.updateMiniBoard(pos.x, pos.y, 0);

            // Find the possible moves for the piece
            gatherPossibleMoves();
        }

        // Setting a piece down
        else if (mainGameDriver.aPieceIsSelected && isSelected)
        {
            // Handle the placement of the piece
            int gridX =  getIndexOfCloseValue(this.transform.position.x, 
                                          mainGameDriver.piecePositions);
            int gridY =  getIndexOfCloseValue(this.transform.position.y, 
                                          mainGameDriver.piecePositions);

            // Check if the closes grid positions are valid moves
            bool validMove = false;
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                if (possibleMoves[i].x == gridX && possibleMoves[i].y == gridY)
                {
                    validMove = true;
                    break;
                }
            }
            
            if (validMove)
            {
                pieceSprite.sortingOrder = mainGameDriver.normalSpriteLayer;

                isSelected = false;
                mainGameDriver.aPieceIsSelected = false;

                // Destroy any piece on the new spot
                removeImpedingPiece(gridX, gridY);

                // Update the mini board
                mainGameDriver.updateMiniBoard(gridX, gridY, value);

                // Update this piece's pos
                pos = new Vector2Int(gridX, gridY);

                snapPieceToGrid(gridX, gridY);

                // Dehighlight the possible moves
                // Display those possible moves
                for (int i = 1; i < possibleMoves.Count; i++)
                {
                    int spaceIndex = possibleMoves[i].y * 8 + 
                                     possibleMoves[i].x;
                    ChessBoard.BoardSpace boardSpace = 
                        chessBoard.boardSpots[spaceIndex];

                    chessBoard.dehighlightBoardSpace(mainGameDriver.boardTexture, 
                                                     boardSpace);
                }

                // Debug - display updated mini board
                mainGameDriver.debugMiniBoard();
            }
            else
            {
                print("Invalid move!");
            }
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

    public void removeImpedingPiece(int col, int row)
    {
        if (mainGameDriver.miniGameBoard[col, row] > 0)
        {
            mainGameDriver.deactivateThePieceAtPos(col, row);
        }
    }

    public void gatherPossibleMoves()
    {   
        possibleMoves = new List<Vector2Int>();
        
        // Add the default move of placing the piece back down
        possibleMoves.Add(new Vector2Int(pos.x, pos.y));

        if (type == ChessPiece.Type.Pawn)
        {
            possibleMoves.AddRange(Pawn.getPossiblePawnMoves(this));
        }

        else if (type == ChessPiece.Type.Knight)
        {
            print("Can move like a knight!");

            possibleMoves.AddRange(Knight.getPossibleKnightMoves(this));
        }

        else if (type == ChessPiece.Type.King)
        {
            print("Can move like a king!");

            possibleMoves.AddRange(getKingMoves());
        }

        else
        {
            if (canMoveAtAngle())
            {
                print("Can move at an angle!");

                possibleMoves.AddRange(getAngledMoves());
            }

            if (canMoveCartesianly())
            {
                print("Can move Cartesianly!");

                possibleMoves.AddRange(getCartesianMoves());
            }
        }

        // Display those possible moves
        for (int i = 1; i < possibleMoves.Count; i++)
        {
            int spaceIndex = possibleMoves[i].y * 8 + possibleMoves[i].x;
            ChessBoard.BoardSpace boardSpace = chessBoard.boardSpots[spaceIndex];

            chessBoard.highlightBoardSpace(mainGameDriver.boardTexture, 
                                           boardSpace);
        }
    }

    public bool canMoveAtAngle()
    {
        ChessPiece.Type[] possibleTypes = {ChessPiece.Type.Bishop, 
                                           ChessPiece.Type.Queen};

        for (int i = 0; i < possibleTypes.Length; i++)
        {
            if (type == possibleTypes[i])
            {
                return true;
            }
        }

        return false;
    }

    public bool canMoveCartesianly()
    {
        ChessPiece.Type[] possibleTypes = {ChessPiece.Type.Rook, 
                                           ChessPiece.Type.Queen};

        for (int i = 0; i < possibleTypes.Length; i++)
        {
            if (type == possibleTypes[i])
            {
                return true;
            }
        }

        return false;
    }

    public bool pieceCanBeKilled(int enemyValue)
    {
        ChessPiece.Color enemyColor = getPieceColorFromInt(enemyValue);

        if ((enemyColor != color) && (enemyValue != 0))
        {
            return true;
        }

        return false;
    }

    public bool checkIfOffBoard(Vector2Int cords)
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

    public bool checkIfAllyPiece(Vector2Int moveCords)
    {   
        int tgtValue = mainGameDriver.miniGameBoard[moveCords.x, moveCords.y];

        if (tgtValue == 0)
        {
            return false;
        }

        ChessPiece.Color tgtColor = getPieceColorFromInt(tgtValue);

        if (color == tgtColor)
        {
            return true;
        }

        return false;
    }

    public List<Vector2Int> getAngledMoves()
    {
        List<Vector2Int> angledMoves = new List<Vector2Int>();

        for (int rowIncrement = -1; rowIncrement <= 1; rowIncrement += 2)
        {
            for (int colIncrement = -1; colIncrement <= 1; colIncrement += 2)
            {
                Vector2Int slope = new Vector2Int(colIncrement, rowIncrement);

                angledMoves.AddRange(getLongMoves(slope));
            }
        }

        return angledMoves;
    }

    public List<Vector2Int> getCartesianMoves()
    {
        List<Vector2Int> cartesianMoves = new List<Vector2Int>();

        for (int increment = -1; increment <= 1; increment += 2)
        {
            Vector2Int slope = new Vector2Int(0, increment);
            cartesianMoves.AddRange(getLongMoves(slope));

            slope = new Vector2Int(increment, 0);
            cartesianMoves.AddRange(getLongMoves(slope));
        }

        return cartesianMoves;
    }

    public List<Vector2Int> getLongMoves(Vector2Int slope)
    {
        List<Vector2Int> longMoves = new List<Vector2Int>();

        int row = pos.y + slope.y;
        int col = pos.x + slope.x;

        while ((row < 8) & (col < 8) & (row > -1) & (col > -1))
        {
            if (mainGameDriver.miniGameBoard[col, row] > 0)
            {
                if (!checkIfAllyPiece(new Vector2Int(col, row)))
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

    public List<Vector2Int> getKingMoves()
    {
        List<Vector2Int> kingMoves = new List<Vector2Int>();

        for (int row = pos.y - 1; row <= pos.y + 1; row++)
        {
            for (int col = pos.x - 1; col <= pos.x + 1; col++)
            {
                bool currentSpot = (row == pos.y) & (col == pos.x);
                bool outOfBounds = (row < 8) & (col < 8) & (row > -1) & (col > -1);

                if (!currentSpot & outOfBounds)
                {
                    print((col, row));

                    if (!checkIfAllyPiece(new Vector2Int(col, row)))
                    {
                        kingMoves.Add(new Vector2Int(col, row));
                    }
                }
            }
        }

        return kingMoves;
    }
}
