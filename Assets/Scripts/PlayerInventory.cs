using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public int rocks; 

    public bool playerHasAmmunition = false;

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
            playerHasAmmunition = true;
        }

        if (rocks == 0)
        {
            playerHasAmmunition = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When colliding with the player, rock is deactivated from scene and gets added to inventory 
        if (other.gameObject.CompareTag("Pickups"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            rocks++;
        }
    }

}
 
