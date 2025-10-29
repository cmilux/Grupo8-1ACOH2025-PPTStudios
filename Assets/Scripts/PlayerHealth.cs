using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player references")]
    [SerializeField] GameObject _player;
    [SerializeField] PlayerManager _playerAnimator;

    [Header("Health integers")]
    [SerializeField] float _playerMaxHealth;      // Stores the max amount of health a player can have
    [SerializeField] public float playerCurrentHealth;  // Stores how much health the player has currently

    [Header("SFX")]
    [SerializeField] AudioClip _playerHitSFX;
    [SerializeField] AudioClip _playerDeathSFX;
    [SerializeField] AudioClip _playerRecoverHealthSFX;
    private AudioSource _playerHealthSFX;

    [Header("Booleans")]
    [SerializeField] public bool activateInkSplatterEffect;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _playerHealth;
    [SerializeField] Slider _healthBar;

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
        if (scene.name == "Zone1")
        {
            playerCurrentHealth = _playerMaxHealth;                                                     //Set player health to max
            _playerAnimator = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();       //Find the playerManager script
            _playerAnimator._isDead = false;                                                            //Player is alive
        }

        if (scene.name == "Zone1" || scene.name == "Zone2" || scene.name == "Zone3")
        {
            _playerHealthSFX = GetComponent<AudioSource>();                                             //Get the audio source
            _healthBar = GameObject.Find("PlayerHealthUI")?.GetComponent<Slider>();                //Get the player health slider UI
            SettingUI();                                                                                //Update player life in UI
        }
    }

    private void Update()
    {
        //Call methods
        PreventFromExceeding();
        OnDeath();
        SettingUI();
        OnPlayerBeingAttacked();
    }

    void PreventFromExceeding()
    {
        // If the player heals more than the max health, set current health to max limit. 
        if (playerCurrentHealth >= _playerMaxHealth)
        {
            playerCurrentHealth = _playerMaxHealth;
        }
    }

    void OnPlayerBeingAttacked()
    {
        if (_playerAnimator._isBeingAttacked == true)
        {
            //Player cant move if is being attacked
            PlayerManager.Instance._playerInput.enabled = false;
        }
        else if (_playerAnimator._isBeingAttacked == false)
        {
            //Player can move if not being attacked
            PlayerManager.Instance._playerInput.enabled = true;
        }
    }
    void OnDeath()
    {
        //Checks player's health
        if (!_playerAnimator._isDead && playerCurrentHealth <= 0)
        {
            //Plays animation
            _playerAnimator._isDead = true;
            //Plays the SFX
            _playerHealthSFX.PlayOneShot(_playerDeathSFX, 0.3f );
            //Starts a coroutine to make a softer transition
            StartCoroutine(WaitnLoadGameOverScene());
        }
    }

    IEnumerator WaitnLoadGameOverScene()
    {
        //Waits for animation to play before showing GameOver scene
        yield return new WaitForSeconds(2.2f);

        SceneManager.LoadScene("GameOver");
    }

    IEnumerator WaitForAnimationToEnd()
    {
        //Wait for hit animation to player to fully play
        yield return new WaitForSeconds(1f);
        _playerAnimator._isBeingAttacked = false;
    }
    void SettingUI()
    {
        //Set and update the player life to the slider object in UI
        _healthBar = GameObject.Find("PlayerHealthUI")?.GetComponent<Slider>();
        _healthBar.value = playerCurrentHealth / _playerMaxHealth;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Gets the damage value from the enemy that the player has collided with 
            int damageAmount = other.gameObject.GetComponent<PathTest>().enemyDamage;

            // Applies that damage amount to the player health
            playerCurrentHealth -= damageAmount;

            //Plays the animation
            _playerAnimator._isBeingAttacked = true;
            //Plays the SFX
            _playerHealthSFX.PlayOneShot(_playerHitSFX, 0.3f);
            //Wait for animation to finish
            StartCoroutine(WaitForAnimationToEnd());
        }

        if (other.gameObject.CompareTag("Ink"))
        {
            // Gets the damage value from the enemy that the player has collided with 
            int inkDamage = other.gameObject.GetComponent<InkManager>().inkDamage;

            // Applies that damage amount to the player health
            playerCurrentHealth -= inkDamage;

            // Calls for the ink splatter effect handled by the ranged enemy manager
            activateInkSplatterEffect = true;

            //Plays the animation
            _playerAnimator._isBeingAttacked = true;
            //Plays the SFX
            _playerHealthSFX.PlayOneShot(_playerHitSFX, 0.3f);
            //Wait for animation to finish
            StartCoroutine(WaitForAnimationToEnd());
            //Destroy the ink
            Destroy(other.gameObject);
        }

        //if (other.gameObject.CompareTag("Tentacle"))
        //{
        //    // Gets the damage value from the tentacle that the player has collided with 
        //    int damageAmount = other.gameObject.GetComponent<BossTentacleManager>().tentacleDamage;
        //
        //    // Applies that damage amount to the player health
        //    playerCurrentHealth -= damageAmount;
        //    // Player is being attacked
        //    _playerAnimator._isBeingAttacked = true;
        //    // Wait for animation to finish
        //    StartCoroutine(WaitForAnimationToEnd());
        //}

        if (other.gameObject.CompareTag("Food"))
        {
            if (playerCurrentHealth != _playerMaxHealth)   // If player has less than the max health amount...
            {
                // Apply heal and destroy food item
                int healAmount = other.gameObject.GetComponent<FoodManager>().healingAmount;
                playerCurrentHealth += healAmount;
                //Play the recover life SFX
                _playerHealthSFX.PlayOneShot(_playerRecoverHealthSFX, 0.3f);
                //Destroy the food
                Destroy(other.gameObject);
            }
            else     // If player is already at max health...
            {
                // Don't pickup and don't heal
                return;
            }
        }
    }
}
