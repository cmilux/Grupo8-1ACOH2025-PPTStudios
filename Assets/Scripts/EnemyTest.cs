using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int enemyDamage;       // Stores how much damage the enemy can do to the player
    [SerializeField] public int enemyHealth;     // Stores how much health the enemy has

    private void Update()
    {
        if (enemyHealth <= 0)   // If the enemy has no more health...
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickups"))
        {
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage and destroy rock
            enemyHealth -= rockDamageAmount;
            Destroy(other.gameObject);
        }
    }
}
