using Screeps3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class NukeMissileArchRenderer : MonoBehaviour
{
    LineRenderer lr;
    public float velocity;
    public float angle;
    public int resolution = 1000;

    float gravity; // force of gravity
    public float radianAngle;

    public GameObject point1;
    public GameObject point2;
    public Transform point3;

    public GameObject missile;

    public int vertexCount = 12;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        // https://en.wikipedia.org/wiki/Projectile_motion
        gravity = Mathf.Abs(Physics.gravity.y);
        lr.startWidth = 1.5f;
        lr.endWidth = 1.5f;

        //missile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(point1.position, point2.position);

    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawLine(point2.position, point3.position);

    //    Gizmos.color = Color.red;
    //    for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
    //    {
    //        Gizmos.DrawLine(Vector3.Lerp(point1.position, point2.position, ratio), Vector3.Lerp(point2.position, point3.position, ratio));
    //    }
    //}


    // Start is called before the first frame update
    void Start()
    {
        RenderArc();
    }

    /// <summary>
    /// Populates the line renderer
    /// </summary>
    private void RenderArc()
    {


        lr.positionCount = resolution + 1;
        lr.SetPositions(CalculateArcArray());

        //var pointList = new List<Vector3>();
        //for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        //{
        //    var tangentLineVertex1 = Vector3.Lerp(point1.position, point2.position, ratio);
        //    var tangentLineVertex2 = Vector3.Lerp(point2.position, point3.position, ratio);
        //    var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
        //    pointList.Add(bezierpoint);
        //}

        //lr.positionCount = pointList.Count;
        //lr.SetPositions(pointList.ToArray());

        //missile.transform.LookAt(point2.transform, Vector3.down);
        //missile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);


    }

    private Vector3[] CalculateArcArray()
    {
        var arcArray = new Vector3[resolution + 1];
        radianAngle = Mathf.Deg2Rad * angle;
        var maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;

        for (int i = 0; i <= resolution; i++)
        {
            var t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    /// <summary>
    /// Calculates height and distance
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateArcPoint(float t, float maxDistance = 0f)
    {
        //float x = t * maxDistance;
        //float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x)/(2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        //return new Vector3(x, y, point1.position.z);
        return MathParabola.Parabola(point1.transform.position, point2.transform.position, Constants.ShardHeight / 2, t);
    }


    // Update is called once per frame
    void Update()
    {
        RenderArc();
    }

    internal void Progress(float progress)
    {
        missile.transform.position = CalculateArcPoint(progress);
        //Debug.Log($"{progress} {missile.transform.position}");
        var nextPoint = CalculateArcPoint(progress + 0.001f);
        missile.transform.LookAt(nextPoint);
    }
}

public class MathParabola
{

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }

}