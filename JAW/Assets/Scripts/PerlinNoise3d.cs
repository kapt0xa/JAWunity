using UnityEngine;

public class PerlinNoise3d
{
    public PerlinNoise3d(int seed, int sampleSizeVal = 16)
    {
        sampleSize = sampleSizeVal;
        sampleVectors = new Vector3[sampleSize, sampleSize, sampleSize];
        System.Random random = new System.Random(seed);
        for (int x = 0; x < sampleSize; x++)
        {
            for (int y = 0; y < sampleSize; y++)
            {
                for (int z = 0; z < sampleSize; z++)
                {
                    sampleVectors[x, y, z] = RandomDirection(random);
                }
            }
        }
    }

    public float Noise(Vector3 point)
    {
        int x0 = Mathf.FloorToInt(point.x);
        int y0 = Mathf.FloorToInt(point.y);
        int z0 = Mathf.FloorToInt(point.z);
        int x1 = x0 + 1;
        int y1 = y0 + 1;
        int z1 = z0 + 1;

        float xd = point.x - x0;
        float yd = point.y - y0;
        float zd = point.z - z0;

        Vector3 v000 = sampleVectors[x0 % sampleSize, y0 % sampleSize, z0 % sampleSize];
        Vector3 v100 = sampleVectors[x1 % sampleSize, y0 % sampleSize, z0 % sampleSize];
        Vector3 v010 = sampleVectors[x0 % sampleSize, y1 % sampleSize, z0 % sampleSize];
        Vector3 v110 = sampleVectors[x1 % sampleSize, y1 % sampleSize, z0 % sampleSize];
        Vector3 v001 = sampleVectors[x0 % sampleSize, y0 % sampleSize, z1 % sampleSize];
        Vector3 v101 = sampleVectors[x1 % sampleSize, y0 % sampleSize, z1 % sampleSize];
        Vector3 v011 = sampleVectors[x0 % sampleSize, y1 % sampleSize, z1 % sampleSize];
        Vector3 v111 = sampleVectors[x1 % sampleSize, y1 % sampleSize, z1 % sampleSize];

        float n000 = Vector3.Dot(v000, new Vector3(xd, yd, zd));
        float n100 = Vector3.Dot(v100, new Vector3(xd - 1, yd, zd));
        float n010 = Vector3.Dot(v010, new Vector3(xd, yd - 1, zd));
        float n110 = Vector3.Dot(v110, new Vector3(xd - 1, yd - 1, zd));
        float n001 = Vector3.Dot(v001, new Vector3(xd, yd, zd - 1));
        float n101 = Vector3.Dot(v101, new Vector3(xd - 1, yd, zd - 1));
        float n011 = Vector3.Dot(v011, new Vector3(xd, yd - 1, zd - 1));
        float n111 = Vector3.Dot(v111, new Vector3(xd - 1, yd - 1, zd - 1));

        n000 = Mathf.Lerp(n000, n100, xd);
        n010 = Mathf.Lerp(n010, n110, xd);
        n001 = Mathf.Lerp(n001, n101, xd);
        n011 = Mathf.Lerp(n011, n111, xd);

        n000 = Mathf.Lerp(n000, n010, yd);
        n001 = Mathf.Lerp(n001, n011, yd);

        return Mathf.Lerp(n000, n001, zd);
    }

    private Vector3 RandomDirection(System.Random random, int attempts = 3)
    {
        Vector3 vector = Vector3.up;
        for (int i = 0; i < attempts; i++)
        {
             vector = new Vector3(
                (float)(random.NextDouble() * 2 - 1),
                (float)(random.NextDouble() * 2 - 1),
                (float)(random.NextDouble() * 2 - 1)
            );
            if (vector.sqrMagnitude < 0.0001f)
            {
                continue;
            }
            if (vector.sqrMagnitude > 1f)
            {
                continue;
            }
            break;
        }
        return vector.normalized;
    }

    private int sampleSize;
    private Vector3[,,] sampleVectors;
}
