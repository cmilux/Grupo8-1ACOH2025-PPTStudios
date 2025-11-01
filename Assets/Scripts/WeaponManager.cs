using NUnit.Framework;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public GameObject[] weapons;                        //Array for weapons
    public int currentWeaponIndex = 0;                 //Index to check what weapon is selected
    [SerializeField] PlayerManager _playerManager;    //PlayerMovement script
    public Image[] weaponsUI;

    [SerializeField] PlayerInventory _playerInventory;

    private void Start()
    {
        //Turns off the weapons except the one being used
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);                //Turns off the weapon not used
        }
        weapons[currentWeaponIndex].SetActive(true);    //Turns on the UI weapon being used

        //Turn off the UI weapon object except the one being used
        for (int i = 0; i < weaponsUI.Length; i++)
        {
            weaponsUI[i].gameObject.SetActive(false);                   //Turns off the UI weapon not used
        }
        weaponsUI[currentWeaponIndex].gameObject.SetActive(true);       //Turns on the UI weapon being used

        //Gets the PlayerManager and the PlayerInventory scripts
        _playerManager = GetComponentInParent<PlayerManager>();
        _playerInventory = GetComponentInParent<PlayerInventory>();
    }

    void OnEnable()
    {
        //Suscribe the event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        //Desuscribe the event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Zone1" || scene.name == "Zone2" || scene.name == "Zone3")
        {
            //Creates a new array
            weaponsUI = new Image[2];
            //Add the weapon images to the array
            weaponsUI[0] = GameObject.Find("RockUI").GetComponent<Image>();
            weaponsUI[1] = GameObject.Find("SprayUI").GetComponent<Image>();

            //Checks if space or westButton were pressed
            if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame))
            {
                //Calls the method
                SwitchWeapons();
            }

            //Calls the method
            UpdateActiveUI();
        }
    }

    private void Update()
    {
        //Checks if space or westButton were pressed
        //if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
        //    (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame))
        //{
        //    //Calls the method
        //    SwitchWeapons();
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchWeapons();
        }
    }

    public void SwitchWeapons()
    {
        Debug.Log($"SwitchWeapons called. weapons=={weapons}, weaponsUI=={weaponsUI}, currentWeaponIndex={currentWeaponIndex}");

        if (weapons == null)
        {
            Debug.LogError("WEAPONS array is NULL in WeaponManager!");
            return;
        }
        if (weaponsUI == null)
        {
            Debug.LogError("WEAPONS UI array is NULL in WeaponManager!");
            return;
        }
        if (weapons.Length == 0 || weaponsUI.Length == 0)
        {
            Debug.LogError($"WEAPON arrays have zero length (weapons={weapons?.Length}, weaponsUI={weaponsUI?.Length})");
            return;
        }

        //Turn off the current weapon (object and UI)
        weapons[currentWeaponIndex].SetActive(false);
        weaponsUI[currentWeaponIndex].gameObject.SetActive(false);
        //Switch the weapon to the next one, going back to the first item if the array is over (object and UI)
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        currentWeaponIndex = currentWeaponIndex % weaponsUI.Length;
        //Turn on new weapon (object and UI)
        weapons[currentWeaponIndex].SetActive(true);
        weaponsUI[currentWeaponIndex].gameObject.SetActive(true);
        //Lets the PlayerManager know what weapon is on
        _playerManager.UpdateActiveAttack(currentWeaponIndex);
        if (currentWeaponIndex != 0 || currentWeaponIndex != 1)
        {
            Debug.Log("current weapon is null");
        }
        //Calls the method
        UpdateActiveUI();
        //WeaponSwitchCooldown();
    }

    IEnumerator WeaponSwitchCooldown()
    {
        PlayerManager.Instance.canSwitchWeapon = false;
        yield return new WaitForSeconds(0.3f);
        PlayerManager.Instance.canSwitchWeapon = true;
    }

    public void UpdateActiveUI()
    {
        if (currentWeaponIndex == 0)        //If distance attack is on
        {
            weaponsUI[0].gameObject.SetActive(true);        //Turns on the rock UI image
            weaponsUI[1].gameObject.SetActive(false);       //Turns off the spray UI image
        }
        else if (currentWeaponIndex == 1)   //If melee attack is on
        {
            weaponsUI[0].gameObject.SetActive(false);       //Turns off the rock UI image
            weaponsUI[1].gameObject.SetActive(true);        //Turns on the spray UI image
        }
    }
}