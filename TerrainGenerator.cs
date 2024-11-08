using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Untuk UI Input Field

public class TerrainGenerator : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float minScale = 10f; 
    public float maxScale = 30f;
    public float minHeightMultiplier = 3f; 
    public float maxHeightMultiplier = 10f;
    public Gradient gradient;
    public Material terrainMaterial;

    public int seed; // Seed untuk randomizer
    private float scale;
    private float heightMultiplier;

    // UI references
    public InputField seedInputField; // Referensi ke InputField untuk seed

    void Start()
    {
        // Panggil randomizer untuk pertama kali
        RandomizeParameters();
        GenerateTerrain();
    }

    // Fungsi untuk randomize parameter
    void RandomizeParameters()
    {
        // Jika seed tidak diatur, gunakan seed yang acak
        if (seed == 0)
        {
            seed = Random.Range(0, 100000); // Seed acak di mulai
        }

        Random.InitState(seed);

        // Randomkan scale dan height multiplier
        scale = Random.Range(minScale, maxScale);
        heightMultiplier = Random.Range(minHeightMultiplier, maxHeightMultiplier);

        Debug.Log($"Random Seed: {seed}");
        Debug.Log($"Random Scale: {scale}");
        Debug.Log($"Random Height Multiplier: {heightMultiplier}");
    }

    // Fungsi untuk menggenerate terrain
    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData();
        ApplyTexture(terrain);
        ApplyMaterial(terrain);
    }

    // Fungsi untuk menggenerate data terrain
    TerrainData GenerateTerrainData()
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, heightMultiplier, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    // Fungsi untuk mengenerate heights terrain
    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    // Fungsi untuk menghitung tinggi terrain berdasarkan PerlinNoise
    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    // Fungsi untuk mengaplikasikan texture
    void ApplyTexture(Terrain terrain)
    {
        float minHeight = 0f;
        float maxHeight = 1f;

        TerrainLayer[] terrainLayers = new TerrainLayer[1];
        terrainLayers[0] = new TerrainLayer();
        terrainLayers[0].diffuseTexture = CreateTexture(minHeight, maxHeight);
        terrain.terrainData.terrainLayers = terrainLayers;
    }

    // Fungsi untuk membuat texture dari height
    Texture2D CreateTexture(float minHeight, float maxHeight)
    {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float heightValue = CalculateHeight(x, y);
                Color color = gradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, heightValue));
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }

    // Fungsi untuk mengaplikasikan material ke terrain
    void ApplyMaterial(Terrain terrain)
    {
        if (terrainMaterial != null)
        {
            terrain.materialTemplate = terrainMaterial;
        }
    }

    // Update terrain saat seed berubah
    public void OnSeedChanged(string newSeed)
    {
        if (int.TryParse(newSeed, out int newSeedValue))
        {
            seed = newSeedValue; // Update seed dengan inputan
            RandomizeParameters(); // Generate ulang terrain
            GenerateTerrain(); // Perbarui terrain dengan seed baru
        }
    }

    // Gunakan OnValidate untuk live update ketika perubahan di Inspector
    void OnValidate()
    {
        if (seed != 0) // Hanya jika seed sudah diatur, baru update terrain
        {
            RandomizeParameters(); 
            GenerateTerrain();
        }
    }
}
