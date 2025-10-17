using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public static GameManager Instance;         //Static instance of GameManager
    [SerializeField] GameObject _entrancePoint;

    [Header("Screen limit variables")]
    [SerializeField] float _xRange = 13.5f;
    [SerializeField] float _yRangeMin = -8.4f;
    [SerializeField] float _yRangeMax = 25.5f;

    private void Awake()
    {
        //If another instance exist, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;                        //Assign the instance
        DontDestroyOnLoad(gameObject);          //Dont destroy between scenes
    }

    private void Update()
    {
        //RestartGame();
        PlayerSideScreenLimit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Zone1");        //Loads the game scene
    }

    public void NextScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerMovement.Instance.transform.position = _entrancePoint.transform.position;
    }
    //I dont think this is necessary but here for testing
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
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
        if (PlayerMovement.Instance == null)
        {
            return;
        }

        //Player is on left side limit
        if (PlayerMovement.Instance.transform.position.x < -_xRange)
        {

            PlayerMovement.Instance.transform.position = new Vector2(-_xRange, PlayerMovement.Instance.transform.position.y);
        }

        //Player is on right side limit
        if (PlayerMovement.Instance.transform.position.x > _xRange)
        {
            PlayerMovement.Instance.transform.position = new Vector2(_xRange, PlayerMovement.Instance.transform.position.y);
        }

        if (PlayerMovement.Instance.transform.position.y > _yRangeMax)
        {
            PlayerMovement.Instance.transform.position = new Vector2(PlayerMovement.Instance.transform.position.x, _yRangeMax);
        }

        if (PlayerMovement.Instance.transform.position.y < _yRangeMin)
        {
            PlayerMovement.Instance .transform.position = new Vector2(PlayerMovement.Instance.transform.position.x, _yRangeMin);
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
