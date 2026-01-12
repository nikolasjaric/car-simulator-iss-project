using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pausePanel;   // referenca na PausePanel

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;          // pauza simulacije
        pausePanel.SetActive(true);   // pokaži UI pauze

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;      // prikaži kursor
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;          // nastavi simulaciju
        pausePanel.SetActive(false);  // sakrij UI pauze

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;     // sakrij kursor
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
