using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoldManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ComputeShader pointInitialise;
    [SerializeField] ComputeShader pointUpdate;
    [SerializeField] ComputeShader screenProcessor;

    [Header("Parameters")]
    [SerializeField] Vector2Int resolution;
    [SerializeField] int pointAmount;
    [SerializeField] float speed;
    [SerializeField] float subtractStrength;
    [SerializeField] float blurStrength;

    RenderTexture unprocessedTexture;
    RenderTexture processedTexture;

    ComputeBuffer pointsBuffer;
    Point[] points;

    public struct Point
    {
        public Vector2 position;
        public float direction;
    }

    private void Start()
    {
        initialiseTexture();

        points = new Point[pointAmount];

        int totalSize = sizeof(float) * 3;
        pointsBuffer = new ComputeBuffer(pointAmount, totalSize);
        pointsBuffer.SetData(points);

        pointInitialise.SetFloats("screenResolution", resolution.x, resolution.y);
        pointInitialise.SetFloats("pointResolution", pointAmount, 1f);
        pointInitialise.SetFloats("seed", Random.Range(0f, 100f), Random.Range(0f, 100f));
        pointInitialise.SetBuffer(0, "points", pointsBuffer);

        pointInitialise.Dispatch(0, (int)Mathf.Ceil(pointAmount / 10), 1, 1);
        pointsBuffer.GetData(points);
        pointsBuffer.Dispose();
    }
    private void Update()
    {
        updatePoints();
        processTexture();
    }

    void initialiseTexture()
    {
        unprocessedTexture = new RenderTexture(resolution.x, resolution.y, 24);
        unprocessedTexture.enableRandomWrite = true;
        unprocessedTexture.filterMode = FilterMode.Point;
        unprocessedTexture.Create();

        processedTexture = new RenderTexture(resolution.x, resolution.y, 24);
        processedTexture.enableRandomWrite = true;
        processedTexture.filterMode = FilterMode.Point;
        processedTexture.Create();
    }

    void updatePoints()
    {
        int totalSize = sizeof(float) * 3;
        pointsBuffer = new ComputeBuffer(pointAmount, totalSize);
        pointsBuffer.SetData(points);

        pointUpdate.SetFloat("speed", Time.deltaTime * speed);
        pointUpdate.SetFloats("pointResolution", pointAmount, 1f);
        pointUpdate.SetFloats("screenResolution", resolution.x, resolution.y);
        pointUpdate.SetFloats("seed", Random.Range(0f, 100f), Random.Range(0f, 100f));
        pointUpdate.SetBuffer(0, "points", pointsBuffer);
        pointUpdate.SetTexture(0, "result", unprocessedTexture);

        pointUpdate.Dispatch(0, (int)Mathf.Ceil(pointAmount / 10), 1, 1);
        pointsBuffer.GetData(points);
        pointsBuffer.Dispose();
    }
    void processTexture()
    {
        screenProcessor.SetFloat("subtract", subtractStrength);
        screenProcessor.SetFloat("blur", blurStrength);
        screenProcessor.SetFloat("deltaTime", Time.deltaTime);
        screenProcessor.SetFloats("resolution", resolution.x, resolution.y);
        screenProcessor.SetTexture(0, "input", unprocessedTexture);
        screenProcessor.SetTexture(0, "result", processedTexture);
        screenProcessor.Dispatch(0, (int)Mathf.Ceil(resolution.x / 8), (int)Mathf.Ceil(resolution.y / 8), 1);

        Graphics.Blit(processedTexture, unprocessedTexture);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(processedTexture, destination);
    }
}
