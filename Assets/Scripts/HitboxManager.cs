using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HitboxManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _hitboxSpawn;
    [SerializeField] float _hitboxRadius;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _meleeEnemy;
    [SerializeField] float _rotateSpeed;
    [SerializeField] PathTest _meleeEnemyManager;
    [SerializeField] Vector2 _playerDistance;

    private void Update()
    {
        RotateHitboxSpawn();

        _playerDistance = _player.transform.position - _meleeEnemy.transform.position;
    }

    // SELE: Prometo hacer anotaciones de cómo funciona esto en cuanto entienda cómo funciona esto
    private void RotateHitboxSpawn()
    {
        Vector3 dir = (_player.transform.position - _hitboxSpawn.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * _hitboxRadius;
        float y = Mathf.Sin(rad) * _hitboxRadius;
        _hitboxSpawn.position = _meleeEnemy.transform.position + new Vector3(x, y, 0f);

        _hitboxSpawn.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void SpawnHitbox()
    {
        Collider2D player = Physics2D.OverlapCircle(_hitboxSpawn.transform.position, _hitboxRadius, _playerLayer);
        
        if (player != null)
        {
            var playerHealth = player.GetComponentInChildren<PlayerHealth>();
            playerHealth.playerCurrentHealth -= _meleeEnemyManager.enemyDamage;
            Debug.Log("Damaged player!");
        }
    }

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(_hitboxSpawn.transform.position, _hitboxRadius);
    }
}