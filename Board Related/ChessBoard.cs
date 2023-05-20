using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour
{
    public Vector2Int textureSize = new Vector2Int(440, 440);
    public Texture2D texture;

    public Color DarkSpaceColor = new Color(118, 150, 86);
    public Color LightSpaceColor = new Color(238, 238, 210);
    public Color highlightDarkSpaceColor = new Color(1, 1, 1);
    public Color highlightLightSpaceColor = new Color(1, 1, 1);

    public List<boardSpot> boardSpots = new List<boardSpot>();

    // Board spot struct
    public struct boardSpot
    {
        public Vector2Int bottomLeftCorner, topRightCorner;
        public Color normalColor, highlightColor;

        public boardSpot(Vector2Int bottomLeftCorner, Vector2Int topRightCorner,
                         Color normalColor, Color highlightColor)
        {
            this.bottomLeftCorner = bottomLeftCorner;
            this.topRightCorner = topRightCorner;
            this.normalColor = normalColor;
            this.highlightColor = highlightColor;
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
                Vector2Int bottomLeftCorner = new Vector2Int(row * rowDivValue,
                                                             col * colDivValue);
                Vector2Int topRightCorner = new Vector2Int((row + 1) * rowDivValue, 
                                                           (col + 1) * colDivValue);

                Color normalColor, highlightColor;

                int rowCheck = row / 2;
                int colCheck = col / 2;
                int check = rowCheck + colCheck;

                if (check % 2 == 0)
                {
                    normalColor = DarkSpaceColor;
                    highlightColor = highlightDarkSpaceColor;
                }
                else
                {
                    normalColor = LightSpaceColor;
                    highlightColor = highlightLightSpaceColor;
                }

                boardSpot newBoardSpot = new boardSpot(bottomLeftCorner, 
                                                       topRightCorner,
                                                       normalColor, 
                                                       highlightColor);

                boardSpots.Add(newBoardSpot);

                // Debug
                print((row, col, bottomLeftCorner, 
                       topRightCorner, normalColor, highlightColor));
            }
        }

        print(boardSpots.Count);
    }

    public static Texture2D drawChessBoard(Texture2D texture, 
                                           Color lightColor, 
                                           Color darkColor)
    {   
        int rowDivValue = texture.width / 8;
        int colDivValue = texture.height / 8;

        for (int row = 0; row < texture.width; row++)
        {
            for (int col = 0; col < texture.height; col++)
            {
                int rowCheck = row / rowDivValue;
                int colCheck = col / colDivValue;
                int check = rowCheck + colCheck;

                if (check % 2 == 0)
                {
                    texture.SetPixel(row, col, darkColor);
                }
                else
                {
                    texture.SetPixel(row, col, lightColor);
                }
            }
        }

        return texture;
    }
}
