using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class RangedEnemyManager : MonoBehaviour
{
    [Header("Pathfinding AI")]
    [SerializeField] Transform _target;               // Gets target's transform
    [SerializeField] NavMeshAgent _agent;             // Gets the agent (enemy) NavMesh

    [Header("Patrol logic")]
    [SerializeField] float _patrolRadius;             // Sets the radius of the patrol area for the enemy
    [SerializeField] bool _enemyWaiting;              // Checks whether the enemy is waiting to go to the next patrol point or not
    [SerializeField] bool _playerDetected;            // Checks whether the enemy has detected the player or not

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
    [SerializeField] GameObject inkSplatter;               // Gets the image for the ink splatter effect
    [SerializeField] PlayerHealth _playerHealth;           // Gets the player health script that calls for the ink splatter effect
    [SerializeField] TextMeshProUGUI _enemy2Health;        // Displays enemy health in UI
    [SerializeField] Slider _healthBar;                    // Gets the enemy's health bar

    [Header("Animation")]
    [SerializeField] Animator _NPCAnimator;           // Reference to the NPC's Animator component
    [SerializeField] Animator _alienAnimator;         // Reference to the alien's Animator component
    [SerializeField] bool _playAttackAnimation;       // Checks if alien can play the attack animation
    [SerializeField] Vector2 _lastDir;                // Stores the last direction the enemy has moved in
    [SerializeField] bool _isMoving;                  // Checks whether the enemy is moving or not
    [SerializeField] bool _enemyDying;                // Checks whether the enemy is moving or not

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
        Time.timeScale = 1;

        if (/*scene.name == "Zone1" ||*/ scene.name == "Zone2" || scene.name == "Zone3")
        {
            _playerHealth = PlayerManager.Instance.GetComponentInChildren<PlayerHealth>();
            //_playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
            _target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Start()
    {
        // Gets the NavMeshAgent from the enemy
        _agent = GetComponent<NavMeshAgent>();

        // Sets current attack cooldown timer to starting cooldown timer
        _currentAttackCooldown = _attackCooldown;

        // Sets current enemy health to the max health value
        _currentEnemyHealth = _enemyHealth;

        //-------------------
        //2D NavMesh settings
        //-------------------

        // Should the agent rotate?
        _agent.updateRotation = false;
        // Should the agent movement ignore the vertical axis?
        _agent.updateUpAxis = false;

        /*
        if (_target == null)
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                _target = playerGO.transform;
            }
            else
            {
                Debug.Log($"{name}:Player not found");
            }
        }*/

        UpdatePatrolPoint();
    }

    void Update()
    {
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

        // Updates health bar above enemy's head to the current health value 
        UpdateHealthBar();
 
        //SettingUI();

        // When the player is attacked by a ranged enemy, call for the ink splatter effect to be activated 
        //if (_playerHealth.activateInkSplatterEffect == true)
        //{
        //    StartCoroutine(InkSplatterEffect());
        //}

        // Until player is detected, activate patrol state
        if (!_playerDetected)
        {
            HandlePatrolState();
        }
        // When player is detected, activate follow state
        else if (_playerDetected)
        {
            HandleFollowState();
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
            EnemyDistanceDamage(other);
        }
        
        // Checks if enemy collides with the spray
        else if (other.gameObject.CompareTag("Spray"))
        {
            EnemyMeleeDamage(other);
        }
    }

    private void EnemyDeath()
    {
        // Checks the enemy's health
        if (_currentEnemyHealth <= 0)
        {
            StartCoroutine(EnemyDeathSequence());
            _enemyDying = true;
            _agent.isStopped = true;
        }
    }

    private IEnumerator EnemyDeathSequence()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
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

        if (distance <= 5f)
        {
            _playerDetected = true;
        }
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
            var ink = Instantiate(_inkPrefab, _firingPoint.position, Quaternion.identity);

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
        // Calculates the direction of the player
        Vector2 direction = _agent.transform.position - _firingPoint.position;

        // Calculates the angle in which the ink bullet should be fired
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotates the firing point towards that angle
        _firingPoint.rotation = Quaternion.Euler(0f, 0f, angle);

        // Orbits the firing point around the NPC sprite
        _firingPoint.RotateAround(transform.position, Vector3.forward, _rotateSpeed * Time.deltaTime);
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

        if (!_enemyWaiting && _agent.hasPath && _agent.velocity.sqrMagnitude > 0.05f)
        {
            _isMoving = true;
        }
        else _isMoving = false;

        _NPCAnimator.SetBool("Moving", _isMoving);
        _NPCAnimator.SetBool("Die", _enemyDying);
    }

    private void HandleAlienSprites()
    {
        _alienAnimator.SetFloat("Horizontal", _lastDir.x);
        _alienAnimator.SetFloat("Vertical", _lastDir.y);
        _alienAnimator.SetBool("Attack", _playAttackAnimation);
        _alienAnimator.SetBool("Die", _enemyDying);
    }

    public IEnumerator InkSplatterEffect()
    {
        // Sets ink splatter effect active for a few seconds, then makes it inactive again
        if (inkSplatter != null)
        {  
            inkSplatter.SetActive(true);
            yield return new WaitForSeconds(3f);
            inkSplatter.SetActive(false);
            _playerHealth.activateInkSplatterEffect = false;
        }
    }

    private void UpdateHealthBar()
    {
        _healthBar.value = _currentEnemyHealth / _enemyHealth;
    }

    private void HandlePatrolState()
    {
        if (_agent.pathPending || _enemyWaiting)
        {
            return;
        }

        if (!_agent.hasPath || _agent.remainingDistance <= _agent.stoppingDistance)
        {
            StartCoroutine(WaitBeforeMoving());
        }
    }

    private void UpdatePatrolPoint()
    {
        Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * _patrolRadius;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, _patrolRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
        else
        {
            return;
        }
    }

    private IEnumerator WaitBeforeMoving()
    {
        _enemyWaiting = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(1f);

        UpdatePatrolPoint();

        _agent.isStopped = false;
        _enemyWaiting = false;
    }

    private void HandleFollowState()
    {
        // Sets the enemy's target to the player
        _agent.SetDestination(_target.position);
    }

    private void EnemyDistanceDamage(Collider2D rock)
    {
        // Gets the damage value from the rock that the enemy has collided with
        int rockDamageAmount = rock.gameObject.GetComponent<RockManager>().rockDamage;

        // Apply damage to enemy
        _currentEnemyHealth -= rockDamageAmount;

        Destroy(rock.gameObject);

        _alienAnimator.SetTrigger("Damaged");

        _playerDetected = true;

        // Checks enemy's health
        EnemyDeath();
    }

    private void EnemyMeleeDamage(Collider2D spray)
    {
        // Gets the damage value from the spray that the enemy has collided with
        int sprayDamage = spray.gameObject.GetComponent<SprayManager>().sprayDamage;

        // Apply damage to enemy
        _currentEnemyHealth -= sprayDamage;

        _alienAnimator.SetTrigger("Damaged");

        // Checks enemy's health
        EnemyDeath();
    }
}
