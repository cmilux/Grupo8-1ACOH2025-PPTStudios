using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public GameObject[] weapons;            //Array for weapons
    private int currentWeaponIndex = 0;     //Index to check what weapon is selected

    private void Start()
    {
        //Turns off the weapons except the one being used
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);                //Turns off the weapon not used
        }
        weapons[currentWeaponIndex].SetActive(true);
    }

    private void Update()
    {
        /// <summary>
        /// NEEDS TO BE UPDATED TO NEW INPUT SYSTEM
        /// </summary>

        //Check if "space"/square where pressed
        if (Input.GetButtonDown("SwitchWeapons_J"))
        {
            SwitchWeapons();
        }
    }

    public void SwitchWeapons()
    {
        //Turn off the current weapon
        weapons[currentWeaponIndex].SetActive(false);
        //Switch the weapon to the next one, going back to the first item if the array is over
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        //Turn on new weapon
        weapons[currentWeaponIndex].SetActive(true);
    }
}