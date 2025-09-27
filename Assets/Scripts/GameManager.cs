using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    PlayerHealth _playerHealth;

    private void Update()
    {
        //RestartGame();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("WeaponsSystem");        //Loads the game scene
    }

    void RestartGame()
    {
        _playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();

        if (_playerHealth.playerCurrentHealth <= 0)
        {
            SceneManager.LoadScene("StartMenu");
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }
}
