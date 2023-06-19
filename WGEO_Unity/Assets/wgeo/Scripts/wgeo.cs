using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class wgeo
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public double X;
        public double Y; 
        public double Z;

        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Interval
    {
        public double Min;
        public double Max;

        public Interval(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Quaternion(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
    }

    [DllImport("wgeo_api")]
    public static extern IntPtr NewArc3d(ref Vector3 center, double radius, ref Quaternion rotation, ref Interval t_interval);

    [DllImport("wgeo_api")]
    public static extern void FreeCurve3d(IntPtr curve);

    [DllImport("wgeo_api")]
    public static extern IntPtr NewSphere(ref Vector3 center, double radius, ref Interval u_interval, ref Interval v_interval);

    [DllImport("wgeo_api")]
    public static extern void FreeSurface(IntPtr surface);

    [DllImport("wgeo_api")]
    public static extern IntPtr NewCurve3dFeature(IntPtr curve);

    [DllImport("wgeo_api")]
    public static extern void FreeCurve3dFeature(IntPtr curve_feature);

    [DllImport("wgeo_api")]
    public static extern IntPtr NewSurfaceFeature(IntPtr surface);

    [DllImport("wgeo_api")]
    public static extern void FreeSurfaceFeature(IntPtr surface_feature);

    [DllImport("wgeo_api")]
    public static extern IntPtr SurfaceToMesh(IntPtr surface_feature, bool normal_required, bool uv_required, double unit_epsilon);

    [DllImport("wgeo_api")]
    public static extern void FreeMesh(IntPtr mesh);

    [DllImport("wgeo_api")]
    public static extern int GetMeshVertexCount(IntPtr mesh);

    [DllImport("wgeo_api")]
    public static extern IntPtr GetMeshVertices(IntPtr mesh);

    [DllImport("wgeo_api")]
    public static extern IntPtr GetMeshNormals(IntPtr mesh);

    [DllImport("wgeo_api")]
    public static extern IntPtr GetMeshUVs(IntPtr mesh);

    [DllImport("wgeo_api")]
    public static extern int GetMeshTriangleCount(IntPtr mesh);

    [DllImport("wgeo_api")]
    public static extern IntPtr GetMeshTriangles(IntPtr mesh);

    [DllImport("wgeo_api")]
    public static extern IntPtr Curve3dToPolyline(IntPtr curve_feature, double unit_epsilon);

    [DllImport("wgeo_api")]
    public static extern void FreePolyline(IntPtr polyline);

    [DllImport("wgeo_api")]
    public static extern int GetPolylineVertexCount(IntPtr polyline);

    [DllImport("wgeo_api")]
    public static extern IntPtr GetPolylineVertices(IntPtr polyline);

    [DllImport("wgeo_api")]
    public static extern bool IsPolylineClosed(IntPtr polyline);

    public static IntPtr NewArc3d(Vector3 center, double radius, Quaternion rotation, Interval t_interval)
    {
        return NewArc3d(ref center, radius, ref rotation, ref t_interval);
    }

    public static IntPtr NewSphere(Vector3 center, double radius, Interval u_interval, Interval v_interval)
    {
        return NewSphere(ref center, radius, ref u_interval, ref v_interval);
    }

    public static Mesh SurfaceToUnityMesh(IntPtr surface_feature, bool normal_required, bool uv_required, double unit_epsilon)
    {
        Mesh unity_mesh = new Mesh();
        IntPtr mesh = SurfaceToMesh(surface_feature, normal_required, uv_required, unit_epsilon);
        unsafe
        {
            int vertex_count = GetMeshVertexCount(mesh);
            int triangle_count = GetMeshTriangleCount(mesh) * 3;
            double* p_vertices = (double*)GetMeshVertices(mesh).ToPointer();
            double* p_normals = (double*)GetMeshNormals(mesh).ToPointer();
            double* p_uvs = (double*)GetMeshUVs(mesh).ToPointer();
            int* p_triangles = (int*)GetMeshTriangles(mesh).ToPointer();
            UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[vertex_count];
            for (int i = 0; i < vertex_count; i++)
            {
                int j = i * 3;
                vertices[i] = new UnityEngine.Vector3((float)p_vertices[j], (float)p_vertices[j + 1], (float)p_vertices[j + 2]);
            }
            UnityEngine.Vector3[] normals;
            if (p_normals != null)
            {
                normals = new UnityEngine.Vector3[vertex_count];
                for (int i = 0;i < vertex_count; i++)
                {
                    int j = i * 3;
                    normals[i] = new UnityEngine.Vector3((float)p_normals[j], (float)p_normals[j + 1], (float)p_normals[j + 2]);
                }
            } else
            {
                normals = null;
            }
            UnityEngine.Vector2[] uvs;
            if (p_uvs != null)
            {
                uvs = new UnityEngine.Vector2[vertex_count];
                for (int i = 0; i < vertex_count; i++)
                {
                    int j = i * 2;
                    uvs[i] = new UnityEngine.Vector2((float)p_uvs[j], (float)p_uvs[j + 1]);
                }
            }
            else
            {
                uvs = null;
            }
            int[] triangles = new int[triangle_count];
            for (int i = 0; i < triangle_count; i++)
            {
                triangles[i] = p_triangles[i];
            }
            unity_mesh.vertices = vertices;
            unity_mesh.normals = normals;
            unity_mesh.uv = uvs;
            unity_mesh.triangles = triangles;
        }

        FreeMesh(mesh);
        return unity_mesh;
    }

    public static Polyline3 Curve3dToUnityPolyline3(IntPtr curve_feature, double unit_epsilon)
    {
        Polyline3 unity_polyline = new Polyline3();
        IntPtr polyline = Curve3dToPolyline(curve_feature, unit_epsilon);
        unsafe
        {
            int vertex_count = GetPolylineVertexCount(polyline);
            double* p_vertices = (double*)GetPolylineVertices(polyline).ToPointer();
            bool closed = IsPolylineClosed(polyline);
            UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[vertex_count];
            for (int i = 0; i < vertex_count; i++)
            {
                int j = i * 3;
                vertices[i] = new UnityEngine.Vector3((float)p_vertices[j], (float)p_vertices[j + 1], (float)p_vertices[j + 2]);
            }
            unity_polyline.Vertices = vertices;
            unity_polyline.Closed = closed;
        }

        FreeMesh(polyline);
        return unity_polyline;
    }

}
