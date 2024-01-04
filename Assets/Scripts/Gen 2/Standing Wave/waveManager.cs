using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class waveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LineRenderer wave1;
    [SerializeField] LineRenderer wave2;
    [SerializeField] LineRenderer standingWave;

    [SerializeField] LineRenderer wave1Equilibrium;
    [SerializeField] LineRenderer wave2Equilibrium;
    [SerializeField] LineRenderer standingWaveEquilibrium;

    [Header("Parameters")]
    [SerializeField] Vector2 points;
    [SerializeField] int divisions;
    [SerializeField] float wavelength;
    [SerializeField] float speed;
    [SerializeField] float amplitude;

    float phase = 0f;

    private void Start()
    {
        wave1.positionCount = divisions;
        wave2.positionCount = divisions;
        standingWave.positionCount = divisions;

        for (int i = 0; i < divisions; i++)
        {
            float relative = (float)i / divisions;
            wave1.SetPosition(i, new Vector3(points.y + ((points.x - points.y) * relative), wave1.transform.position.y, 0f));
            wave2.SetPosition(i, new Vector3(points.y + ((points.x - points.y) * relative), wave2.transform.position.y, 0f));
            standingWave.SetPosition(i, new Vector3(points.y + ((points.x - points.y) * relative), standingWave.transform.position.y, 0f));
        }

        wave1Equilibrium.positionCount = 2;
        wave2Equilibrium.positionCount = 2;
        standingWaveEquilibrium.positionCount = 2;

        wave1Equilibrium.SetPosition(0, new Vector3(points.y, wave1Equilibrium.transform.position.y, 0f));
        wave1Equilibrium.SetPosition(1, new Vector3(points.x, wave1Equilibrium.transform.position.y, 0f));

        wave2Equilibrium.SetPosition(0, new Vector3(points.y, wave2Equilibrium.transform.position.y, 0f));
        wave2Equilibrium.SetPosition(1, new Vector3(points.x, wave2Equilibrium.transform.position.y, 0f));

        standingWaveEquilibrium.SetPosition(0, new Vector3(points.y, standingWaveEquilibrium.transform.position.y, 0f));
        standingWaveEquilibrium.SetPosition(1, new Vector3(points.x, standingWaveEquilibrium.transform.position.y, 0f));
    }
    private void Update()
    {
        float relativeSpeed = (speed / wavelength) * 2f * Mathf.PI;
        phase += Time.deltaTime * relativeSpeed;
        if(phase >= 2f * Mathf.PI) phase = 0f;

        generateWave(wave1, phase);
        generateWave(wave2, -phase);
        superPosition(wave1, wave2, standingWave);
    }
    
    void generateWave(LineRenderer _line, float _phase)
    {
        if (wavelength == 0f)
            return;

        for (int i = 0; i < divisions; i++)
        {
            float distance = (float)i / divisions;
            float relativeDistance = (distance / wavelength) * 2f * Mathf.PI;
            float y = Mathf.Sin(relativeDistance + _phase) * amplitude;
            _line.SetPosition(i, new Vector3(_line.GetPosition(i).x, y + _line.transform.position.y, 0f));
        }
    }
    void superPosition(LineRenderer _line1, LineRenderer _line2, LineRenderer _target)
    {
        for (int i = 0; i < divisions; i++)
        {
            float y = (_line1.GetPosition(i).y - _line1.transform.position.y) + (_line2.GetPosition(i).y - _line2.transform.position.y) + _target.transform.position.y;
            _target.SetPosition(i, new Vector3(_target.GetPosition(i).x, y, 0f));
        }
    }
}
