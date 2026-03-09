using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class IcosphereApplyer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int subdivisions = 10;
    void Start()
    {
        Console.WriteLine("Applying icosphere to mesh filter...");
        var meshFilter = GetComponent<MeshFilter>();
        var mesh = IcosphereGenerator.Generate(subdivisions, true);
        meshFilter.mesh = mesh;
        Console.WriteLine("Icosphere applied to mesh filter.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
