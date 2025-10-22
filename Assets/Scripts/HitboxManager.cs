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
    [SerializeField] PathTest _meleeEnemyManager;

    [Header("Animation")]
    [SerializeField] Animator _alienAnimator;

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Time.timeScale = 1;

    //    if (/*scene.name == "Zone1" ||*/ scene.name == "Zone2" || scene.name == "Zone3")
    //    {
    //        _player = PlayerMovement.Instance.gameObject;
    //    }
    //}

    private void Update()
    {
        RotateHitboxSpawn();
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

        if (player != null && _meleeEnemyManager.attackReady == true && !_meleeEnemyManager.enemyDying)
        {
            var playerHealth = player.GetComponentInChildren<PlayerHealth>();
            playerHealth.playerCurrentHealth -= _meleeEnemyManager.enemyDamage;

            _meleeEnemyManager.attackReady = false;
            _meleeEnemyManager.currentAttackCooldown = _meleeEnemyManager.attackCooldown;
        }
    }

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(_hitboxSpawn.transform.position, _hitboxRadius);
    }
}
