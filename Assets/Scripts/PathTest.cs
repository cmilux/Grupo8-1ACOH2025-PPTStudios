using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PathTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _target;               // Gets target transform
    [SerializeField] NavMeshAgent _agent;             // Gets the agent (enemy) NavMesh
    [SerializeField] AlienManager _hitboxManager;     // Gets the HitboxManager script from the alien

    [Header("Patrol logic")]
    [SerializeField] float _patrolRadius;             // Sets the radius of the patrol area for the enemy
    [SerializeField] bool _enemyWaiting;              // Checks whether the enemy is waiting to go to the next patrol point or not
    [SerializeField] bool _playerDetected;            // Checks whether the enemy has detected the player or not

    [Header("Variables")]
    [SerializeField] float _enemyHealth = 100;        // Stores how much health the enemy has
    [SerializeField] float _currentEnemyHealth;       // Stores how much health the enemy currently has
    [SerializeField] public int enemyDamage;          // Stores how much damage the enemy can do to the player
    [SerializeField] public bool canAttack = false;   // Checks if enemy is in range to attack

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _enemy1Health;   // Displays enemy health in UI
    [SerializeField] Slider _healthBar;               // Gets the enemy's health bar

    [Header("Animation")]
    [SerializeField] Animator _NPCAnimator;           // Reference to the NPC's Animator component
    [SerializeField] Animator _alienAnimator;         // Reference to the alien's Animator component
    [SerializeField] Vector2 _lastDir;                // Stores the last direction the enemy has moved in
    [SerializeField] bool _attackAnimation = false;   // Checks whether the enemy's attack animation should be activated or not
    [SerializeField] bool _isMoving;
    [SerializeField] public bool enemyDying = false;
    [SerializeField] bool _enemyDamaged;

    [Header("Attack Cooldown")]
    [SerializeField] public float attackCooldown;          // Stores the enemy's max attack cooldown timer 
    [SerializeField] public float currentAttackCooldown;   // Stores the enemy's current attack cooldown timer 
    [SerializeField] public bool attackReady = false;      // Checks whether the enemy's attack is on cooldown or not

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Time.timeScale = 1;

    //    if (/*scene.name == "Zone1" ||*/ scene.name == "Zone2" || scene.name == "Zone3")
    //    {
    //        _target = PlayerMovement.Instance.gameObject.transform;
    //    }
    //}

    void Start()
    {
        // Gets the NavMeshAgent from the enemy
        _agent = GetComponent<NavMeshAgent>();

        // Sets current attack cooldown timer to starting cooldown timer
        currentAttackCooldown = attackCooldown;

        // Sets current enemy health to the max health value
        _currentEnemyHealth = _enemyHealth;

        _target = GameObject.FindGameObjectWithTag("Player").transform;

        //-------------------
        //2D NavMesh settings
        //-------------------
        //Should the agent rotate
        _agent.updateRotation = false;
        //Should the agent movement ignore the vertical axis
        _agent.updateUpAxis = false;

        if (_target == null)
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                _target = playerGO.transform;
            }
            else{Debug.Log($"{name}: Player not found on Start()._target remains null");}
        }

        UpdatePatrolPoint();
    }

    void Update()
    {
        // Stores the enemy's last direction while moving
        StoreLastMove();

        // Changes enemy's sprites depending on their axes of movement (handled by StoreLastMove())
        HandleNPCSprites();

        // Changes alien's sprites depending on their axes of movement (handled by StoreLastMove()) and if it's able to attack
        HandleAlienSprites();

        // Checks distance with player to set "_canAttack" to either true or false
        DetectDistanceWithPlayer();

        // Checks whether enemy can attack or if its attack is still in cooldown
        HandleAttackCooldown();

        // Updates health bar above enemy's head to the current health value 
        UpdateHealthBar();

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

        //SettingUI();
    }

    void SettingUI()
    {
        _enemy1Health.SetText($"Healht: {_currentEnemyHealth}");
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
        _enemyDamaged = true;
        //Checks the enemy's health
        if (_currentEnemyHealth <= 0)
        {
            StartCoroutine(EnemyDeathSequence());
            enemyDying = true;
            _agent.isStopped = true;
        }
    }

    private IEnumerator EnemyDeathSequence()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
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

    private void DetectDistanceWithPlayer()
    {
        float distance = Vector2.Distance(transform.position, _target.transform.position);

        if (distance <= _agent.stoppingDistance)
        {
            canAttack = true;
        }
        else canAttack = false;

        if (distance <= 3f)
        {
            _playerDetected = true;
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

        _NPCAnimator.SetBool("isMoving", _isMoving);
        _NPCAnimator.SetBool("isDying", enemyDying);
    }

    private void HandleAlienSprites()
    {
        _alienAnimator.SetFloat("Horizontal", _lastDir.x);
        _alienAnimator.SetFloat("Vertical", _lastDir.y);
        _alienAnimator.SetBool("Attack", _attackAnimation);

        if (canAttack)
        {
            _attackAnimation = true;
        }
        else _attackAnimation = false;

        _alienAnimator.SetBool("Die", enemyDying);
    }

    private void HandleAttackCooldown()
    {
        if (attackReady == false)
        {
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown <= 0)
            {
                attackReady = true;
            }
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
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * _patrolRadius;
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

        _alienAnimator.SetTrigger("Damaged");

        _playerDetected = true;

        //Checks enemy's health
        EnemyDeath();
    }

    private void EnemyMeleeDamage(Collider2D spray)
    {
        // Gets the damage value from the spray that the enemy has collided with
        int sprayDamage = spray.gameObject.GetComponent<SprayManager>().sprayDamage;

        // Apply damage to enemy
        _currentEnemyHealth -= sprayDamage;

        _alienAnimator.SetTrigger("Damaged");

        _playerDetected = true;  

        //Checks enemy's health
        EnemyDeath();
    }
}
