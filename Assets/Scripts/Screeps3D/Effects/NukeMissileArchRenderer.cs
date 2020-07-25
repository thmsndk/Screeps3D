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
        float groundLevel = point1.transform.position.y;
        if(t < 0.015) {
            return new Vector3(point1.transform.position.x, groundLevel + t * 1000, point1.transform.position.z);
        }
        float elevation = groundLevel + 0.015f * 1000;
        // start and end parabola at elevation
        Vector3 parabolaStartV = new Vector3(point1.transform.position.x, elevation, point1.transform.position.z);
        Vector3 parabolaEndV = new Vector3(point2.transform.position.x, elevation,  point2.transform.position.z);

        if ( t >= 0.015 && t <= 0.985 ) {
            return MathParabola.ElevatedParabola(parabolaStartV, parabolaEndV, Constants.ShardHeight, t, 0f);
        }
        return new Vector3(parabolaEndV.x, groundLevel + (1 - t) * 1000, parabolaEndV.z);
    }

    private Vector3 GetRaisePoint(float t) {
        return point1.transform.position + new Vector3(0, 2 + t * 1500, 0);
    }
    // Update is called once per frame
    void Update()
    {
        RenderArc();
    }

    internal void Progress(float progress)
    {
        missile.transform.position = CalculateArcPoint(progress);
        // if(progress <= 0.986) {
            var nextPoint = CalculateArcPoint(progress + 0.001f);
            missile.transform.LookAt(nextPoint);
        // }
    }
}

