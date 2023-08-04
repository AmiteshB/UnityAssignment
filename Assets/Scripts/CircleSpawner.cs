using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleSpawner : MonoBehaviour
{
    public GameObject circlePrefab;
    public int minCircles = 5;
    public int maxCircles = 10;

    private List<GameObject> circles = new List<GameObject>();
    private bool isDrawingLine = false;
    private LineRenderer lineRenderer;

    private void Start()
    {
        GenerateCircles();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    private void GenerateCircles()
    {
        int numCircles = Random.Range(minCircles, maxCircles + 1);
        for (int i = 0; i < numCircles; i++)
        {
            Vector2 position = new Vector2(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f));
            GameObject circle = Instantiate(circlePrefab, position, Quaternion.identity);
            circles.Add(circle);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawingLine = true;
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrawingLine = false;
            CheckIntersectingCircles();
            lineRenderer.positionCount = 0;
        }

        if (isDrawingLine)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePosition);
        }
    }

    private void CheckIntersectingCircles()
    {
        Vector3[] linePositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePositions);

        for (int i = 0; i < circles.Count; i++)
        {
            CircleCollider2D circleCollider = circles[i].GetComponent<CircleCollider2D>();

            bool intersects = false;
            for (int j = 0; j < linePositions.Length - 1; j++)
            {
                Vector2 startPoint = linePositions[j];
                Vector2 endPoint = linePositions[j + 1];

                if (CircleLineIntersection(circleCollider, startPoint, endPoint))
                {
                    intersects = true;
                    break;
                }
            }

            if (intersects)
            {
                Destroy(circles[i]);
                circles.RemoveAt(i);
                i--;
            }
        }
    }

    private bool CircleLineIntersection(CircleCollider2D circleCollider, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 circleCenter = circleCollider.bounds.center;
        float circleRadius = circleCollider.bounds.extents.x;

        Vector2 lineDir = lineEnd - lineStart;
        Vector2 circleToLineStart = lineStart - circleCenter;

        float a = Vector2.Dot(lineDir, lineDir);
        float b = 2f * Vector2.Dot(circleToLineStart, lineDir);
        float c = Vector2.Dot(circleToLineStart, circleToLineStart) - circleRadius * circleRadius;

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
            return false;

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDiscriminant) / (2 * a);
        float t2 = (-b - sqrtDiscriminant) / (2 * a);

        return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
