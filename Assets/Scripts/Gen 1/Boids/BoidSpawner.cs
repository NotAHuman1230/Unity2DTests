using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [Header("Parameters")]
    public GameObject boid;
    public Transform group;
    public int amount;

    private void Start()
    {
        for (int i = 0; i < amount - 1; i++)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(
                new Vector2(Random.Range(0f, Camera.main.pixelWidth), Random.Range(0f, Camera.main.pixelHeight)));
            Instantiate(boid, pos, Quaternion.identity, group);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(boid, pos, Quaternion.identity, group);
        }
    }
}
