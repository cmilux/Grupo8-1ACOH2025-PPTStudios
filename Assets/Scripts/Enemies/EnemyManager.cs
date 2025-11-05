using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //public static EnemyManager Instance;
    public GameObject[] meleeEnemies;
    public GameObject[] distanceEnemies;
    public GameObject boss;
    public int enemyCount;

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

