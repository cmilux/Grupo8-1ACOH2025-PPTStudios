using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RangedEnemyManager : MonoBehaviour
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
    [SerializeField] bool _playerDetected = false;    // Stores whether the player has entered the enemy's detection range or not
    [SerializeField] float _detectionRange;
    [SerializeField] float _moveAwayRange;
    [SerializeField] bool _farAwayFromTarget;
    [SerializeField] bool _targetInRange;
    [SerializeField] bool _tooCloseToTarget;

    Rigidbody2D _rb;

    private void Start()
    { 
        currentAttackCooldown = startingAttackCooldown;
        _rb = GetComponent<Rigidbody2D>();

        if (_target == null)
        {
            _target = GameObject.FindGameObjectWithTag("Player");

        }

        if (_playerHealth == null)
        {
            _playerHealth = GetComponentInChildren<PlayerHealth>();
        }
    }

    private void Update()
    {
        EnemyDeath();
        DetectDistanceWithPlayer();
        currentAttackCooldown -= 1 * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_playerDetected == true && _farAwayFromTarget == true)   // If enemy has detected the player and isn't in range, move until inside range
        {
            // Calculates next position for the enemy to move towards
            Vector2 direction = (_target.transform.position - transform.position).normalized;

            // Move towards that position
            _rb.linearVelocity = new Vector2(direction.x, direction.y) * _speed * Time.deltaTime;
        }
        
        if (_playerDetected == true && _targetInRange == true)
        {
            if (currentAttackCooldown <= 0)
            {
                AttackPlayer();
            }
        }      
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _targetInRange = true;
        }
        else _targetInRange = false;
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
        Vector2 direction = (transform.position - _target.transform.position).normalized;

        if (distance < _detectionRange)
        {
            _playerDetected = true;
        }

        // If player is too close and enemy can't do ranged attack...
        if (distance < _moveAwayRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, _speed * Time.deltaTime);
        }
        else _farAwayFromTarget = true;
    }

    void AttackPlayer()
    {
        // Apply damage to player's current health value
        _playerHealth.playerCurrentHealth -= enemyDamage;

        // Reset attack cooldown
        currentAttackCooldown = startingAttackCooldown;
    }
}

