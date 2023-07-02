using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameDriver : MonoBehaviour
{
public ChessBoard chessBoard;
public GameObject chessPiecePreFab;
public List<ChessPiece> chessPieces;
public List<GameMoves> gameMoves;
public Sprite[] WhiteSpriteList, BlackSpriteList;
public List<int> pieceTexturePositions;
public int[,] miniGameBoard;
public bool aPieceIsSelected = false;
public int normalSpriteLayer = 50;
public int selectedSpriteLayer = 60;
const int maxNumberOfPieces = 32;
string FENString;
ChessPiece.Color playerColor = ChessPiece.Color.White;
const string colCord = "abcdefgh";
const string rowCord = "12345678";

public struct GameMoves
{
    public ChessPiece piece;
    public Vector2Int previousPos;
    public Vector2Int newPos;

    public GameMoves(ChessPiece piece, 
                        Vector2Int previousPos, 
                        Vector2Int newPos
                        )
    {
        this.piece = piece;
        this.previousPos = previousPos;
        this.newPos = newPos;
    }
}

void Start()
{
    // Set the initial FEN String
    FENString = 
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    chessBoard.initChessBoard();

    calcBoardPositions(chessBoard.textureSize.x / 16, 
                       chessBoard.textureSize.x / 8);
    
    initChessGameObjects();

    miniGameBoard = new int[8, 8];

    DecodeFENString(FENString);
    debugMiniBoard();

    populateBoard();

    // Init the game moves list
    gameMoves = new List<GameMoves>();
}


public void calcBoardPositions(int center, int spacing)
{
    pieceTexturePositions = new List<int>();

    for (int i = -4; i < 4; i++)
    {
        pieceTexturePositions.Add(center + (i * spacing));
    }
}


public void initChessGameObjects()
{
    for (int i = 0; i < maxNumberOfPieces; i++)
    {
        GameObject newPiece = Instantiate(chessPiecePreFab, 
                                            new Vector3Int(0, 0, 0), 
                                            Quaternion.identity);

        newPiece.SetActive(false);

        chessPieces.Add(newPiece.GetComponent<ChessPiece>());
    }
}


public void DecodeFENString(string fenString) {
    fenString = fenString.Split(' ')[0];

    int boardIndex = 0;
    int row = 0;
    int col = 0;

    for (int fenIndex = 0; fenIndex < fenString.Length; fenIndex++)
    {
        if (fenString[fenIndex] == '/') {
            continue;
        }

        if (!System.Char.IsNumber(fenString, fenIndex)) {
            row = (int)(boardIndex % 8);
            col = (int)(boardIndex / 8);

            // Convert Char to piece value
            miniGameBoard[row, col] = 
                ChessPiece.getPieceValue(fenString[fenIndex]);
            boardIndex += 1;
        }
        else {
            boardIndex += int.Parse("" + fenString[fenIndex]);
        }
    }

    if (playerColor == ChessPiece.Color.White) {
        miniGameBoard = General.vertVlipArray(miniGameBoard);
    }
}


public void populateBoard() { 
    int pieceBankIndex = 0;

    for (int row = miniGameBoard.GetLength(0) - 1; row >= 0; row--) {

        for (int col = 0; col < miniGameBoard.GetLength(1); col++) {

            if (miniGameBoard[col, row] > 0) {   
                activatePiece(pieceBankIndex, 
                              miniGameBoard[col, row], 
                              new Vector2Int(col, row)
                              );

                pieceBankIndex += 1;
            }
        }
    }
}


public void activatePiece(int pieceIndex, int pieceVal, Vector2Int piecePos) {
    ChessPiece pieceInfo = chessPieces[pieceIndex];
    GameObject chessPiece = pieceInfo.gameObject;

    // Assign the object the appropriate piece info
    pieceInfo.InitChessPiece(pieceVal,
                             piecePos);

    // Move the object to the texture position
    chessPiece.transform.position = new Vector3(
        pieceTexturePositions[pieceInfo.pos.x], 
        pieceTexturePositions[pieceInfo.pos.y]);

    // Set the piece's appropriate sprite
    ChessPiece.setPieceSprite(chessPiece, 
                              pieceInfo);

    // Finally, activate the piece's gameobject
    chessPiece.SetActive(true);
}


public ChessPiece.Color getActiveColor() {
    ChessPiece.Color activeColor;

    string colorString = FENString.Split(' ')[1];

    if (colorString.ToLower() == "w") {
        activeColor = ChessPiece.Color.White;
    } 
    else {
        activeColor = ChessPiece.Color.Black;
    }

    return activeColor;
}


public void iterateActiveColor(ChessPiece.Color oldColor) {

    int activeColorIndex = FENString.IndexOf(" ") + 1;
    FENString = FENString.Remove(activeColorIndex, 1);

    if (oldColor == ChessPiece.Color.White) {
        FENString = FENString.Insert(activeColorIndex, "b");
    }
    else {
        FENString = FENString.Insert(activeColorIndex, "w");
    }
}


public void logFENStringEnPassing(Vector2Int movePos) {
    char row = rowCord[movePos.y];
    char col = colCord[movePos.x];

    string[] FENStrings = FENString.Split(" ");

    FENStrings[3] = "" + col + row;

    FENString = string.Join(" ", FENStrings);

    print(FENString);
}


private bool isNextFenCharAPiece(char fenChar)
{
    if (char.IsNumber(fenChar) || (fenChar == '/'))
    {
        return false;
    }
    
    return true;
}


public void deactivateThePieceAtPos(int col, int row)
{
    for (int i = 0; i < chessPieces.Count; i++)
    {
        if ((chessPieces[i].pos.x == col) && (chessPieces[i].pos.y == row))
        {
            chessPieces[i].gameObject.SetActive(false);
            chessPieces[i].pos = new Vector2Int(-1, -1);
        }
    }
}


public void updateMiniBoard(int col, int row, int pieceValue)
{
    miniGameBoard[col, row] = pieceValue;
}  


public int getPosIndexNearestPos(float pos, List<int> posArray=null)
{
    // Default
    if (posArray == null)
    {
        posArray = pieceTexturePositions;
    }

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


public ChessPiece getTeamKing(ChessPiece.Color teamColor)
{
    for (int i = 0; i < chessPieces.Count; i++)
    {
        if (chessPieces[i].color == teamColor
            && chessPieces[i].type == ChessPiece.Type.King)
        {
            return chessPieces[i];
        }
    }

    return null;
}

public ChessPiece getPieceAtPos(Vector2Int pos)
{
    for (int i = 0; i < chessPieces.Count; i++)
    {
        if (chessPieces[i].pos.x == pos.x 
            && chessPieces[i].pos.y == pos.y)
        {
            return chessPieces[i];
        }
    }

    return null;
}


//--------------------------------------------------------------------------
// Debug Code

// Prints the Mini Board to the terminal
public void debugMiniBoard()
{
    string boardString = "-------------------------\n";

    for (int row = miniGameBoard.GetLength(0) - 1; row >= 0; row--)
    {
        for (int col = 0; col < miniGameBoard.GetLength(1); col++)
        {   
            if (miniGameBoard[col, row] == 0)
            {
                boardString += "  ";
            }
            boardString += miniGameBoard[col, row].ToString() + ", ";
        }

        boardString += "\n";
    }
    boardString += "-------------------------";

    print(boardString);
}
}
