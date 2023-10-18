using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreate : MonoBehaviour
{
    public int width;
    public int height;
    public float subdivision;
    public Sprite square;
    public bool showGrid;

    public GameObject[,] gameobjects;
    public SpriteRenderer[,] renderers;

    private void Start()
    {
        showGrid = false;

        float newTPosX = Mathf.Round(transform.position.x * (1 / subdivision)) / (1 / subdivision);
        float newTPosY = Mathf.Round(transform.position.y * (1 / subdivision)) / (1 / subdivision);
        transform.position = new Vector2(newTPosX, newTPosY);

        gameobjects = new GameObject[width, height];
        renderers = new SpriteRenderer[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float newPosX = transform.position.x + ((x + 1) * subdivision) - (subdivision / 2);
                float newPosY = transform.position.y + ((y + 1) * subdivision) - (subdivision / 2);

                GameObject instance = new GameObject();
                SpriteRenderer render = instance.AddComponent<SpriteRenderer>();
                render.sprite = square;
                instance.transform.position = new Vector2(newPosX, newPosY);
                instance.name = x + ", " + y;
                instance.transform.parent = transform;
                instance.transform.localScale = new Vector2(subdivision,subdivision);

                gameobjects[x, y] = instance;
                renderers[x, y] = render;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showGrid)
        {
            for (int x = 0; x < width + 1; x++)
            {
                Vector2 pos1 = new Vector2(transform.position.x + (x * subdivision), transform.position.y);
                Vector2 pos2 = new Vector2(transform.position.x + (x * subdivision), transform.position.y + (subdivision * height));
                Debug.DrawLine(pos1, pos2, Color.black);
            }

            for (int y = 0; y < height + 1; y++)
            {
                Vector2 pos1 = new Vector2(transform.position.x, transform.position.y + (y * subdivision));
                Vector2 pos2 = new Vector2(transform.position.x + (subdivision * width), transform.position.y + (y * subdivision));
                Debug.DrawLine(pos1, pos2, Color.black);
            }
        }
    }
}
