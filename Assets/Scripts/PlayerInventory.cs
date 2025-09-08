using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int rocks;           //Where the rocks will be saved
    public bool playerHasAmmunition = false;        //Check if player has any ammunition

    void Update()
    {
        // DEBUG - Get rid of rocks when shooting
        if (Input.GetKeyDown(KeyCode.R))
        {
            rocks--;
        }

        AmmunitionAmount();
    }

    void AmmunitionAmount() 
    { 
        if (rocks > 0) 
        { 
            //if player has more than 0 rocks, then it has ammunition
            playerHasAmmunition = true; 
        } 
        if (rocks == 0) 
        {
            //if player has 0 rocks, then it doesn't have ammunition
            playerHasAmmunition = false; 
        } 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When colliding with the player, rock is destroyed from scene and gets added to inventory 
        if (other.gameObject.CompareTag("Pickups"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            rocks++;
        }
    }
}
 
