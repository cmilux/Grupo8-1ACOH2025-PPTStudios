using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int enemyDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickups"))
        {
            //If enemy collides with a rock ("Pickups"), destroy both
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
