using System.Collections;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _hitboxSpawn;
    [SerializeField] GameObject _hitboxPrefab;
    [SerializeField] PathTest _meleeEnemyManager;

    void Update()
    {
        SpawnHitbox();
    }

    private void SpawnHitbox()
    {
        if (_meleeEnemyManager.canAttack == true)
        {
            var hitbox = Instantiate(_hitboxPrefab, _hitboxSpawn.position, _hitboxSpawn.rotation);
            StartCoroutine(DestroyHitbox(hitbox));
        }    
    }

    private IEnumerator DestroyHitbox(GameObject hitbox)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(hitbox);
    }
}


