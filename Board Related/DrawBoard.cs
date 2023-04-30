using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawBoard : MonoBehaviour
{
    public Vector2Int textureSize = new Vector2Int(440, 440);
    public Texture2D texture;
    public RawImage BoardImage;

    public Color DarkSpaceColor = new Color(118, 150, 86);
    public Color LightSpaceColor = new Color(238, 238, 210);

    public void main()
    {
        texture = new Texture2D(textureSize.x, textureSize.y);

        BoardImage = GameObject.FindGameObjectWithTag("Board").
                     GetComponent<RawImage>();
        BoardImage.texture = texture;

        texture = drawChessBoard(texture, LightSpaceColor, DarkSpaceColor);
        
        texture.Apply();
    }

    public Texture2D drawChessBoard(Texture2D texture, Color lightColor, 
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
