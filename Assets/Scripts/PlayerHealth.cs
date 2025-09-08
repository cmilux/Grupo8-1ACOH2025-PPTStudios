using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _playerHealth;      // Stores the player health

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Gets the damage value from the enemy that the player has collided with 
            int damageAmount = other.gameObject.GetComponent<EnemyTest>().enemyDamage;

            // Applies that damage amount to the player health
            _playerHealth -= damageAmount;
        }
    }
}
