using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour
{
    // Object
    public MainGameDriver mainGameDriver;
    public Texture2D boardTexture;
    public RawImage boardImage;

    // Unity Types
    private Vector2Int initTextureSize = new Vector2Int(440, 440);
    public Vector2Int textureWorldPos = new Vector2Int(0, 0);
    private Color DarkSpaceColor = new Color(118, 150, 86, 255);
    private Color LightSpaceColor = new Color(238, 238, 210, 255);
    private Color highlightDarkSpaceColor = new Color(1, 1, 1);
    private Color highlightLightSpaceColor = new Color(1, 1, 1);
    public BoardSpace[,] boardSpots = new BoardSpace[8,8];

    // Type
    public int[] worldXPos = new int[8];
    public int[] worldYPos = new int[8];

    // Board spot struct
    public struct BoardSpace {
        public Vector2Int bottomLeftCorner, topRightCorner;
        public Color normalColor, highlightColor;

        public BoardSpace(Vector2Int bottomLeftCorner, 
                          Vector2Int topRightCorner,
                          Color normalColor, 
                          Color highlightColor) {
            this.bottomLeftCorner = bottomLeftCorner;
            this.topRightCorner = topRightCorner;
            this.normalColor = normalColor;
            this.highlightColor = highlightColor;
        }
    }

    public void initChessBoard() {
        // Create the board texture
        boardTexture = new Texture2D(initTextureSize.x, initTextureSize.y);

        // Point the image texture to the newly create texture
        boardImage.texture = boardTexture;

        // Normalize the texture colors
        DarkSpaceColor = DarkSpaceColor / 255;
        LightSpaceColor = LightSpaceColor / 255;

        // Create the X and Y World Position Arrays
        worldXPos = createPosArray(boardTexture.width / 8);
        worldYPos = createPosArray(boardTexture.height / 8);
        Array.Reverse(worldYPos);

        // Define the 2D Array of Board Spots
        defineTextureSpacesMatrix();

        // Debug - Pain one space
        // paintOneBoardSpace(new Vector2Int(0, 7));
        // paintOneBoardSpace(new Vector2Int(1, 7));

        // Update the image texture to reflect the now defined spots
        paintTheBoardSpacesDefault();
    }


    public int[] createPosArray(int posSpacing) {
        int startingPos = posSpacing / 2;
        int[] posArray = new int[8];

        for (int i = 0; i < 8; i++) {
            posArray[i] = startingPos + (posSpacing * i);
        }

        return posArray;
    }


    public void defineTextureSpacesMatrix() {
        // Everything is relative to the texture. None World
        int spaceWidth = boardTexture.width / 8;
        int spaceHeight = boardTexture.height / 8;
    
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                Vector2Int bottomLeftCorner = new Vector2Int(spaceWidth * x,
                                                             spaceHeight * (7 - y)
                                                             );
                Vector2Int topRightCorner = new Vector2Int(spaceWidth * (x + 1), 
                                                           spaceHeight * ((7 - y) + 1)
                                                           );

                Color normalColor = LightSpaceColor;
                Color highlightColor = highlightLightSpaceColor;

                if ((y + x) % 2 == 0) {
                    normalColor = DarkSpaceColor;
                    highlightColor = highlightDarkSpaceColor;
                }

                BoardSpace newBoardSpot = new BoardSpace(bottomLeftCorner, 
                                                         topRightCorner,
                                                         normalColor, 
                                                         highlightColor
                                                         );

                boardSpots[x, y] = newBoardSpot;
            }
        }
    }

    public void paintTheBoardSpacesDefault()
    {
        for (int spaceX = 0; spaceX < 8; spaceX++) {
            for (int spaceY = 0; spaceY < 8; spaceY++) {
                Vector2Int bottomLeft = boardSpots[spaceX, spaceY].bottomLeftCorner;
                Vector2Int topRight = boardSpots[spaceX, spaceY].topRightCorner;
                Color textureColor = boardSpots[spaceX, spaceY].normalColor;

                for (int row = bottomLeft.y; row <= topRight.y; row++)
                {
                    for (int col = bottomLeft.x; col <= topRight.x; col++)
                    {
                        boardTexture.SetPixel(row, col, textureColor);
                    }
                }
            }
        }

        boardTexture.Apply();
    }

    public void highlightBoardSpace(BoardSpace boardSpace)
    {
        Vector2Int bottomLeft = boardSpace.bottomLeftCorner;
        Vector2Int topRight = boardSpace.topRightCorner;
        Color highlightColor = boardSpace.highlightColor;

        for (int y = bottomLeft.y; y <= topRight.y; y++)
        {
            for (int x = bottomLeft.x; x <= topRight.x; x++)
            {
                boardTexture.SetPixel(x, y, highlightColor);
            }
        }

        boardTexture.Apply();
    }

    public void dehighlightBoardSpace(BoardSpace boardSpace)
    {
        Vector2Int bottomLeft = boardSpace.bottomLeftCorner;
        Vector2Int topRight = boardSpace.topRightCorner;
        Color normalColor = boardSpace.normalColor;

        for (int row = bottomLeft.y; row <= topRight.y; row++)
        {
            for (int col = bottomLeft.x; col <= topRight.x; col++)
            {
                boardTexture.SetPixel(col, row, normalColor);
            }
        }

        boardTexture.Apply();
    }

    public void highlightPossibleMoves(List<Vector2Int> possibleMoves)
    {
        // Display those possible moves
        for (int i = 1; i < possibleMoves.Count; i++)
        {
            BoardSpace boardSpace = boardSpots[possibleMoves[i].x, 
                                               possibleMoves[i].y];
            highlightBoardSpace(boardSpace);
        }
    }


    public int getPosIndexNearestPos(float pos, int[] posArray)
    {
        int closeIndex = 0;

        for (int i = 1; i < posArray.Length; i++)
        {   
            if (Mathf.Abs(pos - posArray[i]) < Mathf.Abs(pos - posArray[closeIndex]))
            {
                closeIndex = i;
            }
        }

        return closeIndex;
    }

    //--------------------------------------------------------------------------
    // Debug Methods
    private void printVector(int[] vector) {
        string vectorString = "";
        for (int i = 0; i < vector.Length; i++) {
            vectorString += vector[i] + " ";
        }
        print(vectorString);
    }

    private void paintOneBoardSpace(Vector2Int cord) {
        Vector2Int btmLeft = boardSpots[cord.x, cord.y].bottomLeftCorner;
        Vector2Int topRight = boardSpots[cord.x, cord.y].topRightCorner;
        Color colour = boardSpots[cord.x, cord.y].normalColor;

        print(colour);
        print(Color.red);

        for (int x = btmLeft.x; x < topRight.x; x++) {
            for (int y = btmLeft.y; y < topRight.y; y++) {
                boardTexture.SetPixel(x, y, colour);
            }

        }

        boardTexture.Apply();
    }

}
