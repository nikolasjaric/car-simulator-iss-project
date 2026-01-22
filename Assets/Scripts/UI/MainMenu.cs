using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string simulationSceneName = "iss_projekt";

    public void PlayGame()
    {
        // ulaz u simulaciju
        SceneManager.LoadScene(simulationSceneName);

        // u simulaciji sakrij/zaključaj kursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
