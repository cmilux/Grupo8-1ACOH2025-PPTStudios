using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;             //Static instance of GameManager

    [Header("References")]
    [SerializeField] GameObject _entrancePoint;     //Entrance point of player
    [SerializeField] GameObject _arrowNextLevel;    //Arrow to load next scene
    [SerializeField] EnemyManager _enemyManager;    //Enemy manager script
    [SerializeField] bool _isPaused;                //Bool to check if game is paused

    [Header("Buttons")]
    [SerializeField] Button exitButton;             //Exit button
    [SerializeField] Button menuButton;             //Menu button
    [SerializeField] Button startButton;            //Start button
    [SerializeField] Button pauseButton;            //Pause button
    [SerializeField] Button playButton;             //Play button

    [Header("Scene transition")]
    public Animator transition;
    public float transitionTime = 1.0f;

    [Header("Screen limit variables")]
    private float _xRange = 15.7f;                  //Player X axis screen limit
    private float _yRangeMin = -8.4f;               //Player Y axis left screen limit
    private float _yRangeMax = 28.5f;               //Player Y axis right screen limit

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
        //Set the frame rate
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Zone1" || sceneName == "Zone2" || sceneName == "Zone3")
        {
            //Call these methods
            PlayerSideScreenLimit();
            ArrowGuide();
        }
    }

    void OnEnable()
    {
        //Suscribe to OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        //Desuscribe to OnSceneLoaded
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Game time is running
        Time.timeScale = 1;

        if (scene.name == "Zone1" || scene.name == "Zone2" || scene.name == "Zone3")
        {
            //Get the entrance point of every scene and set it to the player instance
            _entrancePoint = GameObject.FindGameObjectWithTag("Entrance");
            PlayerManager.Instance.transform.position = _entrancePoint.transform.position;
            //Get the next level arrow
            _arrowNextLevel = GameObject.FindGameObjectWithTag("Arrow");
            //Get the enemy manager
            _enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
            //Get the pause button
            pauseButton = GameObject.Find("PauseButton")?.GetComponent<Button>();
            //Get the Crossfade game object to play the transition animation
            transition = GameObject.Find("Crossfade")?.GetComponent<Animator>();

            if (pauseButton)
            {
                //Call the PauseGame method if it was clicked
                pauseButton.onClick.AddListener(PauseGame);
            }
        }

        if (scene.name == "GameOver")
        {
            //Get the exit a menu button of the game over scene
            exitButton = GameObject.Find("ExitButton")?.GetComponent<Button>();
            menuButton = GameObject.Find("MenuButton")?.GetComponent<Button>();

            if (exitButton)
            {
                //Call the Exit method if it was clicked
                exitButton.onClick.AddListener(Exit);
            }

            if (menuButton)
            {
                //Call the BackToMenu method if it was clicked
                menuButton.onClick.AddListener(BackToMenu);
            }
        }

        if (scene.name == "StartMenu")
        {
            //Get the start and exit button of the start menu
            startButton = GameObject.Find("StartButton").GetComponent<Button>();
            exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
            if (startButton)
            {
                //Call the HowToPlay method if it was clicked
                startButton.onClick.AddListener(HowToPlay);
            }

            if (exitButton)
            {
                //Call the Exit method if it was clicked
                exitButton.onClick.AddListener(Exit);
            }
        }

        if (scene.name == "Controls")
        {
            //Get the Crossfade game object to play the transition animation
            //transition = GameObject.Find("Crossfade")?.GetComponent<Animator>();

            //Get the play button of the controls scene
            playButton = GameObject.Find("PlayButton")?.GetComponent<Button>();
            if (playButton)
            {
                //Call the StartGame method if it was clicked
                playButton.onClick.AddListener(StartGame);
            }
        }
    }

    public void HowToPlay()
    {
        //Loads the controls scene
        SceneManager.LoadScene("Controls");
    }
    public void StartGame()
    {
        //Set the time to 1 to make the game run (unpaused)   
        Time.timeScale = 1;
        //Load the firts scene of the game
        SceneManager.LoadScene("Zone1");        //Loads the game scene
    }

    public void PauseGame()
    {
        if (_isPaused)
        {
            //Set the time to 1 to make the game run (unpaused) 
            Time.timeScale = 1;
            _isPaused = false;
            PlayerManager.Instance._animator.enabled = true;      //Play any player animations while game is unpaused
        }
        else
        {
            //Game is paused
            Time.timeScale = 0;
            _isPaused = true;
            PlayerManager.Instance._animator.enabled = false;      //Dont play any player animations while game is paused
        }
    }

    public void NextScene()
    {
        //Load the next scene waiting the transition to be played
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");                 //Set the trigger
        yield return new WaitForSeconds(transitionTime);    //Wait the time set on transtionTime variable
        SceneManager.LoadScene(levelIndex);                 //Load the next scene
    }

    //I dont think this is necessary but here for testing
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    void ArrowGuide()
    {
        if (SceneManager.GetActiveScene().name == "Zone3")
        {
            //Ignore arrow
            return;
        }

        if (_arrowNextLevel != null)
        {
            if (_enemyManager.enemyCount > 0)
            {
                //If there are enemies alive, turn the arrow off
                _arrowNextLevel.SetActive(false);
            }
            if (_enemyManager.enemyCount <= 0)
            {
                //If there are no more enemies alive, turn the arrow on
                _arrowNextLevel.SetActive(true);
            }
        }

    }

    public void BackToMenu()
    {
        //Load StartMenu scene
        SceneManager.LoadScene("StartMenu");
    }

    public void Exit()
    {
        Time.timeScale = 1;
        
        //Exit the game inside of unity or if it's a build, exit the build
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }

    //Keeps the player between certain screen range
    public void PlayerSideScreenLimit()
    {
        if (PlayerManager.Instance == null)
        {
            return;
        }

        //Player left X axis limit
        if (PlayerManager.Instance.transform.position.x < -_xRange)
        {

            PlayerManager.Instance.transform.position = new Vector2(-_xRange, PlayerManager.Instance.transform.position.y);
        }
        //Player right X axis limit
        if (PlayerManager.Instance.transform.position.x > _xRange)
        {
            PlayerManager.Instance.transform.position = new Vector2(_xRange, PlayerManager.Instance.transform.position.y);
        }

        //Player right Y axis limit
        if (PlayerManager.Instance.transform.position.y > _yRangeMax)
        {
            PlayerManager.Instance.transform.position = new Vector2(PlayerManager.Instance.transform.position.x, _yRangeMax);
        }
        //Player left Y axis limit
        if (PlayerManager.Instance.transform.position.y < _yRangeMin)
        {
            PlayerManager.Instance.transform.position = new Vector2(PlayerManager.Instance.transform.position.x, _yRangeMin);
        }
    }
}
