using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameDriver : MonoBehaviour
{
    public ChessBoard chessBoard;
    public RawImage boardImage;
    public Texture2D boardTexture;
    public GameObject chessPiecePreFab;
    public List<ChessPiece> chessPieces;
    public Sprite[] WhiteSpriteList, BlackSpriteList;
    public List<int> piecePositions;
    public int[,] miniGameBoard;
    public bool aPieceIsSelected = false;
    public int normalSpriteLayer = 50;
    public int selectedSpriteLayer = 60;
    const string startingFENString = 
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    const int maxNumberOfPieces = 32;


    void Start()
    {
        initChessBoard();

        calcBoardPositions(chessBoard.textureSize.x / 16, 
                           chessBoard.textureSize.x / 8);

        initChessGameObjects();

        miniGameBoard = new int[8, 8];

        DecodeFENString(startingFENString);

        debugMiniBoard();

        populateBoard();
    }


    public void initChessBoard()
    {
        boardTexture = new Texture2D(chessBoard.textureSize.x, 
                                chessBoard.textureSize.y);

        RawImage BoardImage = GameObject.FindGameObjectWithTag("Board").
                              GetComponent<RawImage>();
        BoardImage.texture = boardTexture;

        //texture = ChessBoard.drawChessBoard(texture, 
        //                                    chessBoard.LightSpaceColor, 
        //                                    chessBoard.DarkSpaceColor);

        chessBoard.defineBoardSpots(boardTexture);
        
        chessBoard.paintTheBoardSpaces(boardTexture);
    }


    public void calcBoardPositions(int center, int spacing)
    {
        piecePositions = new List<int>();

        for (int i = -4; i < 4; i++)
        {
            piecePositions.Add(center + (i * spacing));
        }

        //piecePositions.Reverse();
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


    public void populateBoard()
    { 
        int pieceBankIndex = 0;

        for (int row = miniGameBoard.GetLength(0) - 1; row >= 0; row--)
        {
            for (int col = 0; col < miniGameBoard.GetLength(1); col++)
                if (miniGameBoard[col, row] > 0)
                {   
                    ChessPiece pieceInfo = chessPieces[pieceBankIndex];
                    GameObject chessPiece = pieceInfo.gameObject;

                    ChessPiece.Type pieceType = ChessPiece.getPieceTypeFromInt(
                        miniGameBoard[col, row]);
                    ChessPiece.Color pieceColor = ChessPiece.getPieceColorFromInt(
                        miniGameBoard[col, row]);

                    pieceInfo.InitChessPiece(pieceType, 
                                            pieceColor,
                                            new Vector2Int(col, row));

                    chessPiece.transform.position = new Vector3(
                        piecePositions[pieceInfo.pos.x], 
                        piecePositions[pieceInfo.pos.y]);

                    ChessPiece.setPieceSprite(chessPiece, 
                                            pieceInfo);

                    chessPiece.SetActive(true);

                    pieceBankIndex += 1;
                }
        }
    }


    public int[] DecodeFENString(string fenString)
    {
        int[] boardVector = new int[8 * 8];

        int fenIndex = 0;
        int boardIndex = 0;

        int row = 7;
        int col = 0;

        // Replace the '/' and split at the first ' '
        fenString = fenString.Replace("/", string.Empty);
        fenString = fenString.Split(' ')[0];

        while (fenIndex < fenString.Length)
        {
            // FEN Index is a piece
            if (isNextFenCharAPiece(fenString[fenIndex]))
            {
                boardVector[boardIndex] = ChessPiece.getPieceValue(
                                              fenString[fenIndex]);

                boardIndex++;

                miniGameBoard[col, row] = ChessPiece.getPieceValue(
                                              fenString[fenIndex]);
                
                col += 1;
            }
            // FEN is blank space
            else
            {
                boardIndex += int.Parse("" + fenString[fenIndex]);
                col += int.Parse("" + fenString[fenIndex]);
            }

            if (col >= 8)
            {
                col = 0;
                row -= 1;
            }

            fenIndex++;
        }

        return boardVector;
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
