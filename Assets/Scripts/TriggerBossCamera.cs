using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class TriggerBossCamera : MonoBehaviour
{
    [Header("Boss Camera")]
    [SerializeField] GameObject _boss;                                             // Stores the boss' GameObject
    [SerializeField] BossManager _bossManager;                                     // Stores the boss' "BossManager" component
    [SerializeField] CinemachineCamera _sceneCamera;                               // Stores the scene's camera 
    [SerializeField] CinemachineCamera _bossCamera;                                // Stores the boss zone's camera 
    [SerializeField] GameObject _playerBounds;                                     // Stores the player bounds that activate when triggering the boss camera collider
    [SerializeField] bool _playerTriggeredBossCamera;                              // Checks if the player's triggered the box collider at the start of the boss zone 
    [SerializeField] List<GameObject> _bossZoneBounds = new List<GameObject>();    // Stores all the boss zone's bounds to be set active when switching cameras

    private void Start()
    {
        _bossManager = _boss.GetComponent<BossManager>();
    }

    private void Update()
    {
        HandleBossCamera();
    }

    void HandleBossCamera()
    {
        if (_playerTriggeredBossCamera)
        {
            _sceneCamera.Priority = 0;
            _bossCamera.Priority = 20;

            foreach (var bound in _bossZoneBounds)
            {
                bound.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerTriggeredBossCamera = true;
            _bossManager.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerBounds.SetActive(true);
        }
    }
}
