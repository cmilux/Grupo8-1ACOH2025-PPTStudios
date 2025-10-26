using System;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    [Header("References")]
    [SerializeField] bool _goToNextLevel;   //Load next scene
    [SerializeField] string _levelName;     //Used to test, load the scene requiered

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_goToNextLevel)
            {
                //If player collides with the arrow will load next level
                GameManager.Instance.NextScene();
            }

            //Not be neccesary but here to test
            else
            {
                //Load the scene requiered on inspector
                GameManager.Instance.LoadScene(_levelName);
            }
        }   
    }
}
