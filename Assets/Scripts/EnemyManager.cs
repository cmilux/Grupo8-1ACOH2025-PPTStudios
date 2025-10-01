using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int enemyDamage;                // Stores how much damage the enemy can do to the player
    [SerializeField] public int enemyHealth;                // Stores how much health the enemy has
    [SerializeField] public float startingAttackCooldown;   // Handles enemy's attack cooldown time
    [SerializeField] public float currentAttackCooldown;           // Handles enemy's current attack cooldown time

    [Header("Follow Logic")]
    [SerializeField] GameObject _target;                    // Stores target for the enemy to follow (player)
    [SerializeField] PlayerHealth _playerHealth;            // Reference to "PlayerHealth" script
    [SerializeField] float _speed;                          // Speed at which the enemy moves towards target
    [SerializeField] public bool playerDetected = false;    // Stores whether the player has entered the enemy's detection range or not
    [SerializeField] public float detectionRange;           

    Rigidbody2D _rb;

    private void Start()
    {
        currentAttackCooldown = startingAttackCooldown;
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.Find("Player");
    }

    private void Update()
    {
        EnemyDeath();
        DetectDistanceWithPlayer();
        currentAttackCooldown -= 1 * Time.deltaTime;

        if (currentAttackCooldown <= 0)
        {
            AttackPlayer();
        }
        else return;
    }

    private void FixedUpdate()
    {
        if (playerDetected == true)
        {
            // Calculates next position for the enemy to move towards
            Vector2 direction = (_target.transform.position - transform.position).normalized;

            // Move towards that position
            _rb.linearVelocity = new Vector2(direction.x, direction.y) * _speed * Time.deltaTime;
        }
        else return;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Rock") || other.gameObject.CompareTag("Spray"))
        {
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage and destroy rock
            enemyHealth -= rockDamageAmount;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // If enemy is on range of the player, pause movement and attack
        if (other.gameObject.CompareTag("Player"))
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        } 
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If player is off range, resume movement until on range to attack again
        if (other.gameObject.CompareTag("Player"))
        {
            _rb.constraints = RigidbodyConstraints2D.None;
        }
    }

    private void EnemyDeath()
    {
        // If the enemy has no more health...
        if (enemyHealth <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }

    private void DetectDistanceWithPlayer()
    {
        float distance = Vector2.Distance(transform.position, _target.transform.position);
        
        if (distance < detectionRange)
        {
            playerDetected = true;
        }
    }

    void AttackPlayer()
    {
        // Apply damage to player's current health value
        _playerHealth.playerCurrentHealth -= enemyDamage;

        // Reset attack cooldown
        currentAttackCooldown = startingAttackCooldown;
    }
}

