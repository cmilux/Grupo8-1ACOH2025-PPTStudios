using UnityEngine;

public class InkManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int inkDamage;   // Stores how much damage an ink bullet can do to the player

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the ink bullet triggers with walls or enemies and destroys it
        if (other.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
