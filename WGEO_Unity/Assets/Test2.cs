using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    void Start()
    {
        var arc3d = wgeo.NewArc3d(new wgeo.Vector3(0, 0, 0), 1, new wgeo.Quaternion(0, 0, 0, 1), new wgeo.Interval(0, Math.PI * 2));
        var curveFeature = wgeo.NewCurve3dFeature(arc3d);
        var polyline = wgeo.Curve3dToUnityPolyline3(curveFeature, Math.PI / 7);
        wgeo.FreeCurve3dFeature(curveFeature);
        wgeo.FreeCurve3d(arc3d);
        var lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.loop = polyline.Closed;
        lineRenderer.positionCount = polyline.Vertices.Length;
        lineRenderer.SetPositions(polyline.Vertices);
    }
}
