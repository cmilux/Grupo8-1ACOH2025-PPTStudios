using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int rocks; 

    void Update()
    {
        // DEBUG - Get rid of rocks when shooting
        if (Input.GetKeyDown(KeyCode.R))
        {
            rocks--;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When colliding with the player, rock is deactivated from scene and gets added to inventory 
        if (other.gameObject.CompareTag("Pickups"))
        {
            other.gameObject.SetActive(false);
            rocks++;
        }
    }
}
 
