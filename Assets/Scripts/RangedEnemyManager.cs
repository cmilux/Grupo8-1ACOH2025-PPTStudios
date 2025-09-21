using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyManager : MonoBehaviour
{
    [Header("Pathfinding AI")]
    [SerializeField] Transform _target;               // Gets target's transform
    [SerializeField] NavMeshAgent _agent;             // Gets the agent (enemy) NavMesh

    [Header("Attack logic")]
    [SerializeField] bool _canAttack;                 // Checks whether enemy is able to attack or not
    [SerializeField] float _attackThreshold;          // Stores the enemy's distance threshold to attack 
    [SerializeField] float _attackCooldown;           // Stores the starting attack cooldown timer at the start of the game
    [SerializeField] float _currentAttackCooldown;    // Stores the current attack cooldown timer during the game

    [Header("Bullet variables")]
    [SerializeField] Transform _firingPoint;          // Stores the firing point where the enemy's ink will spawn from
    [SerializeField] GameObject _inkPrefab;           // Stores the ink bullet prefab
    [SerializeField] float _inkSpeed;                 // Stores the speed at which the ink bullet will travel

    [Header("Stats")]
    [SerializeField] int _enemyHealth;                // Stores how much health the enemy has
    [SerializeField] float _rotateSpeed;              // Handles the speed at which the enemy rotates towards the player


    void Start()
    {
        // Gets the NavMeshAgent from the enemy
        _agent = GetComponent<NavMeshAgent>();

        //-------------------
        //2D NavMesh settings
        //-------------------

        // Should the agent rotate?
        _agent.updateRotation = false;
        // Should the agent movement ignore the vertical axis?
        _agent.updateUpAxis = false;

        // Sets current attack cooldown timer to starting cooldown timer
        _currentAttackCooldown = _attackCooldown;
    }

    void Update()
    {
        // Follows player
        _agent.SetDestination(_target.position);

        // Checks distance with player to either stop or throw ink 
        CheckDistanceWithPlayer();

        // Checks whether enemy can attack or if its attack is still in cooldown
        HandleAttackCooldown();

        // Makes the enemy rotate towards the player's position, so that the firing point is always facing the player when shooting
        RotateTowardsPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Checks if enemy collides with a rock
        if (other.gameObject.CompareTag("Rock"))
        {
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage to enemy
            _enemyHealth -= rockDamageAmount;

            // Checks enemy's health
            EnemyDeath();
        }

        // Checks if enemy collides with the spray
        else if (other.gameObject.CompareTag("Spray"))
        {
            // Gets the damage value from the spray that the enemy has collided with
            int sprayDamage = other.gameObject.GetComponent<SprayManager>().sprayDamage;

            // Apply damage to enemy
            _enemyHealth -= sprayDamage;

            // Checks enemy's health
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        // Checks the enemy's health
        if (_enemyHealth <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }

    private void CheckDistanceWithPlayer()
    {
        // Calculates distance between enemy and player
        float distance = Vector2.Distance(transform.position, _target.transform.position);

        // If distance between enemy and player is under the attack threshold...
        if (distance <= _agent.stoppingDistance)
        {
            // Enemy is able to attack
            _canAttack = true;
        }
        else _canAttack = false;
    }

    private void HandleAttackCooldown()
    {
        // If cooldown time is at zero...
        if (_currentAttackCooldown <= 0)
        {
            // Attack!
            ThrowInk();

            // And reset cooldown timer back to starting cooldown timer
            _currentAttackCooldown = _attackCooldown;
        }
        // If cooldown time is over zero, then tick down to zero
        else _currentAttackCooldown -= Time.deltaTime;
    }

    private void ThrowInk()
    {
        // If enemy is able to attack and "HandleAttackCooldown()" has called for the enemy to attack...
        if (_canAttack == true)
        {
            // Instantiates the ink prefab at the enemy's firing point's position and rotation
            var ink = Instantiate(_inkPrefab, _firingPoint.position, _firingPoint.rotation);

            // Calculates the player's direction
            Vector2 direction = (_target.transform.position - transform.position).normalized;

            // Gets the bullet's rigidbody and applies velocity towards the player's direction at a certain speed
            ink.GetComponent<Rigidbody2D>().linearVelocity = direction * _inkSpeed;

            // If the bullet hasn´t hit any colliders, destroy it after a few seconds
            Destroy(ink, 2f);
        }
    }

    // SELE: Todo este void tengo que consultarlo con el profe porque si bien funciona, no termino de entender algunas funciones ;_; 
    private void RotateTowardsPlayer()
    { 
        // Calculates the player's direction
        Vector2 targetPosition = _target.transform.position - transform.position;

        // Calculates the angle at which the player's at in relation to the enemy and applies an offset so it rotates correctly
        float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg - 90f;

        // Rotates the firing point towards the angle
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
