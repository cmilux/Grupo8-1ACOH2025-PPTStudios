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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Zone1")
        {
            playerCurrentHealth = _playerMaxHealth;
            _playerAnimator = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
            _playerAnimator._isDead = false;
        }

        //string sceneName = SceneManager.GetActiveScene().name;
        if (scene.name == "Zone1" || scene.name == "Zone2" || scene.name == "Zone3")
        {
            _playerHealthSFX = GetComponent<AudioSource>();
            _healthBar = GameObject.Find("PlayerHealthUI")?.GetComponent<Slider>();
            //if (_healthBar == null) 
            //{
            //    Debug.Log("Wasnt Found (H)");                    
            //}
            SettingUI();
        }
    }

    private void Update()
    {
        PreventFromExceeding();
        OnDeath();
        SettingUI();
    }

    void PreventFromExceeding()
    {
        // If the player heals more than the max health, set current health to max limit. 
        if (playerCurrentHealth >= _playerMaxHealth)
        {
            playerCurrentHealth = _playerMaxHealth;
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
            StartCoroutine(WaitnLoadScene());
        }
    }

    IEnumerator WaitnLoadScene()
    {
        yield return new WaitForSeconds(2.2f);

        SceneManager.LoadScene("GameOver");
    }

    IEnumerator WaitForAnimationToEnd()
    {
        yield return new WaitForSeconds(1.5f);
        _playerAnimator._isBeingAttacked = false;
    }
    void SettingUI()
    {
        //_playerHealth.SetText($"Health: {playerCurrentHealth}");

        //_healthBar.fillAmount = Mathf.Clamp(playerCurrentHealth / _playerMaxHealth, 0 ,1);
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

            StartCoroutine(WaitForAnimationToEnd());

            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Food"))
        {
            if (playerCurrentHealth != _playerMaxHealth)   // If player has less than the max health amount...
            {
                // Apply heal and destroy food item
                int healAmount = other.gameObject.GetComponent<FoodManager>().healingAmount;
                playerCurrentHealth += healAmount;

                _playerHealthSFX.PlayOneShot(_playerRecoverHealthSFX, 0.3f);

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
