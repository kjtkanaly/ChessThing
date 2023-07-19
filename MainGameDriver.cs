using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameDriver : MonoBehaviour
{
// Objects
public ChessBoard chessBoard;
public FEN fen;
public GameObject chessPiecePreFab;
public List<ChessPiece> chessPieces;
public List<GameMoves> gameMoves;
public Sprite[] WhiteSpriteList, BlackSpriteList;

// Types
public int[,] miniGameBoard;
public bool aPieceIsSelected = false;
public int normalSpriteLayer = 50;
public int selectedSpriteLayer = 60;
const int maxNumberOfPieces = 32;


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
    // Setup Board Array using FEN String
    fen = this.GetComponent<FEN>();
    fen.DecodeFENString(fen.FENString);
    
    // Setup the Board image
    chessBoard.initChessBoard();

    /*
    initChessGameObjects();

    miniGameBoard = new int[8, 8];

    fen.DecodeFENString(fen.FENString);

    populateBoard();

    // Init the game moves list
    gameMoves = new List<GameMoves>();
    /**/
}

public List<int> calcBoardPositions(int center, int spacing)
{
    List<int> boardPositions = new List<int>();

    for (int i = -4; i < 4; i++)
    {
        boardPositions.Add(center + (i * spacing));
    }

    return boardPositions;
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
    Vector2Int boardPos = new Vector2Int(chessBoard.worldXPos[piecePos.x],
                                         chessBoard.worldYPos[piecePos.y]
                                         );
    chessPiece.transform.position = new Vector3(boardPos.x, boardPos.y);

    // Set the piece's appropriate sprite
    ChessPiece.setPieceSprite(chessPiece, 
                              pieceInfo);

    // Finally, activate the piece's gameobject
    chessPiece.SetActive(true);
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

}
