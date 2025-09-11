using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health integers")]
    [SerializeField] private int _playerMaxHealth;      // Stores the max amount of health a player can have
    [SerializeField] private int _playerCurrentHealth;  // Stores how much health the player has currently

    private void Start()
    {
        // Start game at max health amount
        _playerCurrentHealth = _playerMaxHealth;
    }

    private void Update()
    {
        PreventFromExceeding();
    }

    void PreventFromExceeding()
    {
        // If the player heals more than the max health, set current health to max limit. 
        if (_playerCurrentHealth > _playerMaxHealth)
        {
            _playerCurrentHealth = _playerMaxHealth;
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Gets the damage value from the enemy that the player has collided with 
            int damageAmount = other.gameObject.GetComponent<EnemyManager>().enemyDamage;

            // Applies that damage amount to the player health
            _playerCurrentHealth -= damageAmount;
        }

        if (other.gameObject.CompareTag("Food"))
        {
            if (_playerCurrentHealth != _playerMaxHealth)   // If player has less than the max health amount...
            {
                // Apply heal and destroy food item
                int healAmount = other.gameObject.GetComponent<FoodManager>().healingAmount;
                _playerCurrentHealth += healAmount;
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
