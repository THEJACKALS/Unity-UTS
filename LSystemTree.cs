using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemTree : MonoBehaviour
{
    public int iterations = 5;         // Jumlah iterasi
    public float angle = 25f;          // Sudut cabang
    public float length = 10f;         // Panjang cabang
    public float lengthDecay = 0.7f;   // Rasio pemendekan panjang tiap iterasi
    public float angleChange = 5f;     // Perubahan sudut berdasarkan input
    public GameObject branchPrefab;    // Prefab untuk batang/cabang
    public GameObject leafPrefab;      // Prefab untuk daun
    public Terrain terrain;            // Referensi ke Terrain tempat pohon akan disebar
    public int treeCount = 10;         // Jumlah pohon yang akan disebar

    private string axiom = "F";        // Aksiom dasar
    private Dictionary<char, string> rules = new Dictionary<char, string>();  // Aturan produksi
    private string currentString;      

    void Start()
    {
        // Definisikan aturan produksi untuk pohon
        rules.Add('F', "F[+F]F[-F][F]");
        currentString = axiom;

        // Proses iterasi untuk menghasilkan string akhir
        for (int i = 0; i < iterations; i++)
        {
            currentString = GenerateNextString(currentString);
        }

        // Menggambar hasil L-System
        DrawLSystem(currentString);

        // Menyebar pohon secara acak di terrain
        SpreadTrees();
    }

    // Fungsi untuk menghasilkan string baru berdasarkan aturan produksi
    string GenerateNextString(string input)
    {
        string result = "";
        foreach (char c in input)
        {
            result += rules.ContainsKey(c) ? rules[c] : c.ToString();
        }
        return result;
    }

    // Fungsi untuk menggambar L-System menggunakan objek 3D
    void DrawLSystem(string input)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Vector3 position = Vector3.zero;   
        Vector3 direction = Vector3.up;    
        float currentLength = length;

        Debug.Log("Starting L-System Drawing...");  // Debug untuk memulai

        foreach (char c in input)
        {
            if (c == 'F')   
            {
                // Membuat tabung untuk batang/cabang
                Vector3 newPosition = position + direction * currentLength;
                CreateBranch(position, newPosition);
                position = newPosition;
            }
            else if (c == '+')   
            {
                direction = Quaternion.Euler(0, 0, angle) * direction;
                Debug.Log("Rotating to the right by angle: " + angle);  // Debug rotasi kanan
            }
            else if (c == '-')   
            {
                direction = Quaternion.Euler(0, 0, -angle) * direction;
                Debug.Log("Rotating to the left by angle: " + angle);  // Debug rotasi kiri
            }
            else if (c == '[')   
            {
                transformStack.Push(new TransformInfo(position, direction, currentLength));
                Debug.Log("Pushing to stack");  // Debug ketika posisi ditambahkan ke stack
                currentLength *= lengthDecay;
            }
            else if (c == ']')   
            {
                TransformInfo ti = transformStack.Pop();
                Debug.Log("Popping from stack");  // Debug ketika posisi diambil dari stack
                position = ti.position;
                direction = ti.direction;
                currentLength = ti.length;
            }
        }

        Debug.Log("Finished Drawing L-System");  // Debug untuk menandakan selesai
    }

    // Fungsi untuk membuat tabung (branch) dari posisi dan arah
    void CreateBranch(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject branch = Instantiate(branchPrefab, startPosition, Quaternion.identity);
        float distance = Vector3.Distance(startPosition, endPosition);

        // Set ukuran tabung berdasarkan jarak
        branch.transform.localScale = new Vector3(0.1f, distance / 2f, 0.1f);

        // Posisi tabung di tengah antara startPosition dan endPosition
        branch.transform.position = (startPosition + endPosition) / 2;

        // Rotasi tabung agar mengarah sesuai dengan cabang
        branch.transform.up = endPosition - startPosition;
        
        // Tambahkan daun di ujung cabang
        Instantiate(leafPrefab, endPosition, Quaternion.identity);
    }

    // Fungsi untuk menyebarkan pohon secara acak ke terrain
    void SpreadTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            // Ambil posisi acak di terrain
            float x = Random.Range(0f, terrain.terrainData.size.x);
            float z = Random.Range(0f, terrain.terrainData.size.z);
            float y = terrain.SampleHeight(new Vector3(x, 0, z));  // Ambil tinggi pada posisi tersebut

            // Tentukan posisi pohon
            Vector3 treePosition = new Vector3(x, y, z);

            // Tentukan rotasi pohon agar tegak di terrain
            Vector3 normal = terrain.terrainData.GetInterpolatedNormal(x / terrain.terrainData.size.x, z / terrain.terrainData.size.z);
            Quaternion treeRotation = Quaternion.FromToRotation(Vector3.up, normal);

            // Buat pohon di posisi dan rotasi yang telah ditentukan
            Instantiate(branchPrefab, treePosition, treeRotation);
            Instantiate(leafPrefab, treePosition + normal * 2f, Quaternion.identity);  // Menambahkan daun di atas pohon
        }
    }

    // Struct untuk menyimpan informasi transformasi
    private struct TransformInfo
    {
        public Vector3 position;
        public Vector3 direction;
        public float length;

        public TransformInfo(Vector3 position, Vector3 direction, float length)
        {
            this.position = position;
            this.direction = direction;
            this.length = length;
        }
    }

    void Update()
    {
        // Input pengguna untuk mengubah sudut secara dinamis
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle -= angleChange * Time.deltaTime;  // Kurangi sudut
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            angle += angleChange * Time.deltaTime;  // Tambah sudut
        }
        if (Input.GetKey(KeyCode.DownArrow)) 
        {
            angle += 2f * Time.deltaTime;  // Menambah sudut untuk cabang yang mengarah ke bawah
        }
    }
}
