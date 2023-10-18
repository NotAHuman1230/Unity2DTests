using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroid;
    public Transform sun;
    public Transform group;
    public int amount;
    public float minDistance;
    public float maxDistance;

    private void Awake()
    {
        for (int i = 0; i < amount; i++)
        {
            float angle = Random.Range(0f, 360f);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 pos = (Vector2)sun.position + (direction * Random.Range(minDistance, maxDistance));
            GameObject instance = Instantiate(asteroid, pos, Quaternion.identity, group);
        }
    }
}
