using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCircle : MonoBehaviour
{
    public GridCreate gridScript;
    public float scrollSpeed;

    public int radius;
    public Color selectedColour = Color.black;

    Vector2 gridOrigin;
    Vector2 circleOrigin;

    float virtualRadius;

    private void Start()
    {
        virtualRadius = radius;
    }

    private void Update()
    {
        pickColor();
        changeSize();
        showSize();
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            findOrigin();
            drawCircle();
        }
    }

    void findOrigin()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float newPosX = (Mathf.Round((mousePos.x - (gridScript.subdivision / 2)) * (1 / gridScript.subdivision)) / (1 / gridScript.subdivision)) + (gridScript.subdivision / 2);
        float newPosY = (Mathf.Round((mousePos.y - (gridScript.subdivision / 2)) * (1 / gridScript.subdivision)) / (1 / gridScript.subdivision)) + (gridScript.subdivision / 2);
        circleOrigin = new Vector2(newPosX, newPosY);
        float newGridPosX = ((-gridScript.transform.position.x + (gridScript.subdivision / 2) + circleOrigin.x) / gridScript.subdivision) - 1;
        float newGridPosY = ((-gridScript.transform.position.y + (gridScript.subdivision / 2) + circleOrigin.y) / gridScript.subdivision) - 1;
        gridOrigin = new Vector2(newGridPosX, newGridPosY);
    }
    void drawCircle()
    {
        float d = 3 - (2 * radius);
        int x = 0;
        int y = radius;

        while (y >= x)
        {
            x++;
            if (d < 0)
                d = d + (4 * x) + 6;
            else
            {
                d = d + 4 * (x - y) + 10;
                y--;
            }
            addPixel(x, y);
        }
        for (int i = (int)gridOrigin.x - radius+1; i < gridOrigin.x + radius+1; i++)
        {
            if(Input.GetKey(KeyCode.Mouse0))
                gridScript.renderers[i, (int)gridOrigin.y].color = selectedColour;
            else if(Input.GetKey(KeyCode.Mouse1))
                gridScript.renderers[i, (int)gridOrigin.y].color = Color.white;
        }
    }
    void addPixel(int x, int y)
    {
        /*
        gridScript.renderers[(int)gridOrigin.x + x, (int)gridOrigin.y + y].color = Color.red;
        gridScript.renderers[(int)gridOrigin.x - x, (int)gridOrigin.y - y].color = Color.red;
        gridScript.renderers[(int)gridOrigin.x - x, (int)gridOrigin.y + y].color = Color.red;
        gridScript.renderers[(int)gridOrigin.x + x, (int)gridOrigin.y - y].color = Color.red;

        gridScript.renderers[(int)gridOrigin.x + y, (int)gridOrigin.y + x].color = Color.red;
        gridScript.renderers[(int)gridOrigin.x - y, (int)gridOrigin.y - x].color = Color.red;
        gridScript.renderers[(int)gridOrigin.x - y, (int)gridOrigin.y + x].color = Color.red;
        gridScript.renderers[(int)gridOrigin.x + y, (int)gridOrigin.y - x].color = Color.red;
        */

        if (Input.GetKey(KeyCode.Mouse0))
        {
            for (int i = 0; i < x * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + x - i, (int)gridOrigin.y + y].color = selectedColour;
            }
            for (int i = 0; i < x * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + x - i, (int)gridOrigin.y - y].color = selectedColour;
            }
            for (int i = 0; i < y * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + y - i, (int)gridOrigin.y + x].color = selectedColour;
            }
            for (int i = 0; i < y * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + y - i, (int)gridOrigin.y - x].color = selectedColour;
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < x * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + x - i, (int)gridOrigin.y + y].color = Color.white;
            }
            for (int i = 0; i < x * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + x - i, (int)gridOrigin.y - y].color = Color.white;
            }
            for (int i = 0; i < y * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + y - i, (int)gridOrigin.y + x].color = Color.white;
            }
            for (int i = 0; i < y * 2; i++)
            {
                gridScript.renderers[(int)gridOrigin.x + y - i, (int)gridOrigin.y - x].color = Color.white;
            }
        }
    }
    void pickColor()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedColour = Color.black;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            selectedColour = Color.red;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            selectedColour = Color.green;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            selectedColour = Color.blue;
    }
    void changeSize()
    {
        if (Input.GetKey(KeyCode.LeftBracket))
            virtualRadius = virtualRadius - (scrollSpeed * Time.deltaTime);
        else if(Input.GetKey(KeyCode.RightBracket))
            virtualRadius = virtualRadius + (scrollSpeed * Time.deltaTime);
        virtualRadius = Mathf.Clamp(virtualRadius, 1, Mathf.Infinity);

        radius = Mathf.RoundToInt(virtualRadius);
    }
    void showSize()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float newPosX = (Mathf.Round((mousePos.x - (gridScript.subdivision / 2)) * (1 / gridScript.subdivision)) / (1 / gridScript.subdivision)) + (gridScript.subdivision / 2);
        float newPosY = (Mathf.Round((mousePos.y - (gridScript.subdivision / 2)) * (1 / gridScript.subdivision)) / (1 / gridScript.subdivision)) + (gridScript.subdivision / 2);
        Vector2 NcircleOrigin = new Vector2(newPosX, newPosY);
        Debug.DrawLine(NcircleOrigin, NcircleOrigin + (Vector2.right * ((radius * gridScript.subdivision) + (gridScript.subdivision / 2))), selectedColour);
        Debug.DrawLine(NcircleOrigin, NcircleOrigin + (Vector2.left * ((radius * gridScript.subdivision) - (gridScript.subdivision / 2))), selectedColour);
        Debug.DrawLine(NcircleOrigin, NcircleOrigin + (Vector2.up * ((radius * gridScript.subdivision) + (gridScript.subdivision / 2))), selectedColour);
        Debug.DrawLine(NcircleOrigin, NcircleOrigin + (Vector2.down * ((radius * gridScript.subdivision) + (gridScript.subdivision / 2))), selectedColour);
    }
}
