using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    void Start()
    {
        var surface = wgeo.NewSphere(new wgeo.Vector3(0, 0, 0), 1, new wgeo.Interval(0, Math.PI), new wgeo.Interval(Math.PI, Math.PI * 2));
        var surfaceFeature = wgeo.NewSurfaceFeature(surface);
        var mesh = wgeo.SurfaceToUnityMesh(surfaceFeature, true, true, Math.PI / 7);
        wgeo.FreeSurfaceFeature(surfaceFeature);
        wgeo.FreeSurface(surface);
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    int i;
}
