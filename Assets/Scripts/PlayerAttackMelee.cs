using UnityEngine;

public class PlayerAttackMelee : MonoBehaviour
{
    [Header("Attack Radius")]
    [SerializeField] Transform _radiusT;
    [SerializeField] float _radiusD = 2f;

    [Header("Bools")]
    [SerializeField] bool _isAttacking = false;

    [Header("Stats")]
    [SerializeField] int _playerMeleeDamage;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isAttacking = true;
        }
        else _isAttacking = false;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy") && _isAttacking == true)
        {
            collider.gameObject.GetComponent<EnemyTest>().enemyHealth -= _playerMeleeDamage;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_radiusT.position, _radiusD);
    }
}
