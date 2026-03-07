using System.Collections.Generic;
using UnityEngine;

public class IcosphereGenerator
{
    static readonly Mesh icosahedronTemplate = Resources.Load<Mesh>("IcosahedronTemplate");

    readonly int subdivisions;
    readonly Mesh icosphereMesh;

    public IcosphereGenerator(int subdivisions)
    {
        this.subdivisions = subdivisions;

        const int VertType = 0;
        const int EdgeType = 1;
        const int FaceType = 2;

        icos

        List<Vector3> vertices = new List<Vector3>(icosahedronTeplate.vertices);
    }
}
