using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class AlienManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _hitboxSpawn;
    [SerializeField] float _hitboxRadius;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _meleeEnemy;
    [SerializeField] float _rotateSpeed;
    [SerializeField] MeleeEnemyManager _meleeEnemyManager;
    [SerializeField] PlayerHealth _playerHealth;

    private void Start()
    {
        _playerHealth = GameObject.Find("Player")?.GetComponentInChildren<PlayerHealth>();
    }

    private void Update()
    {
        //RotateHitboxSpawn();
        if (_meleeEnemyManager.canAttack)
        {
            SpawnHitbox();
        }
    }

    // SELE: Prometo hacer anotaciones de c�mo funciona esto en cuanto entienda c�mo funciona esto
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

        if (player == null)
        {
            Debug.Log("Player collider is null in alien manager");
        }
        else
        {
            Debug.Log("Player collider is NOT null");
        }
        if (player != null && _meleeEnemyManager.attackReady == true && !_meleeEnemyManager.enemyDying)
        {
            var playerHealth = player.GetComponentInChildren<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.Log("Player is null in alien manager");
            }
            playerHealth.playerCurrentHealth -= _meleeEnemyManager.enemyDamage;

            StartCoroutine(StopAnimationDamage());

            //Plays the SFX
            _playerHealth._playerHealthSFX.PlayOneShot(_playerHealth._playerHitSFX, 0.3f);

            _meleeEnemyManager.attackReady = false;
            _meleeEnemyManager.currentAttackCooldown = _meleeEnemyManager.attackCooldown;
        }
    }

    IEnumerator StopAnimationDamage()
    {
        PlayerManager.Instance._isBeingAttacked = true;             //Plays the animation
        yield return new WaitForSeconds(1f);
        PlayerManager.Instance._isBeingAttacked = false;            //Stops the animation
    }

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(_hitboxSpawn.transform.position, _hitboxRadius);
    }
}
