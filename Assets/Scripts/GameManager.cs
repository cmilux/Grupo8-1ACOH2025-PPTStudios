using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    PlayerHealth _playerHealth;
    public GameObject _player;

    float _xRange = 8.5f;
    float _yRange = 4.5f;


    private void Update()
    {
        //RestartGame();
        PlayerSideScreenLimit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("WeaponsSystem");        //Loads the game scene
    }

   /*
    void RestartGame()
    {
        _playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();

        if (_playerHealth.playerCurrentHealth <= 0)
        {
            SceneManager.LoadScene("StartMenu");
        }
    }
   */

    //Keeps the player between certain screen range
    public void PlayerSideScreenLimit()
    {
        if (_player == null)
        {
            return;
        }

        //Player is on left side limit
        if (_player.transform.position.x < -_xRange)
        {
            
            _player.transform.position = new Vector2(-_xRange, _player.transform.position.y);
        }

        //Player is on right side limit
        if (_player.transform.position.x > _xRange)
        {
            _player.transform.position = new Vector2(_xRange, _player.transform.position.y);
        }

        if (_player.transform.position.y > _yRange)
        {
            _player.transform.position = new Vector2(_player.transform.position.x, _yRange);
        }

        if (_player.transform.position.y < -_yRange)
        {
            _player.transform.position = new Vector2(_player.transform.position.x, -_yRange);
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
