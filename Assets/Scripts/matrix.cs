using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class matrix : MonoBehaviour
{
    private static int blockSize=20;

    private int[,] mainMatrix = new int[Screen.height/blockSize, Screen.width/blockSize];

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Screen.width);
        GameObject square = new GameObject("Square");
        SpriteRenderer renderer = square.AddComponent<SpriteRenderer>();

        // Create a new 20x20 texture
        Texture2D texture = new Texture2D(20, 20);
        
        // Fill the texture with a solid color
        Color32[] colors = new Color32[texture.width * texture.height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.green;
        }

        // Set the sprite to be a simple white square
        renderer.sprite = Sprite.Create(texture, 
                                        new Rect(0, 0, blockSize, blockSize), 
                                        new Vector2(0.5f, 0.5f));

        // Set the color of the square
        renderer.color = Color.green;

        // Set the position of the square
        square.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
