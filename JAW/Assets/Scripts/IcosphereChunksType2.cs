using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class IcosphereChunksType2
{
    static readonly Mesh icosahedronTemplate = Resources.Load<Mesh>("IcosahedronTemplate");
    // public static readonly Vector3[] icoVerts = icosahedronTemplate.vertices;
    //public static readonly int[] icoTriangles = icosahedronTemplate.triangles;

    // solutions to:
    // (2B)^2 = A^2 + B^2 + (A - B)^2 // edges are same length
    // A^2 + B^2 = 1 // vertices are on unit sphere
    const float icoA = 0.8506508083520399321815404970630110722404014037648168818367402423f; // sqrt( (5 + sqrt(5)) / 10 ) = sqrt(sqrt(1/5)/phi)
    const float icoB = 0.5257311121191336060256690848478766072854979322433417815289355232f; // sqrt( (5 - sqrt(5)) / 10 ) = sqrt(sqrt(1/5)*phi)

    public static readonly Vector3[] icoVerts =
        new Vector3[12]
        {
            new Vector3(+icoA, +icoB, 0),
            new Vector3(+icoA, -icoB, 0),
            new Vector3(-icoA, +icoB, 0),
            new Vector3(-icoA, -icoB, 0),
            
            new Vector3(0, +icoA, +icoB),
            new Vector3(0, +icoA, -icoB),
            new Vector3(0, -icoA, +icoB),
            new Vector3(0, -icoA, -icoB),
            
            new Vector3(+icoB, 0, +icoA),
            new Vector3(-icoB, 0, +icoA),
            new Vector3(+icoB, 0, -icoA),
            new Vector3(-icoB, 0, -icoA),
        };
    public static readonly int[] icoTriangles =
        new int [20*3]
        {
            // Y: 4 5 6 7
            0, 5, 4, // 00
            2, 4, 5, // 01
            1, 6, 7, // 02
            3, 7, 6, // 03

            // Z: 8 9 10 11
            5,10,11, // 04
            7,11,10, // 05
            4, 9, 8, // 06
            6, 8, 9, // 07

            // X: 0 1 2 3
            0, 8, 1, // 08
            0, 1,10, // 09
            2, 3, 9, // 10
            2,11, 3, // 11

            // diagonal triangles
            0,10, 5, // 12
            2, 5,11, // 13
            2, 9, 4, // 14
            0, 4, 8, // 15
            1, 7,10, // 16
            3,11, 7, // 17
            3, 6, 9, // 18
            1, 8, 6, // 19
        };

    static readonly Dictionary<(int, int), (int, int)> edgesToTriangles = GenerateEdgesToTriangles();
    [UnityEditor.Callbacks.DidReloadScripts]
    static void IcosahedronCheck()
    {
        //common icosahedron sense
        Debug.Assert(icoVerts.Length == 12);
        Debug.Assert(icoTriangles.Length == 20*3);
        // triangles and edge lengths are correct
        for(int i = 0; i < icoTriangles.Length; i += 3)
        {
            Vector3 a, b, c;
            a = icoVerts[icoTriangles[i + 0]];
            b = icoVerts[icoTriangles[i + 1]];
            c = icoVerts[icoTriangles[i + 2]];
            Debug.Assert((a-b).magnitude/icoB == 2.0f);
            Debug.Assert((c-b).magnitude/icoB == 2.0f);
            Debug.Assert((a-c).magnitude/icoB == 2.0f);
        }
        Debug.Log("asserts passed icosahedron edges lenght == 2*icoB");
        // edge -> triangles mapping is correct
        for(int i = 0; i < icoTriangles.Length; i += 3)
        {
            int a = icoTriangles[i + 0];
            int b = icoTriangles[i + 1];
            int c = icoTriangles[i + 2];
            void CheckEdge(int x, int y)
            {
                var key = (Mathf.Min(x, y), Mathf.Max(x, y));
                Debug.Assert(edgesToTriangles.ContainsKey(key));
                var (t1, t2) = edgesToTriangles[key];
                Debug.Assert(t1 == i || t2 == i);
            }
            CheckEdge(a, b);
            CheckEdge(b, c);
            CheckEdge(c, a);
        }
        Debug.Log("asserts passed icosahedron edges to triangles mapping");
    }
    static Dictionary<(int, int), (int, int)> GenerateEdgesToTriangles()
    {
        var dict = new Dictionary<(int, int), (int, int)>();
        for(int i = 0; i < icoTriangles.Length; i += 3)
        {
            int a = icoTriangles[i + 0];
            int b = icoTriangles[i + 1];
            int c = icoTriangles[i + 2];
            void AddEdge(int x, int y)
            {
                var key = (Mathf.Min(x, y), Mathf.Max(x, y));
                if (dict.ContainsKey(key))
                    dict[key] = (dict[key].Item1, i);
                else
                    dict[key] = (i, -1);
            }
            AddEdge(a, b);
            AddEdge(b, c);
            AddEdge(c, a);
        }
        return dict;
    }


    // solutions to:
    // 3C^2 = 1
    // (2B)^2 = (A-C)^2 + (B^-C)2 + C^2 // edges are same length
    // A^2 + B^2 = 1 // vertices are on unit sphere
    const float dodecaA = 0.9341723589627156964511186235480453296292878265169952424405634575f; // sqrt((3 + sqrt(5)) / 6) = sqrt(1/3)/phi
    const float dodecaB = 0.3568220897730899319419698430460878739816860752468683664219611311f; // sqrt((3 - sqrt(5)) / 6) = sqrt(1/3)*phi
    const float dodecaC = 0.5773502691896257645091487805019574556476017512701268760186023264f; // sqrt(1/3)
}