using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    PlayerMovement _playerMovement;
    public GameObject _player;

    float _xRange = 13.5f;
    float _yRangeMin = -8.4f;
    float _yRangeMax = 25.5f;


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

        if (_player.transform.position.y > _yRangeMax)
        {
            _player.transform.position = new Vector2(_player.transform.position.x, _yRangeMax);
        }

        if (_player.transform.position.y < _yRangeMin)
        {
            _player.transform.position = new Vector2(_player.transform.position.x, -_yRangeMin);
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
