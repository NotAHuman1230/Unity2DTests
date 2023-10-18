using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed;
    public float step;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        float y = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
        transform.Translate(new Vector2(x, y));

        if (Input.GetKey(KeyCode.E))
            Camera.main.orthographicSize += step * Time.deltaTime;
        else if (Input.GetKey(KeyCode.Q) && Camera.main.orthographicSize - (step * Time.deltaTime) >= 0f)
            Camera.main.orthographicSize -= step * Time.deltaTime;
    }
}
