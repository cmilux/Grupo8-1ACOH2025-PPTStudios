using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

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
    [SerializeField] float _enemyHealth;              // Stores how much health the enemy has
    [SerializeField] float _currentEnemyHealth;       // Stores how much health the enemy currently has
    [SerializeField] float _rotateSpeed;              // Handles the speed at which the enemy rotates towards the player

    [Header("UI")]
    [SerializeField] GameObject inkSplatter;          // Gets the image for the ink splatter effect
    [SerializeField] PlayerHealth _playerHealth;      // Gets the player health script that calls for the ink splatter effect
    [SerializeField] TextMeshProUGUI _enemy2Health;   // Displays enemy health in UI
    [SerializeField] Slider _healthBar;

    [Header("Animation")]
    [SerializeField] Animator _NPCAnimator;           // Reference to the NPC's Animator component
    [SerializeField] Animator _alienAnimator;         // Reference to the alien's Animator component
    [SerializeField] bool _playAttackAnimation;       // Checks if alien can play the attack animation
    [SerializeField] Vector2 _lastDir;                // Stores the last direction the enemy has moved in
    [SerializeField] bool _enemyDamaged;

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

        _currentEnemyHealth = _enemyHealth;
    }

    void Update()
    {
        // Follows player
        _agent.SetDestination(_target.position);

        // Checks distance with player to either stop or throw ink 
        CheckDistanceWithPlayer();

        // Checks whether enemy can attack or if its attack is still in cooldown
        HandleAttackCooldown();

        // Makes the firing point rotate towards the player's position, so that it's always facing the player when shooting
        RotateFiringPoint();

        // Stores the enemy's last direction while moving
        StoreLastMove();

        // Changes enemy's sprites depending on their axes of movement (handled by StoreLastMove())
        HandleNPCSprites();

        // Changes alien's sprites depending on their axes of movement (handled by StoreLastMove()) and if it's able to attack
        HandleAlienSprites();

        UpdateHealthBar();

        SettingUI();

        if (_playerHealth.activateInkSplatterEffect == true)
        {
            StartCoroutine(InkSplatterEffect());
        }
    }

    void SettingUI()
    {
        _enemy2Health.SetText($"Health: {_currentEnemyHealth}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Checks if enemy collides with a rock
        if (other.gameObject.CompareTag("Rock"))
        {
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage to enemy
            _currentEnemyHealth -= rockDamageAmount;

            _enemyDamaged = true;

            // Checks enemy's health
            EnemyDeath();
        }
        // Checks if enemy collides with the spray
        else if (other.gameObject.CompareTag("Spray"))
        {
            // Gets the damage value from the spray that the enemy has collided with
            int sprayDamage = other.gameObject.GetComponent<SprayManager>().sprayDamage;

            // Apply damage to enemy
            _currentEnemyHealth -= sprayDamage;

            _enemyDamaged = true;

            // Checks enemy's health
            EnemyDeath();
        }
        else _enemyDamaged = false;
    }

    private void EnemyDeath()
    {
        // Checks the enemy's health
        if (_currentEnemyHealth <= 0)
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
        else
        {
            _currentAttackCooldown -= Time.deltaTime;
            _playAttackAnimation = false;
        }
    }

    private void ThrowInk()
    {
        // If enemy is able to attack and "HandleAttackCooldown()" has called for the enemy to attack...
        if (_canAttack == true)
        {
            // Plays the alien's attack animation
            _playAttackAnimation = true;

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

    private void RotateFiringPoint()
    {
        // Points firing point to player's position
        _firingPoint.LookAt(_agent.transform.position);

        // Makes firing point rotate around the enemy
        _firingPoint.transform.RotateAround(transform.position, Vector3.up, _rotateSpeed * Time.deltaTime);
    }

    private void StoreLastMove()
    {
        // Saves the enemy's last movement direction
        Vector2 moveDir = new Vector2(_agent.velocity.y, _agent.velocity.x).normalized;

        if (moveDir != Vector2.zero)
        {
            _lastDir = moveDir;
        }
    }

    private void HandleNPCSprites()
    {
        _NPCAnimator.SetFloat("Horizontal", _lastDir.x);
        _NPCAnimator.SetFloat("Vertical", _lastDir.y);
    }

    private void HandleAlienSprites()
    {
        _alienAnimator.SetFloat("Horizontal", _lastDir.x);
        _alienAnimator.SetFloat("Vertical", _lastDir.y);
        _alienAnimator.SetBool("Attack", _playAttackAnimation);
        _alienAnimator.SetBool("Damage", _enemyDamaged);
    }

    public IEnumerator InkSplatterEffect()
    {
        // Sets ink splatter effect active for a few seconds, then makes it inactive again
        inkSplatter.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        inkSplatter.SetActive(false);
        _playerHealth.activateInkSplatterEffect = false;
    }

    private void UpdateHealthBar()
    {
        _healthBar.value = _currentEnemyHealth / _enemyHealth;
    }
}
