using JetBrains.Annotations;
using UnityEngine;

public class IcosphereChunkType2
{
    static readonly Mesh icosahedronTemplate = Resources.Load<Mesh>("IcosahedronTemplate");
    // public static readonly Vector3[] icoVerts = icosahedronTemplate.vertices;
    //public static readonly int[] icoTriangles = icosahedronTemplate.triangles;

    // solutions to:
    // (2B)^2 = A^2 + B^2 + (A - B)^2 // edges are same length
    // A^2 + B^2 = 1 // vertices are on unit sphere
    const float icoA = 0.8506508083520399321815404970630110722404014037648168818367402423f; // sqrt( (5 + sqrt(5)) / 10 ) = sqrt(1/phi/sqrt(5))
    const float icoB = 0.5257311121191336060256690848478766072854979322433417815289355232f; // sqrt( (5 - sqrt(5)) / 10 ) = sqrt(phi/sqrt(5))

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
    [UnityEditor.Callbacks.DidReloadScripts]
    static void IcosahedronCheck()
    {
        for(int i = 2; i < icoTriangles.Length; i += 3)
        {
            Vector3 a, b, c;
            a = icoVerts[icoTriangles[i - 2]];
            b = icoVerts[icoTriangles[i - 1]];
            c = icoVerts[icoTriangles[i - 0]];
            Debug.Assert((a-b).magnitude/icoB == 2.0f);
            Debug.Assert((c-b).magnitude/icoB == 2.0f);
            Debug.Assert((a-c).magnitude/icoB == 2.0f);
        }
        Debug.Log("asserts passed icosahedron edges lenght == 2*icoB");
    }

    // solutions to:
    // 3C^2 = 1
    // (2B)^2 = (A-C)^2 + (B^-C)2 + C^2 // edges are same length
    // A^2 + B^2 = 1 // vertices are on unit sphere
    const float dodecaA = 0.9341723589627156964511186235480453296292878265169952424405634575f; // sqrt((3 + sqrt(5)) / 6) = sqrt(1/3)/phi
    const float dodecaB = 0.3568220897730899319419698430460878739816860752468683664219611311f; // sqrt((3 - sqrt(5)) / 6) = sqrt(1/3)*phi
    const float dodecaC = 0.5773502691896257645091487805019574556476017512701268760186023264f; // sqrt(1/3)
}

public class FaceChunk : IcosphereChunkType2
{
    [SerializeField] int planetSize;
    [SerializeField] int icoFace;
    [SerializeField] Vector2Int affinPos;
    //public Vector3 pos {get{}}
}
