using System.Collections.Generic;
using UnityEngine;

public class IcosphereGenerator
{
    static readonly Mesh icosahedronTemplate = Resources.Load<Mesh>("IcosahedronTemplate");
    static readonly int[] icosahedronTriangles = icosahedronTemplate.triangles;
    static readonly Vector3[] icosahedronVertices = icosahedronTemplate.vertices;

    readonly int subdivisions;
    readonly Mesh icosphereMesh;

    struct NodeName
    {
        public const int OrigVert = 0; // a = vertex id, b = 0, c = 0
        public const int EdgeVert = 1; // a = vertex with smaller id, b = vertex with larger id, c = offet along edge (1 - n-1)
        public const int FaceVert = 2; // a = triangle index offset, b = offset along [0-1] edge, c = offset along [0-2] edge. b>=0, c>=0, b+c<n
        public int type;
        public int a;
        public int b;
        public int c;
    }

    public IcosphereGenerator(int subdivisions)
    {
        this.subdivisions = subdivisions;
        // create verts
        Dictionary<NodeName, int> namedVerts = new Dictionary<NodeName, int>();
        for(int i = 0; i < icosahedronTriangles.Length; i += 3)
        {
            for(int x = 0; x <= subdivisions; x++)
            {
                for(int y = 0; y <= subdivisions - x; y++)
                {
                    var name = GetNodeName(i, x, y);
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
                for(int y = 0; y <= subdivisions - x - 2; y++)
                {
                    a = GetNodeName(i, x, y);
                    b = GetNodeName(i, x + 1, y);
                    c = GetNodeName(i, x, y + 1);
                    d = GetNodeName(i, x + 1, y + 1);

                    triangles.Add(namedVerts[a]);
                    triangles.Add(namedVerts[b]);
                    triangles.Add(namedVerts[c]);

                    triangles.Add(namedVerts[b]);
                    triangles.Add(namedVerts[d]);
                    triangles.Add(namedVerts[c]);
                }
                a = GetNodeName(i, x, subdivisions - x - 1);
                b = GetNodeName(i, x + 1, subdivisions - x - 1);
                c = GetNodeName(i, x, subdivisions - x);

                triangles.Add(namedVerts[a]);
                triangles.Add(namedVerts[b]);
                triangles.Add(namedVerts[c]);
            }
        }

        // transform named verts to positions
        Vector3[] vertices = new Vector3[namedVerts.Count];
        foreach(var pair in namedVerts)
        {
            vertices[pair.Value] = GetNodePos(pair.Key);
        }

        icosphereMesh = new Mesh();
        icosphereMesh.vertices = vertices;
        icosphereMesh.triangles = triangles.ToArray();
        icosphereMesh.RecalculateNormals();
    }

    public Mesh GetMesh()
    {
        return icosphereMesh;
    }

    NodeName GetNodeName(int triangleoffset, int x, int y)
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

    Vector3 GetNodePos(NodeName name)
    {
        switch(name.type)
        {
            case NodeName.OrigVert:
                return icosahedronVertices[name.a].normalized;
            case NodeName.EdgeVert:
                return Vector3.Lerp(
                    icosahedronVertices[name.a],
                    icosahedronVertices[name.b],
                    (float)name.c / subdivisions).normalized;
            case NodeName.FaceVert:
                var v0 = icosahedronVertices[icosahedronTriangles[name.a]];
                var v1 = icosahedronVertices[icosahedronTriangles[name.a + 1]];
                var v2 = icosahedronVertices[icosahedronTriangles[name.a + 2]];
                var offset1 = (v1 - v0) * ((float)name.b / subdivisions);
                var offset2 = (v2 - v0) * ((float)name.c / subdivisions);
                return (v0 + offset1 + offset2).normalized;
            default:
                throw new System.Exception("Invalid node type");
        }
    }
}