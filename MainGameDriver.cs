using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameDriver : MonoBehaviour
{
    public ChessBoard chessBoard;
    public RawImage boardImage;
    public GameObject chessPiecePreFab;
    public List<GameObject> chessPieces;
    public Sprite[] WhiteSpriteList, BlackSpriteList;
    public List<int> piecePositions;
    const string startingFENString = 
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    const int maxNumberOfPieces = 32;

    void Start()
    {
        initChessBoard();

        calcBoardPositions(chessBoard.textureSize.x / 16, 
                           chessBoard.textureSize.x / 8);

        initChessGameObjects();

        int[] startingBoard = DecodeFENString(startingFENString);

        populateBoard(startingBoard);
    }

    public void initChessBoard()
    {
        Texture2D texture = new Texture2D(chessBoard.textureSize.x, 
                                          chessBoard.textureSize.y);

        RawImage BoardImage = GameObject.FindGameObjectWithTag("Board").
                              GetComponent<RawImage>();
        BoardImage.texture = texture;

        texture = ChessBoard.drawChessBoard(texture, 
                                            chessBoard.LightSpaceColor, 
                                            chessBoard.DarkSpaceColor);
        
        texture.Apply();
    }

    public void calcBoardPositions(int center, int spacing)
    {
        piecePositions = new List<int>();

        for (int i = -4; i < 4; i++)
        {
            piecePositions.Add(center + (i * spacing));
        }

        piecePositions.Reverse();
    }

    public void initChessGameObjects()
    {
        for (int i = 0; i < maxNumberOfPieces; i++)
        {
            GameObject newPiece = Instantiate(chessPiecePreFab, 
                                              new Vector3Int(0, 0, 0), 
                                              Quaternion.identity);

            newPiece.SetActive(false);

            chessPieces.Add(newPiece);
        }
    }

    public void populateBoard(int[] startingBoard)
    { 
        int pieceBankIndex = 0; 

        for (int i = 0; i < startingBoard.Length; i++)
        {
            if (startingBoard[i] > 0)
            {   
                GameObject chessPiece = chessPieces[pieceBankIndex];
                ChessPiece pieceInfo = chessPiece.GetComponent<ChessPiece>();

                ChessPiece.Type pieceType = ChessPiece.getPieceTypeFromInt(
                    startingBoard[i]);
                ChessPiece.Color pieceColor = ChessPiece.getPieceColorFromInt(
                    startingBoard[i]);

                pieceInfo.InitChessPiece(pieceType, 
                                         pieceColor,
                                         new Vector2Int(i % 8, 
                                                        i / 8));

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
            }
            // FEN is blank space
            else
            {
                boardIndex += int.Parse("" + fenString[fenIndex]);
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
}
