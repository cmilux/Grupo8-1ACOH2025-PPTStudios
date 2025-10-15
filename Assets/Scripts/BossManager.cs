using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float _maxBossHealth;
    [SerializeField] float _currentBossHealth;                

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

        Vector2[] inkDirections = new Vector2[3];
        inkDirections[0] = targetPosition;
        inkDirections[1] = Quaternion.AngleAxis(_spreadAngle, Vector3.forward) * targetPosition;
        inkDirections[2] = Quaternion.AngleAxis(-_spreadAngle, Vector3.forward) * targetPosition;

        foreach (Vector2 dir in inkDirections)
        {
            // Instancia la bala en el punto de disparo
            var ink = Instantiate(_inkPrefab, _firingPoint.position, Quaternion.identity);

            // Le da velocidad en la dirección correspondiente
            ink.GetComponent<Rigidbody2D>().linearVelocity = dir * _inkSpeed;

            // Destruye la bala luego de 2 segundos
            Destroy(ink, 2f);
        }
    }

    void MeleeAttack()
    {
        _usedPositions.Clear();
        int tentaclesSpawned = 0;

        while (tentaclesSpawned < _maxTentacleSpawn)
        {
            float x = Random.Range(-6.91f, 7.25f);
            float y = Random.Range(12.66f, 19.89f);
            Vector3 spawnPos = new Vector3(x, y, 0f);

            if (Physics2D.OverlapCircle((Vector2)spawnPos, 1f, _colliderLayers))
            {
                continue;
            }

            bool tentacleTooClose = false;

            foreach (var pos in _usedPositions)
            {
                if (Vector3.Distance(pos, spawnPos) < _minTentacleDistance)
                {
                    tentacleTooClose = true;
                    break;
                }
            }

            if (!tentacleTooClose)
            {
                var tentacle = Instantiate(_tentaclePrefab, spawnPos, Quaternion.identity);
                Destroy(tentacle, 5f); 
                _usedPositions.Add(spawnPos);
                tentaclesSpawned++;
            }
        }
    }
}
