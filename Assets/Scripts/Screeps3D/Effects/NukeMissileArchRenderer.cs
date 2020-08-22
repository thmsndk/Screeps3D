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
    public float nukeHeightOffset = 1.64f;

    public int vertexCount = 12;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        // https://en.wikipedia.org/wiki/Projectile_motion
        gravity = Mathf.Abs(Physics.gravity.y);
        lr.startWidth = 1.5f;
        lr.endWidth = 1.5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        RenderArc();
    }

    /// <summary>
    /// Populates the line renderer
    /// </summary>
    public void RenderArc()
    {
        lr.positionCount = resolution + 1;
        lr.SetPositions(CalculateArcArray());
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
    public Vector3 CalculateArcPoint(float t, float maxDistance = 0f)
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
            return MathParabola.ElevatedParabola(parabolaStartV, parabolaEndV, Constants.ShardHeight, t, nukeHeightOffset);
        }
        return new Vector3(parabolaEndV.x, groundLevel + (1 - t) * 1000 + nukeHeightOffset, parabolaEndV.z);
    }

    private Vector3 GetRaisePoint(float t) {
        return point1.transform.position + new Vector3(0, 2 + t * 1500, 0);
    }
    // Update is called once per frame
    void Update()
    {
        //calling this every frame is.... bad xD performance takes a huge hit #217
        //RenderArc();
    }

    public void Show(bool show)
    {
        lr.enabled = show;
    }
}

