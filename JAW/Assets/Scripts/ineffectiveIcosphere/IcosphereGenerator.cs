using System;
using System.Collections.Generic;
using UnityEngine;

public class IcosphereGenerator
{
    // solutions to:
    // (2B)^2 = A^2 + B^2 + (A - B)^2 // edges are same length
    // A^2 + B^2 = 1 // vertices are on unit sphere
    const float A = 0.8506508083520399321815404970630110722404014037648168818367402423f; // sqrt( (5 + sqrt(5)) / 10 )
    const float B = 0.5257311121191336060256690848478766072854979322433417815289355232f; // sqrt( (5 - sqrt(5)) / 10 )
    static readonly Mesh icosahedronTemplate = Resources.Load<Mesh>("IcosahedronTemplate");
    static readonly int[] icosahedronTriangles = icosahedronTemplate.triangles;
    static readonly Vector3[] icosahedronVertices = icosahedronTemplate.vertices;

    readonly int subdivisions;
    readonly Mesh icosphereMesh;

    struct NodeName
    {
        public const char OrigVert = 'V'; // a = vertex id, b = 0, c = 0
        public const char EdgeVert = 'E'; // a = vertex with smaller id, b = vertex with larger id, c = offet along edge (1 - n-1)
        public const char FaceVert = 'F'; // a = triangle index offset, b = offset along [0-1] edge, c = offset along [0-2] edge. b>=0, c>=0, b+c<n
        public char type;
        public int a;
        public int b;
        public int c;
    }

    public static Mesh Generate(int subdivisions, bool normalized = true)
    {
        // create verts
        Dictionary<NodeName, int> namedVerts = new Dictionary<NodeName, int>();
        for(int i = 0; i < icosahedronTriangles.Length; i += 3)
        {
            for(int x = 0; x <= subdivisions; x++)
            {
                for(int y = 0; y <= subdivisions - x; y++)
                {
                    var name = GetNodeName(i, x, y, subdivisions);
                    if(!namedVerts.ContainsKey(name))
                    {
                        namedVerts[name] = namedVerts.Count;
                    }
                }
            }
        }

        // create triangles
        List<int> triangles = new List<int>();
        for(int i = 0; i < icosahedronTriangles.Length; i += 3)
        {
            for(int x = 0; x <= subdivisions; x++)
            {
                NodeName a, b, c, d;
                int y;
                for(y = 0; y < subdivisions - x - 1; y++)
                {
                    a = GetNodeName(i, x, y, subdivisions);
                    b = GetNodeName(i, x + 1, y, subdivisions);
                    c = GetNodeName(i, x, y + 1, subdivisions);
                    d = GetNodeName(i, x + 1, y + 1, subdivisions);

                    triangles.Add(namedVerts[a]);
                    triangles.Add(namedVerts[b]);
                    triangles.Add(namedVerts[c]);

                    triangles.Add(namedVerts[b]);
                    triangles.Add(namedVerts[d]);
                    triangles.Add(namedVerts[c]);
                }

                if(y == subdivisions - x - 1)
                {
                    a = GetNodeName(i, x, y, subdivisions);
                    b = GetNodeName(i, x + 1, y, subdivisions);
                    c = GetNodeName(i, x, y + 1, subdivisions);

                    triangles.Add(namedVerts[a]);
                    triangles.Add(namedVerts[b]);
                    triangles.Add(namedVerts[c]);
                }
            }
        }

        // transform named verts to positions
        Vector3[] vertices = new Vector3[namedVerts.Count];
        foreach(var pair in namedVerts)
        {
            vertices[pair.Value] = GetNodePos(pair.Key, subdivisions, normalized);
        }

        var icosphereMesh = new Mesh();
        icosphereMesh.vertices = vertices;
        icosphereMesh.triangles = triangles.ToArray();
        icosphereMesh.RecalculateNormals();
        return icosphereMesh;
    }

    static NodeName GetNodeName(int triangleoffset, int x, int y, int subdivisions)
    {
        if(x == 0 && y == 0)
        {
            return new NodeName { type = NodeName.OrigVert, a = icosahedronTriangles[triangleoffset], b = 0, c = 0 };
        }
        if(x == subdivisions && y == 0)
        {
            return new NodeName { type = NodeName.OrigVert, a = icosahedronTriangles[triangleoffset + 1], b = 0, c = 0 };
        }
        if(x == 0 && y == subdivisions)
        {
            return new NodeName { type = NodeName.OrigVert, a = icosahedronTriangles[triangleoffset + 2], b = 0, c = 0 };
        }

        if(y == 0)
        {
            var edge = new NodeName { type = NodeName.EdgeVert,
                a = icosahedronTriangles[triangleoffset],
                b = icosahedronTriangles[triangleoffset + 1],
                c = x };
            if(edge.a > edge.b)
            {
                var temp = edge.a;
                edge.a = edge.b;
                edge.b = temp;
                edge.c = subdivisions - edge.c;
            }
            return edge;
        }
        if(x == 0)
        {
            var edge = new NodeName { type = NodeName.EdgeVert,
                a = icosahedronTriangles[triangleoffset],
                b = icosahedronTriangles[triangleoffset + 2],
                c = y };
            if(edge.a > edge.b)
            {
                var temp = edge.a;
                edge.a = edge.b;
                edge.b = temp;
                edge.c = subdivisions - edge.c;
            }
            return edge;
        }
        if( x + y == subdivisions)
        {
            var edge = new NodeName { type = NodeName.EdgeVert,
                a = icosahedronTriangles[triangleoffset + 1],
                b = icosahedronTriangles[triangleoffset + 2],
                c = y };
            if(edge.a > edge.b)
            {
                var temp = edge.a;
                edge.a = edge.b;
                edge.b = temp;
                edge.c = x;
            }
            return edge;
        }

        return new NodeName 
        {
            type = NodeName.FaceVert,
            a = triangleoffset,
            b = x,
            c = y
        };
    }

    static Vector3 GetNodePos(NodeName name, int subdivisions, bool normalized)
    {
        Vector3 pos;
        switch(name.type)
        {
            case NodeName.OrigVert:
                pos = icosahedronVertices[name.a];
                break;
            case NodeName.EdgeVert:
                pos = Vector3.Lerp(
                    icosahedronVertices[name.a],
                    icosahedronVertices[name.b],
                    (float)name.c / subdivisions);
                break;
            case NodeName.FaceVert:
                var v0 = icosahedronVertices[icosahedronTriangles[name.a]];
                var v1 = icosahedronVertices[icosahedronTriangles[name.a + 1]];
                var v2 = icosahedronVertices[icosahedronTriangles[name.a + 2]];
                var offset1 = (v1 - v0) * ((float)name.b / subdivisions);
                var offset2 = (v2 - v0) * ((float)name.c / subdivisions);
                pos = v0 + offset1 + offset2;
                break;
            default:
                throw new System.Exception("Invalid node type");
        }

        return normalized ? pos.normalized : pos;
    }
}