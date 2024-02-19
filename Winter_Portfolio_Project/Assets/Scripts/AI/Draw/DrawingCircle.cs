using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingCircle : MonoBehaviour
{
    LineRenderer circleRenderer;

    [SerializeField] int _steps = 100;
    [SerializeField] float _afterSeconds = 1.5f;

    public void Initialize(float radius)
    {
        circleRenderer = GetComponent<LineRenderer>();
        DrawCircle(radius);
        Invoke("DestroyAfterSeconds", _afterSeconds);
    }

    void DestroyAfterSeconds()
    {
        Destroy(gameObject);
    }

    void DrawCircle(float radius)
    {
        circleRenderer.positionCount = _steps + 1;

        Vector3 firstDot = Vector3.zero;

        for (int currentStep = 0; currentStep < _steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / _steps;
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;


            Vector3 currentPosition = new Vector3(x, 0, y);
            if (currentStep == 0) firstDot = currentPosition;

            circleRenderer.SetPosition(currentStep, currentPosition);
        }

        circleRenderer.SetPosition(_steps, firstDot);
    }
}
