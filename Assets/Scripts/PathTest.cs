using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class PathTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _target;                     //Gets target transform
    [SerializeField] NavMeshAgent _agent;                   //Get's the agent (enemy) NavMesh

    [Header("Variables")]
    [SerializeField] int _enemyHealth = 100;                  // Stores how much health the enemy has
    public int enemyDamage = 7;                             // Stores how much damage the enemy can do to the player

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _enemy1Health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Gets the NavMeshAgent from the enemy
        _agent = GetComponent<NavMeshAgent>();

        //-------------------
        //2D NavMesh settings
        //-------------------
        //Should the agent rotate
        _agent.updateRotation = false;
        //Should the agent movement ignore the vertical axis
        _agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Follows player
        _agent.SetDestination(_target.position);

        SettingUI();
    }

    void SettingUI()
    {
        _enemy1Health.SetText($"Healht: {_enemyHealth}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Checks if enemy collides with a rock
        if (other.gameObject.CompareTag("Rock"))
        {
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage to enemy
            _enemyHealth -= rockDamageAmount;

            //Checks enemy's health
            EnemyDeath();
        }
        //Checks if enemy collides with the spray
        else if (other.gameObject.CompareTag("Spray"))
        {
            // Gets the damage value from the spray that the enemy has collided with
            int sprayDamage = other.gameObject.GetComponent<SprayManager>().sprayDamage;

            // Apply damage to enemy
            _enemyHealth -= sprayDamage;

            //Checks enemy's health
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        //Checks the enemy's health
        if (_enemyHealth <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }
}
