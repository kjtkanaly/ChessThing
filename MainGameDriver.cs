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
    public int[] piecePositions;
    private string startingFENString = 
        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private int maxPossiblePieces = 32;

    void Start()
    {
        initBoard();

        definePiecePositions(chessBoard.textureSize.x / 16, 
                             chessBoard.textureSize.x / 8);

        for (int i = 0; i < maxPossiblePieces; i++)
        {
            GameObject newPiece = Instantiate(chessPiecePreFab, 
                                              new Vector3Int(0, 0, 0), 
                                              Quaternion.identity);

            newPiece.SetActive(false);

            chessPieces.Add(newPiece);
        }

        piecePositions = initBoardWithPieces(startingFENString);
    }

    public void initBoard()
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

    public void definePiecePositions(int center, int spacing)
    {
        piecePositions = new int[8];

        for (int i = -4; i < 4; i++)
        {
            piecePositions[i + 4] = center + (i * spacing);
        }
    }

    public int[] initBoardWithPieces(string startingFENString)
    {
        int[] startingBoard = DecodeFENString(startingFENString); 
        int pieceBankIndex = 0; 

        for (int i = 0; i < startingBoard.Length; i++)
        {
            if (startingBoard[i] > 0)
            {   
                ChessPiece pieceInfo = chessPieces[pieceBankIndex].
                                           GetComponent<ChessPiece>();

                ChessPiece.Color thisColor = ChessPiece.Color.White;
                if ((startingBoard[i] - 8) > 6)
                {
                    thisColor = ChessPiece.Color.Black;
                }

                int pieceValueHolder = startingBoard[i];

                while (pieceValueHolder > 6)
                {
                    pieceValueHolder -= 8;
                }
                
                ChessPiece.Type thisType = (ChessPiece.Type)pieceValueHolder;

                pieceInfo.InitChessPiece(thisType, 
                                         thisColor,
                                         new Vector2Int(i % 8, 
                                                        i / 8));
                chessPieces[pieceBankIndex].transform.position = 
                    new Vector3(piecePositions[pieceInfo.pos.x], 
                                piecePositions[pieceInfo.pos.y]);

                ChessPiece.setPieceSprite(chessPieces[pieceBankIndex], 
                                          pieceInfo);

                chessPieces[pieceBankIndex].SetActive(true);

                pieceBankIndex += 1;
            }
        }

        return startingBoard;
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
