using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float _maxBossHealth;                  // Stores the boss' max health value
    [SerializeField] float _currentBossHealth;              // Stores the boss' current health value

    [Header("Ranged Attack")]
    [SerializeField] Transform _firingPoint;                // Stores the firing point where the boss's ink bullets will spawn from
    [SerializeField] GameObject _inkPrefab;                 // Stores the ink bullet prefab
    [SerializeField] GameObject _target;                    // Gets the player's game object
    [SerializeField] float _spreadAngle;                    // Stores the angle at which the ink bullets will travel
    [SerializeField] float _inkSpeed;                       // Stores the speed at which the ink bullets will travel
    [SerializeField] float _rangedAttackCooldown;           // Stores the starting ranged attack cooldown timer at the start of the game
    [SerializeField] float _currentRangedAttackCooldown;    // Stores the current ranged attack cooldown timer during the game

    [Header("Melee Attack")] 
    [SerializeField] GameObject _tentaclePrefab;                  // Stores the tentacle prefab
    [SerializeField] float _meleeAttackCooldown;                  // Stores the starting melee attack cooldown timer at the start of the game
    [SerializeField] float _currentMeleeAttackCooldown;           // Stores the current melee attack cooldown timer during the game
    [SerializeField] float _minTentacleDistance;                  // Handles offset between tentacle spawns
    [SerializeField] int _maxTentacleSpawn;                       // Stores the max amount of tentacles to be spawned during the attack
    [SerializeField] LayerMask _colliderLayers;                   // Stores layers with obstacles to avoid tentacle spawns generating inside the colliders
    private List<Vector3> _usedPositions = new List<Vector3>();   // Stores used positions during the generation of tentacle spawns to avoid overlapping spawn points
    

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");

        // Sets current ranged attack cooldown timer to starting cooldown timer
        _currentRangedAttackCooldown = _rangedAttackCooldown;

        // Sets current melee attack cooldown timer to starting cooldown timer
        _currentMeleeAttackCooldown = _meleeAttackCooldown;

        // Sets current boss health to the max health value
        _currentBossHealth = _maxBossHealth;
    }

    private void Update()
    {
        // Checks both boss attacks cooldown timers and calls for whichever type of attack is available
        HandleAttackCooldowns();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Checks if boss collides with the spray
        if (other.gameObject.CompareTag("Spray"))
        {
            // Gets the damage value from the spray that the boss has collided with
            int sprayDamage = other.gameObject.GetComponent<SprayManager>().sprayDamage;

            // Apply damage to boss
            _currentBossHealth -= sprayDamage;

            // Checks boss's health
            BossDeath();
        }

        if (other.gameObject.CompareTag("Rock"))
        {
            // Gets the damage value from the rock that the boss has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage to boss
            _currentBossHealth -= rockDamageAmount;

            // Checks enemy's health
            BossDeath();
        }
    }

    void BossDeath()
    {
        // If boss' current health has reached 0...
        if (_currentBossHealth <= 0)
        {
            // Destroy boss
            Destroy(gameObject);
        }
    }

    private void HandleAttackCooldowns()
    {
        // If ranged attack cooldown time is at zero...
        if (_currentRangedAttackCooldown <= 0)
        {
            // Attack!
            RangedAttack();

            // And reset cooldown timer back to starting cooldown timer
            _currentRangedAttackCooldown = _rangedAttackCooldown;
        }
        // If cooldown time is over zero, then tick down to zero
        else
        {
            _currentRangedAttackCooldown -= Time.deltaTime;
        }

        // If melee attack cooldown time is at zero...
        if (_currentMeleeAttackCooldown <= 0)
        {
            // Attack!
            MeleeAttack();

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
            float x = Random.Range(-6.91f, 7.25f);
            float y = Random.Range(12.66f, 19.89f);
            Vector3 spawnPos = new Vector3(x, y, 0f);

            // Checks if the random position is overlapping with any obstacle's collider. If so, reset process back to start to find a new spawn position
            if (Physics2D.OverlapCircle((Vector2)spawnPos, 1f, _colliderLayers))
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
                var tentacle = Instantiate(_tentaclePrefab, spawnPos, Quaternion.identity);

                // Destroys it after 5 secs
                Destroy(tentacle, 5f);

                // Adds spawn position to the list of previously generated spawn positions
                _usedPositions.Add(spawnPos);

                // Increase the counter up by 1
                tentaclesSpawned++;
            }
        }
    }
}
