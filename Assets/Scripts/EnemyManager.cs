using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //public static EnemyManager Instance;
    [SerializeField] public GameObject[] meleeEnemies;
    [SerializeField] public GameObject[] distanceEnemies;
    public int enemyCount;

    private void Awake()
    {
        /*8
        if (Instance == null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        */
    }

    private void Update()
    {
        EnemiesOnScene();
    }

    void EnemiesOnScene()
    {
        meleeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        distanceEnemies = GameObject.FindGameObjectsWithTag("DistanceEnemy");
        enemyCount = meleeEnemies.Length +  distanceEnemies.Length;
    }
}

