using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour
{
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

    public void defineBoardSpots(Texture2D texture)
    {
        int rowDivValue = texture.width / 8;
        int colDivValue = texture.height / 8;

        print((rowDivValue, colDivValue));

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Vector2Int bottomLeftCorner = new Vector2Int(
                                                  row * rowDivValue,
                                                  col * colDivValue);
                Vector2Int topRightCorner = new Vector2Int(
                                                (row + 1) * rowDivValue - 1, 
                                                (col + 1) * colDivValue - 1);

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

                // Debug
                print((row, col, bottomLeftCorner, 
                       topRightCorner, normalColor, highlightColor,
                       spaceColor));
            }
        }

        print(boardSpots.Count);
    }

    public void paintTheBoardSpaces(Texture2D texture)
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
                    texture.SetPixel(row, col, textureColor);
                }
            }
        }

        texture.Apply();
    }

    public void highlightBoardSpace(Texture2D texture, BoardSpace boardSpace)
    {
        Vector2Int bottomLeft = boardSpace.bottomLeftCorner;
        Vector2Int topRight = boardSpace.topRightCorner;
        Color highlightColor = boardSpace.highlightColor;

        for (int row = bottomLeft.y; row <= topRight.y; row++)
        {
            for (int col = bottomLeft.x; col <= topRight.x; col++)
            {
                texture.SetPixel(row, col, highlightColor);
            }
        }

        texture.Apply();
    }

    public void dehighlightBoardSpace(Texture2D texture, BoardSpace boardSpace)
    {
        Vector2Int bottomLeft = boardSpace.bottomLeftCorner;
        Vector2Int topRight = boardSpace.topRightCorner;
        Color normalColor = boardSpace.normalColor;

        for (int row = bottomLeft.y; row <= topRight.y; row++)
        {
            for (int col = bottomLeft.x; col <= topRight.x; col++)
            {
                texture.SetPixel(row, col, normalColor);
            }
        }

        texture.Apply();
    }
}
