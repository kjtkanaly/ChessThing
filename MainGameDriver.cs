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
public Vector2Int[,] boardPosMap = new Vector2Int[8, 8];
public bool aPieceIsSelected = false;
public int normalSpriteLayer = 50;
public int selectedSpriteLayer = 60;
const int maxNumberOfPieces = 32;
public string FENString;
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

    calcBoardPositions(chessBoard.textureSize.x / 16, 
                       chessBoard.textureSize.x / 8);
    createBoardPosMap();

    chessBoard.initChessBoard();
    initChessGameObjects();

    // Test Board
    chessBoard.txtTest();

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


public void createBoardPosMap() {
    for (int row = 0; row < boardPosMap.GetLength(0); row++) {
        for (int col = 0; col < boardPosMap.GetLength(1); col++) {
            boardPosMap[col, row] = 
                new Vector2Int(pieceTexturePositions[col],
                               pieceTexturePositions[7 - row]);
        }
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
}


public void convertBoardToString(){
    string outputFEN = "";

    for (int i = 0; i < miniGameBoard.GetLength(0); i++) {
        int[] row = getRow(miniGameBoard, i);
        string rowString = "";

        foreach (int val in row) {
            string valString = ChessPiece.getPieceLetterFromValue(val);

            rowString += valString;
        }

        int count = 0;
        string newRowString = "";
        foreach (char val in rowString) {
            if (val == '0') {
                count += 1;
            }
            else {
                newRowString += count.ToString();
                count = 0;
            }
            
            newRowString += val;
        }

        newRowString += count.ToString();
        newRowString = newRowString.Replace("0", "");
        outputFEN += newRowString + "/";
    }

    string[] FENStrings = FENString.Split(" ");
    FENStrings[0] = outputFEN;
    FENString = string.Join(" ", FENStrings);
}


public int[] getRow(int[,] array, int rowIndex) {
    int[] row = new int[array.GetLength(1)];

    for (int col = 0; col < array.GetLength(1); col++) {
        row[col] = array[col, rowIndex];
    }

    return row;
}


public void populateBoard() { 
    int pieceBankIndex = 0;

    for (int row = 0; row < miniGameBoard.GetLength(0); row++) {

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
        boardPosMap[pieceInfo.pos.x, pieceInfo.pos.y].x, 
        boardPosMap[pieceInfo.pos.x, pieceInfo.pos.y].y);

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

    string[] FENStrings = FENString.Split(" ");

    if (oldColor == ChessPiece.Color.White) {
        FENString = FENStrings[1] = "b";
    }
    else {
        FENString = FENStrings[1] = "w";
    }

    FENString = string.Join(" ", FENStrings);
}


public void logPieceEnPassing(bool enPassing, Vector2Int movePos) {
    string[] FENStrings = FENString.Split(" ");

    if (enPassing) {
        char row = rowCord[movePos.y];
        char col = colCord[movePos.x];

        FENStrings[3] = "" + col + row;
    }
    else {
        FENStrings[3] = "-";
    }

    FENString = string.Join(" ", FENStrings);
}


public void checkRooksAndKings()
{
    string[] FENs = FENString.Split(" ");
    string blackCastle = FENs[0].Split("/")[0];
    string whiteCastle = FENs[0].Split("/")[7];
    int[] cols = {0, 4, 7};

    // Black Castle Check
    string ctle = "";
    foreach (int col in cols) {
        ctle += blackCastle[col];
    }

    if (ctle.Substring(0, 2) != "rk") {
        FENs[2] = FENs[2].Replace("q", "");
    }
    if (ctle.Substring(1, 2) != "kr") {
        FENs[2] = FENs[2].Replace("k", "");
    }

    // White Castle Check
    ctle = "";
    foreach (int col in cols) {
        ctle += whiteCastle[col];
    }

    if (ctle.Substring(0, 2) != "RK") {
        FENs[2] = FENs[2].Replace("Q", "");
    }
    if (ctle.Substring(1, 2) != "KR") {
        FENs[2] = FENs[2].Replace("K", "");
    }
    
    if (FENs[2] == "") {
        FENs[2] = "-";
    }

    FENString = string.Join(" ", FENs);
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
public void debugMiniBoard() {
    string[] boardString = new string[8];

    for (int row = 0; row < miniGameBoard.GetLength(0); row++)
    {
        for (int col = 0; col < miniGameBoard.GetLength(1); col++)
        {   
            if (miniGameBoard[col, row] == 0)
            {
                boardString[row] += " ";
            }
            boardString[row] += miniGameBoard[col, row].ToString() + ",";
        }

        boardString[row] += "\n";
    }

    string printString = string.Join("", boardString);

    print(printString);
}

}
