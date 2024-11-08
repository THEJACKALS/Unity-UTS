using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // Objek yang akan diikuti (Capsule)
    public Vector3 offset;            // Jarak antara kamera dan player
    public float rotationSpeed = 5f;  // Kecepatan rotasi kamera
    public float smoothSpeed = 0.125f; // Kecepatan smoothing untuk mengikuti

    private float mouseX, mouseY;      // Input dari pergerakan mouse

    void LateUpdate()
    {
        // Mendapatkan input dari mouse untuk rotasi
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35f, 60f); // Membatasi rotasi vertikal agar tidak terlalu tinggi/rendah

        // Buat rotasi baru berdasarkan input mouse
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        
        // Hitung posisi kamera dari rotasi baru dan offset
        Vector3 desiredPosition = target.position + rotation * offset;

        // Menggunakan Lerp untuk membuat perpindahan kamera lebih smooth
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;

        // Membuat kamera selalu menghadap player
        transform.LookAt(target);
    }
}
