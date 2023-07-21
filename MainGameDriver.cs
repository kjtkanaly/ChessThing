using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameDriver : MonoBehaviour
{
    // Objects
    public ChessBoard chessBoard;
    public FEN Fen;
    public ChessPieceMaster CPM;
    public List<GameMoves> gameMoves;

    // Types
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

    // -----------------------------------------------------------------------------
    // Game Events

    void Start()
    {
        // Initialize the FEN Object
        Fen.InitFenObj(this);

        // Setup Board Array using FEN String
        Fen.DecodeFENString(Fen.FENString);
        
        // Setup the Board image
        chessBoard.initChessBoard();

        // Create the chess objects for future use
        CPM.createChessObjects(maxNumberOfPieces);

        // Update the chess objects with board accurate info
        CPM.updateChessObjectsInfo(Fen.grid);

        /*
        populateBoard();

        // Init the game moves list
        gameMoves = new List<GameMoves>();
        /**/
    }

    // -----------------------------------------------------------------------------
    // Main Related Functions

    // -------------------------------------------------------------------------
    // Old Functions

}
