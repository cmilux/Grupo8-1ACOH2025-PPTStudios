using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public GameObject[] weapons;                        //Array for weapons
    private int currentWeaponIndex = 0;                 //Index to check what weapon is selected
    [SerializeField] PlayerMovement _playerMovement;    //PlayerMovement script

    private void Start()
    {
        //Turns off the weapons except the one being used
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);                //Turns off the weapon not used
        }
        weapons[currentWeaponIndex].SetActive(true);

        //Gets the PlayerMovement script
        _playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame))
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
        //Lets the PlayerMovement know what weapon is on
        _playerMovement.UpdateActiveAttack(currentWeaponIndex);
    }
}