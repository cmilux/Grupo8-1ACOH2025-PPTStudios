using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject _player;
    [SerializeField] PlayerMovement _playerAnimator;

    [Header("References")]
    [SerializeField] PathTest _meleeEnemyManager;
    [SerializeField] Image _healthBar;

    [Header("Health integers")]
    [SerializeField] int _playerMaxHealth;      // Stores the max amount of health a player can have
    [SerializeField] public int playerCurrentHealth;  // Stores how much health the player has currently

    [Header("Booleans")]
    [SerializeField] public bool activateInkSplatterEffect;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _playerHealth;


    private void Start()
    {
        // Start game at max health amount
        playerCurrentHealth = _playerMaxHealth;
        _playerAnimator = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        PreventFromExceeding();
        OnDeath();
        // SettingUI();
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
        if (playerCurrentHealth <= 0)
        {
            //Sets the player off the screen
            //_player.SetActive(false);

            //animation
            _playerAnimator._isDead = true;

            StartCoroutine(WaitnLoadScene());
            //Loads game over scene
            //SceneManager.LoadScene("GameOver");
        }
    }

    IEnumerator WaitnLoadScene()
    {
        yield return new WaitForSeconds(5f);

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

        // _healthBar.fillAmount = Mathf.Clamp(playerCurrentHealth / _playerMaxHealth, 0 ,1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Gets the damage value from the enemy that the player has collided with 
            int damageAmount = other.gameObject.GetComponent<PathTest>().enemyDamage;

            // Applies that damage amount to the player health
            playerCurrentHealth -= damageAmount;

            _playerAnimator._isBeingAttacked = true;
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

            _playerAnimator._isBeingAttacked = true;
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
