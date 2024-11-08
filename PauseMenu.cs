using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;  // Flag untuk cek apakah game sedang di-pause
    public GameObject pauseMenuUI;            // Referensi ke UI pause menu

    void Update()
    {
        // Jika pemain menekan tombol "Escape", menu pause akan muncul
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();  // Lanjutkan game jika sudah di-pause
            }
            else
            {
                Pause();   // Pause game jika sedang berjalan
            }
        }
    }

    // Fungsi untuk melanjutkan game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);   // Sembunyikan pause menu
        Time.timeScale = 1f;            // Set waktu kembali ke normal
        GameIsPaused = false;           // Set flag bahwa game tidak lagi di-pause
    }

    // Fungsi untuk pause game
    void Pause()
    {
        pauseMenuUI.SetActive(true);    // Tampilkan pause menu
        Time.timeScale = 0f;            // Set waktu berhenti
        GameIsPaused = true;            // Set flag bahwa game sedang di-pause
    }

    // Fungsi untuk restart game (muat ulang scene saat ini)
    public void RestartGame()
    {
        Time.timeScale = 1f;  // Pastikan waktu kembali normal saat restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Muat ulang scene saat ini
    }

    // Fungsi untuk keluar dari game
    public void QuitGame()
    {
        SceneManager.LoadScene("Main Menu");
        Debug.Log("Quitting game...");
       
    }
}
