using UnityEngine;

public class RockManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int rockDamage;   // Stores how much damage a rock can do to enemies

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the rock triggers with walls or enemies and destroys it
        if (other.gameObject.CompareTag("Walls") || other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
       
    }
}
