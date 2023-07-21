using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    // Objects
    private MainGameDriver mainGameDriver;
    private ChessBoard chessBoard;
    private FEN Fen;
    private ChessPieceMaster CPM;

    // Unity Types
    private SpriteRenderer pieceSprite;
    private List<Vector2Int> possibleMoves;
    public Vector2Int pos;
    
    // Types
    public Type type;
    public Color color;
    public int value;
    public int movementCount;   

public void InitChessPiece(int value, Vector2Int posValue, MainGameDriver MGD)
{
    Type type = getPieceTypeFromInt(value);
    Color color = getPieceColorFromInt(value);

    // Piece Objects
    this.mainGameDriver = MGD;
    this.chessBoard = MGD.chessBoard;
    this.Fen = MGD.Fen;
    this.CPM = MGD.CPM;

    // Piece Info
    this.value = value;
    this.type = type;
    this.color = color;
    this.pos = posValue;
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


public static string getPieceLetterFromValue(int pieceVal) {
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


public void snapPieceToGrid(int posXIndex, int posYIndex)
{
    Vector3 currentPosition = this.transform.position;

    int boardPosIndex = posXIndex * 8 + posYIndex;
    currentPosition.x = chessBoard.worldXPos[posXIndex];
    currentPosition.y = chessBoard.worldYPos[posYIndex];

    this.transform.position = currentPosition;
}


public void removeImpedingPiece(Vector2Int pos)
{
    if (Fen.grid[pos.x, pos.y] > 0)
    {
        CPM.deactivateThePieceAtPos(pos);
    }
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


public bool checkIfAllyPiece(Vector2Int moveCords)
{   
    int tgtValue = Fen.grid[moveCords.x, moveCords.y];

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


public Color getEnemyColor(Color allyColor)
{
    if (allyColor == Color.White)
    {
        return Color.Black;
    }
    else
    {
        return Color.White;
    }
}


public bool isKingCastling(Vector2Int newPos)
{
    bool pieceCheck = type == Type.King;
    bool movementCheck = (Mathf.Abs(pos.x - newPos.x) == 2)
                         && (pos.y == newPos.y);

    if (pieceCheck || movementCheck)
    {
        return true;
    }

    return false;
}


//-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-


public bool isSelected = false;

void Start()
{
    mainGameDriver = GameObject.FindGameObjectWithTag("Game Control").
                        GetComponent<MainGameDriver>();

    chessBoard = mainGameDriver.chessBoard;

    pieceSprite = this.GetComponent<SpriteRenderer>();

    movementCount = 0;

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
        // Check if it is the piece's team's turn
        if (Fen.getActiveColor() != color) {
            return;
        }

        // Bring the pieces sprite level up
        pieceSprite.sortingOrder = mainGameDriver.selectedSpriteLayer;

        // Update control booleans
        isSelected = true;
        mainGameDriver.aPieceIsSelected = true;

        // Update the mini board
        // mainGameDriver.updateMiniBoard(pos.x, pos.y, 0);

        // Find the possible moves for the piece
        // possibleMoves = MovementLogic.gatherPossibleMoves(this);

        // Highlight the possible moves
        chessBoard.highlightPossibleMoves(possibleMoves);
    }

    // Setting a piece down
    else if (mainGameDriver.aPieceIsSelected && isSelected)
    {
        // Handle the placement of the piece
        Vector3 thisPosition = this.transform.position;
        int gridX = chessBoard.getPosIndexNearestPos(
                        thisPosition.x,
                        chessBoard.worldXPos
                        );
        int gridY = chessBoard.getPosIndexNearestPos(
                        thisPosition.y,
                        chessBoard.worldYPos
                        );

        // Debug
        print((gridY, gridX));

        // Check if the closes grid positions are valid moves
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            if (possibleMoves[i].x == gridX && possibleMoves[i].y == gridY) {
                bool newSpot = false;
                if (gridX != pos.x || gridY != pos.y) {
                    newSpot = true;
                }

                movingPiece(newSpot, new Vector2Int(gridX, gridY));

                break;
            }
        }
    }
}


public void movingPiece(bool newSpot, Vector2Int movePos) {
    // Update the mini board
    // mainGameDriver.updateMiniBoard(movePos.x, movePos.y, value);

    if (newSpot) {
        // Destroy any piece on the new spot
        removeImpedingPiece(movePos);

        // Log the move in the master list
        MainGameDriver.GameMoves gameMove; 
        gameMove = new MainGameDriver.GameMoves(this, 
                                                pos, 
                                                movePos
                                                );
        mainGameDriver.gameMoves.Add(gameMove);

        // Log that this piece has now moved
        movementCount += 1;

        // Update the board section of the FEN string
        mainGameDriver.Fen.convertBoardToString();

        // Update the active color in the FEN String
        Fen.iterateActiveColor(color);

        // Check for appropriate updates to the castling log in FEN string
        Fen.checkRooksAndKings();

        // Debug
        print(Fen.FENString);
    }

    // Set the object's sprite back to the normal layer
    pieceSprite.sortingOrder = mainGameDriver.normalSpriteLayer;

    // Update this piece's pos
    pos = new Vector2Int(movePos.x, movePos.y);

    snapPieceToGrid(movePos.x, movePos.y);

    // Dehighlight the possible moves
    chessBoard.paintTheBoardSpaces();

    // Log that we are no longer holding a piece
    isSelected = false;
    mainGameDriver.aPieceIsSelected = false;
}


}
