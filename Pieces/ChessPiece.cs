using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
public MainGameDriver mainGameDriver;
public ChessBoard chessBoard;
public FEN fen;
public SpriteRenderer pieceSprite;
public Type type;
public Color color;
public Vector2Int pos;
public Vector2Int enPassingPos;
public List<Vector2Int> possibleMoves;
public int value;
public int movementCount;
public bool enPassing;
public bool canCastleRight;
public bool canCastleLeft;
public static string pieceChars = "kpnbrq";

public void InitChessPiece(int value, 
                           Vector2Int posValue)
{
    Type type = getPieceTypeFromInt(value);
    Color color = getPieceColorFromInt(value);

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


public void snapPieceToGrid(int posXIndex, int posYIndex)
{
    Vector3 currentPosition = this.transform.position;

    int boardPosIndex = posXIndex * 8 + posYIndex;
    currentPosition.x = chessBoard.boardSpots[boardPosIndex].globalCenter.x;
    currentPosition.y = chessBoard.boardSpots[boardPosIndex].globalCenter.y;

    this.transform.position = currentPosition;
}


public void removeImpedingPiece(int col, int row)
{
    if (mainGameDriver.miniGameBoard[col, row] > 0)
    {
        mainGameDriver.deactivateThePieceAtPos(col, row);
    }
}


public void removeEnpassingPiece()
{
    if (enPassing)
    {
        mainGameDriver.deactivateThePieceAtPos(enPassingPos.x, 
                                               enPassingPos.y);

        // Update the mini board
        mainGameDriver.updateMiniBoard(enPassingPos.x, enPassingPos.y, 0);

        enPassing = false;
        enPassingPos = new Vector2Int(-1, -1);
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

    enPassing = false;
    canCastleLeft = false;
    canCastleRight = false;
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
        if (fen.getActiveColor() != color) {
            return;
        }

        // Bring the pieces sprite level up
        pieceSprite.sortingOrder = mainGameDriver.selectedSpriteLayer;

        // Update control booleans
        isSelected = true;
        mainGameDriver.aPieceIsSelected = true;

        // Update the mini board
        mainGameDriver.updateMiniBoard(pos.x, pos.y, 0);

        // Find the possible moves for the piece
        possibleMoves = MovementLogic.gatherPossibleMoves(this);

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
                        chessBoard.globalColPos
                        );
        int gridY = chessBoard.getPosIndexNearestPos(
                        thisPosition.y,
                        chessBoard.globalRowPos
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
    mainGameDriver.updateMiniBoard(movePos.x, movePos.y, value);

    if (newSpot) {
        // Destroy any piece on the new spot
        removeImpedingPiece(movePos.x, movePos.y);

        // Destory any enPassing Pieces
        removeEnpassingPiece();

        // Log the move in the master list
        MainGameDriver.GameMoves gameMove; 
        gameMove = new MainGameDriver.GameMoves(this, 
                                                pos, 
                                                movePos
                                                );
        mainGameDriver.gameMoves.Add(gameMove);

        // Log that this piece has now moved
        movementCount += 1;

        // Log if piece is pawn that is now EnPassing
        checkIfEnPassing(movePos);

        // Update the board section of the FEN string
        mainGameDriver.fen.convertBoardToString();

        // Update the active color in the FEN String
        fen.iterateActiveColor(color);

        // Check for appropriate updates to the castling log in FEN string
        fen.checkRooksAndKings();

        // Debug
        print(fen.FENString);
    }

    // Set the object's sprite back to the normal layer
    pieceSprite.sortingOrder = mainGameDriver.normalSpriteLayer;

    // Update this piece's pos
    pos = new Vector2Int(movePos.x, movePos.y);

    snapPieceToGrid(movePos.x, movePos.y);

    // Dehighlight the possible moves
    chessBoard.paintTheBoardSpacesDefault();

    // Log that we are no longer holding a piece
    isSelected = false;
    mainGameDriver.aPieceIsSelected = false;
}


public void checkIfEnPassing(Vector2Int movePos) {
    bool enPassingCheck = false;

    if ((type == Type.Pawn) && (Mathf.Abs(movePos.y - pos.y) == 2)) {
        enPassingCheck = true;
    }

    fen.logPieceEnPassing(enPassingCheck, movePos);

    return;
}


}
