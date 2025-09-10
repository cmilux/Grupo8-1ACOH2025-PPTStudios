using UnityEngine;

public class PlayerAttackMelee : MonoBehaviour
{
    [Header("Attack Radius")]
    [SerializeField] Transform _radiusT;
    [SerializeField] float _radiusD = 2f;

    [Header("Bools")]
    [SerializeField] bool _isAttacking = false;    // Checks if player is pressing the "attack" button 

    [Header("Stats")]
    [SerializeField] int _playerMeleeDamage;       // Stores how much damage the player does in a melee attack 

    private void FixedUpdate()
    {
        // Checks if player is pressing the "attack" button 
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isAttacking = true;
        }
        else _isAttacking = false;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        // If player's radius collides with an enemy and the player is pressing the "attack" button...
        if (collider.gameObject.CompareTag("Enemy") && _isAttacking == true)
        {
            // Apply player melee damage to the enemy colliding with the player
            collider.gameObject.GetComponent<EnemyManager>().enemyHealth -= _playerMeleeDamage;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_radiusT.position, _radiusD);
    }
}
