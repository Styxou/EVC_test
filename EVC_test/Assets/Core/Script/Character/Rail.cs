using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailScript : MonoBehaviour
{
    [Header("References")]
    public SplineContainer railSpline;

    [Header("Spline")]
    public bool normalDir;
    public float totalSplineLength;

    // Start is called before the first frame update
    void Start()
    {
        railSpline = GetComponent<SplineContainer>();
        totalSplineLength = railSpline.CalculateLength();
    }

    // convert local float3 position to vector3 world position

    public Vector3 LocalToWorldConversion(float3 localPoint)
    {
        Vector3 worldPos = transform.TransformPoint(localPoint);
        return worldPos;
    }

    // converts vector3 world positions to local float3 positions
    public float3 WorldToLocalConversion(Vector3 worldPoint)
    {
        float3 localPos = transform.InverseTransformPoint(worldPoint);
        return localPos;
    }

    public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
    {
        float3 nearestPoint;
        float time;
        SplineUtility.GetNearestPoint(railSpline.Spline, WorldToLocalConversion(playerPos), out nearestPoint, out time);
        worldPosOnSpline = LocalToWorldConversion(nearestPoint);
        return time;
    }

    public void CalculateDirection(float3 railForward, Vector3 playerForward)
    {
        float angle = Vector3.Angle(playerForward.normalized, railForward);
        if (angle > 90f)
        {
            normalDir = false;
        }
        else
        {
            normalDir = true;
        }
    }

}
