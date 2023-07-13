using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour
{
public MainGameDriver mainGameDriver;
public Texture2D boardTexture;
public Vector2Int textureSize = new Vector2Int(440, 440);
public Color DarkSpaceColor = new Color(118, 150, 86);
public Color LightSpaceColor = new Color(238, 238, 210);
public Color highlightDarkSpaceColor = new Color(1, 1, 1);
public Color highlightLightSpaceColor = new Color(1, 1, 1);
public List<BoardSpace> boardSpots = new List<BoardSpace>();

public enum SpaceColor
{
    white = 0,
    black = 1
}

// Board spot struct
public struct BoardSpace
{
    public Vector2Int bottomLeftCorner, topRightCorner;
    public Color normalColor, highlightColor;
    public SpaceColor color;

    public BoardSpace(Vector2Int bottomLeftCorner, Vector2Int topRightCorner,
                        Color normalColor, Color highlightColor, SpaceColor color)
    {
        this.bottomLeftCorner = bottomLeftCorner;
        this.topRightCorner = topRightCorner;
        this.normalColor = normalColor;
        this.highlightColor = highlightColor;
        this.color = color;
    }
}

public void txtTest() {
    for (int row = 0; row <= boardTexture.height/8; row++) {
        for (int col = 0; col <= boardTexture.width/8; col++) {
            boardTexture.SetPixel(col, row, Color.red);
        }
    }

    boardTexture.Apply();
}

public void initChessBoard()
{
    boardTexture = new Texture2D(textureSize.x, textureSize.y);

    RawImage BoardImage = GameObject.FindGameObjectWithTag("Board").
                            GetComponent<RawImage>();
    BoardImage.texture = boardTexture;

    defineBoardSpots();
    paintTheBoardSpacesDefault();
}

public void defineBoardSpots()
{
    int spaceWidth = boardTexture.width / 8;
    int spaceHeight = boardTexture.height / 8;
    int spaceWidthCenter = spaceWidth / 2;
    int spaceHeightCenter = spaceHeight / 2;

    Vector2Int[,] boardPosMap = mainGameDriver.boardPosMap;
 
    for (int row = 7; row >= 0; row--)
    {
        for (int col = 0; col < 8; col++)
        {
            Vector2Int spaceCenter = new Vector2Int(
                                         spaceWidthCenter + (spaceWidth * col),
                                         spaceHeightCenter + (spaceHeight * row)
                                         );

            Vector2Int bottomLeftCorner = new Vector2Int(
                                                spaceWidth * col,
                                                spaceHeight * row
                                                );
            Vector2Int topRightCorner = new Vector2Int(
                                            spaceWidth * (col + 1), 
                                            spaceHeight * (row + 1)
                                            );

            Color normalColor, highlightColor;
            SpaceColor spaceColor;

            int modCheck = row + col;

            if (modCheck % 2 == 0)
            {
                normalColor = DarkSpaceColor;
                highlightColor = highlightDarkSpaceColor;
                spaceColor = SpaceColor.black;
            }
            else
            {
                normalColor = LightSpaceColor;
                highlightColor = highlightLightSpaceColor;
                spaceColor = SpaceColor.white;
            }

            BoardSpace newBoardSpot = new BoardSpace(bottomLeftCorner, 
                                                    topRightCorner,
                                                    normalColor, 
                                                    highlightColor,
                                                    spaceColor);

            boardSpots.Add(newBoardSpot);
        }
    }
}

public void paintTheBoardSpacesDefault()
{
    for (int spaceIndex = 0; spaceIndex < boardSpots.Count; spaceIndex++)
    {
        Vector2Int bottomLeft = boardSpots[spaceIndex].bottomLeftCorner;
        Vector2Int topRight = boardSpots[spaceIndex].topRightCorner;
        Color textureColor = boardSpots[spaceIndex].normalColor;

        for (int row = bottomLeft.y; row <= topRight.y; row++)
        {
            for (int col = bottomLeft.x; col <= topRight.x; col++)
            {
                boardTexture.SetPixel(row, col, textureColor);
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

    print((bottomLeft, topRight));

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
        int spaceIndex = possibleMoves[i].y * 8 + possibleMoves[i].x;
        BoardSpace boardSpace = boardSpots[spaceIndex];
        print((possibleMoves[i], 
               boardSpace.bottomLeftCorner,
               boardSpace.topRightCorner));
        highlightBoardSpace(boardSpace);
    }
}
}
