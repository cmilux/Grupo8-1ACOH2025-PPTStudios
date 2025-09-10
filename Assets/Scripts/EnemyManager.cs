using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int enemyDamage;       // Stores how much damage the enemy can do to the player
    [SerializeField] public int enemyHealth;     // Stores how much health the enemy has

    [Header("Follow Logic")]
    [SerializeField] GameObject _target;
    [SerializeField] Vector2 _direction;
    [SerializeField] float _speed;
    [SerializeField] bool _activateFollowLogic;

    private void Update()
    {
        _target = GameObject.Find("Player");

        // Stores player's position
        _direction = (_target.transform.position - transform.position).normalized;

        // Move towards player's position (if follow logic is activated)
        if (_activateFollowLogic == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, _direction, _speed * Time.deltaTime);
        }

        // If the enemy has no more health...
        if (enemyHealth <= 0)   
        {
            // Destroy enemy
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickups"))
        {
            // Gets the damage value from the rock that the enemy has collided with
            int rockDamageAmount = other.gameObject.GetComponent<RockManager>().rockDamage;

            // Apply damage and destroy rock
            enemyHealth -= rockDamageAmount;
            Destroy(other.gameObject);
        }
    }
}
