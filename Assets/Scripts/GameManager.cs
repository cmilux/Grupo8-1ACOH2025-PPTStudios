using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;         //Static instance of GameManager
    
    [Header("References")]
    [SerializeField] GameObject _entrancePoint;
    [SerializeField] GameObject _arrowNextLevel;
    [SerializeField] EnemyManager _enemyManager;
    [SerializeField] bool _isPaused;
    
    [Header("Buttons")]
    [SerializeField] Button exitButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button startButton;
    [SerializeField] Button pauseButton;

    [Header("Screen limit variables")]
    private float _xRange = 15.7f;
    private float _yRangeMin = -8.4f;
    private float _yRangeMax = 28.5f;

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

    private void Start()
    {
        
    }

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Zone1" || sceneName == "Zone2" || sceneName == "Zone3")
        {
            //RestartGame();
            PlayerSideScreenLimit();
            ArrowGuide();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;

        if (scene.name == "Zone1" || scene.name == "Zone2" || scene.name == "Zone3")
        {
            PlayerMovement.Instance.transform.position = _entrancePoint.transform.position;
            _arrowNextLevel = GameObject.FindGameObjectWithTag("Arrow");
            _enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
            pauseButton = GameObject.Find("PauseButton")?.GetComponent<Button>();

            if (pauseButton)
            {
                pauseButton.onClick.AddListener(PauseGame);
            }
            /*
            var foundManager = GameObject.FindGameObjectWithTag("EnemyManager");
            if (foundManager != null)
            {
                _enemyManager = foundManager.GetComponent<EnemyManager>();
                Debug.Log("EnemyMan was found");
            }
            */
        }
        
        if (scene.name == "GameOver")
        {
            exitButton  = GameObject.Find("ExitButton")?.GetComponent<Button>();
            menuButton  = GameObject.Find("MenuButton")?.GetComponent<Button>();
            
            if (exitButton)
            {
                exitButton.onClick.AddListener(Exit);
            }
            
            if (menuButton)
            {
                menuButton.onClick.AddListener(BackToMenu);
            }
        }

        if (scene.name == "StartMenu")
        {
            startButton = GameObject.Find("StartButton").GetComponent<Button>();
            exitButton =  GameObject.Find("ExitButton").GetComponent<Button>();
            if (startButton)
            {
                startButton.onClick.AddListener(StartGame);
            }

            if (exitButton)
            {
                exitButton.onClick.AddListener(Exit);
            }
        }
    }
    public void StartGame()
    {
        /*
        var oldPlayer = GameObject.FindGameObjectWithTag("Player");
        if (oldPlayer != null)
        {
            Destroy(oldPlayer);
        }
        */
        
        Time.timeScale = 1;

        SceneManager.LoadScene("Zone1");        //Loads the game scene
    }

    public void PauseGame()
    {
        if (_isPaused)
        {
            Time.timeScale = 1;
            _isPaused = false;
        }
        else
        {
            Time.timeScale = 0;
            PlayerMovement.Instance._animator.enabled = false;
            _isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    public void NextScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //PlayerMovement.Instance.transform.position = _entrancePoint.transform.position;
    }

    void ArrowGuide()
    {
        if (SceneManager.GetActiveScene().name == "Zone3")
        {
            return;
        }
        
        _enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        Debug.Log($"EnemyMan is set");
        
        if (_arrowNextLevel != null)
        {
            if (_enemyManager.enemyCount > 0)
            {
                _arrowNextLevel.SetActive(false);
            }
            if (_enemyManager.enemyCount <= 0)
            {
                _arrowNextLevel.SetActive(true);
            }
        }
        
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
   
   public void BackToMenu()
   {
       SceneManager.LoadScene("StartMenu");
   }

   public void Exit()
   {
       Time.timeScale = 1;
        
#if UNITY_EDITOR
       EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
   }

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
}
