using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int enemyDamage;       // Stores how much damage the enemy can do to the player
    [SerializeField] public int enemyHealth;      // Stores how much health the enemy has
    [SerializeField] public int attackCooldown;

    [Header("Follow Logic")]
    [SerializeField] GameObject _target;
    [SerializeField] float _speed;

    Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.Find("Player");
    }

    private void Update()
    {
        // If the enemy has no more health...
        if (enemyHealth <= 0)
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    { 
        // Calculates next position for the enemy to move towards
        Vector2 direction = (_target.transform.position - transform.position).normalized;

        // Move towards that position
        _rb.linearVelocity = new Vector2(direction.x, direction.y) * _speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Rock") || other.gameObject.CompareTag("Spray"))
        {
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage and destroy rock
            enemyHealth -= rockDamageAmount;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            //AttackPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _rb.constraints = RigidbodyConstraints2D.None;
        }
    }

    //void AttackPlayer()
    //{
    //    if (attackCooldown <= 0)
    //    {
    //        _target._playerCurrentHealth -= enemyDamage;
    //    }
    //}
}

