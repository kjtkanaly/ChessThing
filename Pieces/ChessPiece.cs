using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Variables

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
    public ChessPieceMaster.Type type;
    public ChessPieceMaster.Color color;
    public int value;
    public int movementCount = 0; 
    public bool isSelected = false;  

    // -------------------------------------------------------------------------
    // Game Events

    void Start()
    {
        pieceSprite = this.GetComponent<SpriteRenderer>();
        
        movementCount = 0;
    }

    void FixedUpdate()
    {
        // Stick the piece to the user's mouse position
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

    // -------------------------------------------------------------------------
    // Master Piece Related Methods

    public void InitChessPiece(int value, Vector2Int posValue, MainGameDriver MGD)
    {
        // Piece Objects
        this.mainGameDriver = MGD;
        this.chessBoard = MGD.chessBoard;
        this.Fen = MGD.Fen;
        this.CPM = MGD.CPM;

        ChessPieceMaster.Type type = CPM.getPieceTypeFromInt(value);
        ChessPieceMaster.Color color = CPM.getPieceColorFromInt(value);

        // Piece Info
        this.value = value;
        this.type = type;
        this.color = color;
        this.pos = posValue;
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
        ChessPieceMaster.Color enemyColor = CPM.getPieceColorFromInt(enemyValue);

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

        ChessPieceMaster.Color tgtColor = CPM.getPieceColorFromInt(tgtValue);

        if (color == tgtColor)
        {
            return true;
        }

        return false;
    }


    public bool isKingCastling(Vector2Int newPos)
    {
        bool pieceCheck = type == ChessPieceMaster.Type.King;
        bool movementCheck = (Mathf.Abs(pos.x - newPos.x) == 2)
                            && (pos.y == newPos.y);

        if (pieceCheck || movementCheck)
        {
            return true;
        }

        return false;
    }


    //-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-%-


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
