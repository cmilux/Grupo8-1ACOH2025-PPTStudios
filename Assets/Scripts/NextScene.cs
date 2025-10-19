using System;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    [SerializeField] bool _goToNextLevel;
    [SerializeField] string _levelName;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_goToNextLevel)
            {
                GameManager.Instance.NextScene();
            }
            //It may not be neccesary but here to test
            else
            {
                GameManager.Instance.LoadScene(_levelName);
            }
        }   
    }
}
