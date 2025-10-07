using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PathTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _target;               // Gets target transform
    [SerializeField] NavMeshAgent _agent;             // Gets the agent (enemy) NavMesh
    [SerializeField] HitboxManager _hitboxManager;    // Gets the HitboxManager script from the alien

    [Header("Variables")]
    [SerializeField] float _enemyHealth = 100;          // Stores how much health the enemy has
    [SerializeField] float _currentEnemyHealth;         // Stores how much health the enemy currently has
    [SerializeField] public int enemyDamage;          // Stores how much damage the enemy can do to the player
    [SerializeField] public bool canAttack = false;   // Checks if enemy is in range to attack

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _enemy1Health;   // Displays enemy health in UI
    [SerializeField] Slider _healthBar;

    [Header("Animation")]
    [SerializeField] Animator _NPCAnimator;           // Reference to the NPC's Animator component
    [SerializeField] Animator _alienAnimator;         // Reference to the alien's Animator component
    [SerializeField] Vector2 _lastDir;                // Stores the last direction the enemy has moved in
    [SerializeField] bool _attackAnimation = false;
    [SerializeField] bool _enemyDamaged = false;

    [Header("Attack Cooldown")]
    [SerializeField] public float attackCooldown;
    [SerializeField] public float currentAttackCooldown;
    [SerializeField] public bool attackReady = false;

    void Start()
    {
        // Gets the NavMeshAgent from the enemy
        _agent = GetComponent<NavMeshAgent>();

        currentAttackCooldown = attackCooldown;

        _currentEnemyHealth = _enemyHealth;

        //-------------------
        //2D NavMesh settings
        //-------------------
        //Should the agent rotate
        _agent.updateRotation = false;
        //Should the agent movement ignore the vertical axis
        _agent.updateUpAxis = false;
    }

    void Update()
    {
        //Follows player
        _agent.SetDestination(_target.position);

        // Stores the enemy's last direction while moving
        StoreLastMove();

        // Changes enemy's sprites depending on their axes of movement (handled by StoreLastMove())
        HandleNPCSprites();

        // Changes alien's sprites depending on their axes of movement (handled by StoreLastMove()) and if it's able to attack
        HandleAlienSprites();

        // Checks distance with player to set "_canAttack" to either true or false
        DetectDistanceWithPlayer();

        HandleAttackCooldown();

        UpdateHealthBar();

        SettingUI();
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
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage to enemy
            _currentEnemyHealth -= rockDamageAmount;

            _enemyDamaged = true;

            //Checks enemy's health
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

            //Checks enemy's health
            EnemyDeath();
        }
        else _enemyDamaged = false;
    }

    private void EnemyDeath()
    {
        //Checks the enemy's health
        if (_currentEnemyHealth <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
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

        if (distance <= 1.05f)
        {
            canAttack = true;
        }
        else canAttack = false;
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
        _alienAnimator.SetBool("Attack", _attackAnimation);
        _alienAnimator.SetBool("Damage", _enemyDamaged);

        if (canAttack && attackReady)
        {
            _attackAnimation = true;
        }
        else _attackAnimation = false;
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
}
