using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;         // Kecepatan pergerakan player
    public float jumpForce = 5f;         // Kekuatan lompatan
    public LayerMask groundLayer;        // Layer tanah untuk deteksi lompat
    public float rotationSpeed = 10f;    // Kecepatan rotasi karakter
    public Camera playerCamera;          // Kamera yang digunakan
    
    public Transform groundCheck;        // Transform untuk pengecekan tanah
    public float groundDistance = 0.4f;  // Jarak sphere untuk deteksi tanah

    public float gravityMultiplier = 2f;     // Pengali gravitasi saat jatuh
    public float lowJumpMultiplier = 2.5f;   // Pengali gravitasi untuk lompat rendah

    private Rigidbody rb;                // Komponen Rigidbody
    private bool isGrounded = false;     // Cek apakah player berada di tanah
    private Vector3 movementDirection;   // Arah pergerakan

    void Start()
    {
        // Ambil Rigidbody dari Capsule
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Input pergerakan horizontal (A/D atau panah kiri/kanan)
        float moveX = Input.GetAxis("Horizontal");

        // Input pergerakan vertikal (W/S atau panah atas/bawah)
        float moveZ = Input.GetAxis("Vertical");

        // Buat vektor untuk arah pergerakan berdasarkan input
        movementDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Cek apakah Capsule berada di tanah dengan CheckSphere
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Debug untuk memeriksa apakah Capsule berada di tanah
        Debug.Log("Is Grounded: " + isGrounded);

        // Jika player menekan tombol "Jump" dan Capsule sedang berada di tanah
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Memanggil fungsi untuk rotasi karakter
        RotateCharacter();
    }

    void FixedUpdate()
    {
        // Set velocity untuk pergerakan Capsule (mengabaikan y-axis untuk menjaga lompatan)
        MoveCharacter();

        // Manipulasi gravitasi saat di udara (untuk lompatan lebih halus)
        ApplyGravity();
    }

    void MoveCharacter()
    {
        // Ambil arah kamera
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0; // Mengabaikan komponen Y agar karakter tidak berputar ke atas/bawah
        cameraForward.Normalize(); // Normalisasi untuk mendapatkan vektor unit

        // Tentukan arah gerakan relatif terhadap kamera
        Vector3 right = playerCamera.transform.right;
        right.y = 0; // Mengabaikan komponen Y
        right.Normalize();

        // Hitung arah gerakan
        Vector3 moveDirection = cameraForward * movementDirection.z + right * movementDirection.x;

        // Set velocity untuk pergerakan Capsule
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    void Jump()
    {
        // Tambahkan gaya lompat pada Rigidbody di arah vertikal (y-axis)
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    void ApplyGravity()
    {
        // Jika sedang jatuh (y-velocity negatif), tambahkan gravitasi
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
        }
        // Jika melakukan lompatan rendah (tombol loncat dilepas), tambahkan gravitasi rendah
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void RotateCharacter()
    {
        // Jika ada input pergerakan (Capsule bergerak)
        if (movementDirection != Vector3.zero)
        {
            // Ambil arah kamera
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0; // Mengabaikan komponen Y agar karakter tidak berputar ke atas/bawah
            cameraForward.Normalize(); // Normalisasi untuk mendapatkan vektor unit

            // Tentukan arah gerakan relatif terhadap kamera
            Vector3 moveDirection = cameraForward * movementDirection.z + playerCamera.transform.right * movementDirection.x;

            // Tentukan rotasi baru berdasarkan arah gerakan
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            // Lerp untuk membuat rotasi lebih smooth
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}