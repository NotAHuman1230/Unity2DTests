using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketManager : MonoBehaviour
{
    WebSocket ws;

    private void Start()
    {
        ws = new WebSocket("ws://localhost:6969");
        ws.OnMessage += (sender, e) => Debug.Log($"Received message from Python: {e.Data}");
        ws.Connect();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Send a message to Python when the space key is pressed
            ws.Send("Hello from Unity!");
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            ws.Close();
        }
    }

    private void OnApplicationQuit() {
        ws.Close();
    }
}
