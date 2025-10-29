using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float _maxBossHealth;                  // Stores the boss' max health value
    [SerializeField] float _currentBossHealth;              // Stores the boss' current health value
    [SerializeField] public bool playerDetected;

    [Header("UI")]
    public Slider _bossHealth;

    [Header("Ranged Attack")]
    [SerializeField] Transform _firingPoint;                // Stores the firing point where the boss's ink bullets will spawn from
    [SerializeField] GameObject _inkPrefab;                 // Stores the ink bullet prefab
    [SerializeField] Transform _target;                     // Gets the player's game object
    [SerializeField] float _spreadAngle;                    // Stores the angle at which the ink bullets will travel
    [SerializeField] float _inkSpeed;                       // Stores the speed at which the ink bullets will travel
    [SerializeField] float _rangedAttackCooldown;           // Stores the starting ranged attack cooldown timer at the start of the game
    [SerializeField] float _currentRangedAttackCooldown;    // Stores the current ranged attack cooldown timer during the game

    [Header("Melee Attack")]
    [SerializeField] GameObject _tentaclePrefab;                   // Stores the tentacle prefab
    [SerializeField] float _meleeAttackCooldown;                   // Stores the starting melee attack cooldown timer at the start of the game
    [SerializeField] float _currentMeleeAttackCooldown;            // Stores the current melee attack cooldown timer during the game
    [SerializeField] float _minTentacleDistance;                   // Handles offset between tentacle spawns
    [SerializeField] int _maxTentacleSpawn;                        // Stores the max amount of tentacles to be spawned during the attack
    [SerializeField] LayerMask _obstacleLayers;                    // Stores the obstacles layer to avoid tentacle spawns generating inside the colliders
    [SerializeField] bool _isUnderGround;
    private List<Vector3> _usedPositions = new List<Vector3>();    // Stores used positions during the generation of tentacle spawns to avoid overlapping spawn points
    private List<GameObject> _tentacles = new List<GameObject>();

    [Header("Animation")]
    [SerializeField] Animator _bossAnimator;
    [SerializeField] bool _meleeAttackStartAnim;
    [SerializeField] public bool meleeAttackLoopAnim;
    [SerializeField] public bool meleeAttackFinishAnim;
    [SerializeField] bool _rangedAttackAnim;
    [SerializeField] bool _bossDamaged;
    [SerializeField] bool _bossDying;

    void Start()
    {
        // Sets current ranged attack cooldown timer to starting cooldown timer
        _currentRangedAttackCooldown = _rangedAttackCooldown;

        // Sets current melee attack cooldown timer to starting cooldown timer
        _currentMeleeAttackCooldown = _meleeAttackCooldown;

        // Sets current boss health to the max health value
        _currentBossHealth = _maxBossHealth;

        _bossAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Checks both boss attacks cooldown timers and calls for whichever type of attack is available
        HandleAttackCooldowns();

        HandleBossAnimations();

        BossHealthUI();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Checks if boss collides with the spray
        if (other.gameObject.CompareTag("Spray") && !_isUnderGround && !_bossDying)
        {
            // Gets the damage value from the spray that the boss has collided with
            int sprayDamage = other.gameObject.GetComponent<SprayManager>().sprayDamage;

            // Apply damage to boss
            _currentBossHealth -= sprayDamage;

            _bossDamaged = true;

            Destroy(other.gameObject);

            // Checks boss's health
            BossDeath();
        }

        if (other.gameObject.CompareTag("Rock") && !_isUnderGround && !_bossDying)
        {
            // Gets the damage value from the rock that the boss has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage to boss
            _currentBossHealth -= rockDamageAmount;

            _bossAnimator.SetTrigger("Damaged");

            _bossDamaged = true;

            Destroy(other.gameObject);

            // Checks enemy's health
            BossDeath();
        }
    }

    void BossHealthUI()
    {
        _bossHealth.value = _currentBossHealth / _maxBossHealth;
    }

    void BossDeath()
    {
        // If boss' current health has reached 0...
        if (_currentBossHealth <= 0)
        {
            _bossAnimator.Play("Die");
            _bossDying = true;
            StartCoroutine(BossDeathSequence());
        }
    }

    private IEnumerator BossDeathSequence()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void HandleAttackCooldowns()
    {
        if (!playerDetected)
        {
            return;
        }

        if (_bossDying)
        {
            return;
        }

        // If ranged attack cooldown time is at zero...
        if (_currentRangedAttackCooldown <= 0)
        {
            _rangedAttackAnim = true;

            // And reset cooldown timer back to starting cooldown timer
            _currentRangedAttackCooldown = _rangedAttackCooldown;
        }
        // If cooldown time is over zero, then tick down to zero
        else
        {
            _rangedAttackAnim = false;
            _currentRangedAttackCooldown -= Time.deltaTime;
        }

        // If melee attack cooldown time is at zero...
        if (_currentMeleeAttackCooldown <= 0)
        {
            _bossAnimator.Play("Melee Attack Start");

            // And reset cooldown timer back to starting cooldown timer
            _currentMeleeAttackCooldown = _meleeAttackCooldown;
        }
        // If cooldown time is over zero, then tick down to zero
        else
        {
            _currentMeleeAttackCooldown -= Time.deltaTime;
        }
    }

    void RangedAttack()
    {
        // Calculates the player's direction
        //PROBLEMA ACA
        Vector2 targetPosition = (_target.transform.position - transform.position).normalized;

        // Creates a list of 3 directions for the ink bullets' trayectories, depending on the player's position, in different angles
        Vector2[] inkDirections = new Vector2[3];
        inkDirections[0] = targetPosition;
        inkDirections[1] = Quaternion.AngleAxis(_spreadAngle, Vector3.forward) * targetPosition;
        inkDirections[2] = Quaternion.AngleAxis(-_spreadAngle, Vector3.forward) * targetPosition;

        // For each direction calculated...
        foreach (Vector2 dir in inkDirections)
        {
            // Instantiates the ink bullet, starting from the boss' firing point
            var ink = Instantiate(_inkPrefab, _firingPoint.position, Quaternion.identity);

            // Applies speed to the ink bullet's linear velocity
            ink.GetComponent<Rigidbody2D>().linearVelocity = dir * _inkSpeed;

            // Destroys ink bullet after 2 secs
            Destroy(ink, 2f);
        }
    }

    void MeleeAttack()
    {
        // Resets list of previously generated spawn positions before attack 
        _usedPositions.Clear();

        // Sets tentacles spawned counter back to 0
        int tentaclesSpawned = 0;

            // While tentacles spawned is lower than the max amount of tentacles that should be spawned...
            while (tentaclesSpawned < _maxTentacleSpawn)
            {
                // Finds a random position inside a set range
                float x = Random.Range(-5.57f, 6.57f);
                float y = Random.Range(14.71f, 20.02f);
                Vector3 spawnPos = new Vector3(x, y, 0f);

                // Checks if the random position is overlapping with any obstacle's collider. If so, reset process back to start to find a new spawn position
                if (Physics2D.OverlapCircle((Vector2)spawnPos, 1f, _obstacleLayers))
                {
                    continue;
                }

                bool tentacleTooClose = false;

                // Checks if, inside previously generated spawn positions list, the random position is too close to another one
                foreach (var pos in _usedPositions)
                {
                    if (Vector3.Distance(pos, spawnPos) < _minTentacleDistance)
                    {
                        // If so, reset process back to start to find a new spawn position
                        tentacleTooClose = true;
                        break;
                    }
                }

                // If spawn position is valid...
                if (!tentacleTooClose)
                {
                    // Instantiates the tentacle in that spawn position
                    GameObject tentacle = Instantiate(_tentaclePrefab, spawnPos, Quaternion.identity);

                    SpriteRenderer tentacleSprite = GetComponent<SpriteRenderer>();
                    
                    tentacleSprite.flipX = Random.Range(0, 2) == 1;

                    // Destroys it after 5 secs
                    Destroy(tentacle, 5f);

                    // Adds spawn position to the list of previously generated spawn positions
                    _usedPositions.Add(spawnPos);

                    // Increase the counter up by 1
                    tentaclesSpawned++;
                }
            }  
        
        if (tentaclesSpawned >= _maxTentacleSpawn)
        {
            StartCoroutine(HandleBossReappearence());
        }
    }

    void HandleBossAnimations()
    {
        _bossAnimator.SetBool("Melee Attack Start", _meleeAttackStartAnim);
        _bossAnimator.SetBool("Melee Attack Finish", meleeAttackFinishAnim);
        _bossAnimator.SetBool("Ranged Attack", _rangedAttackAnim);
        _bossAnimator.SetBool("Die", _bossDying);
        _bossAnimator.SetBool("Damaged", _bossDamaged);
    }

    IEnumerator HandleBossReappearence()
    {
        yield return new WaitForSeconds(4.9f);
        _bossAnimator.Play("Melee Attack Finish");
    }
}
