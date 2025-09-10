using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon variables for array")]
    int totalWeapons = 2;
    public int currentWeaponIndex;

    [Header("GameObjects")]
    public GameObject[] weapons;
    public GameObject weaponHolder;
    public GameObject currentWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //totalWeapons = weaponHolder.transform.childCount;
        //weapons = new GameObject[totalWeapons];

        for (int i = 0; i < totalWeapons; i++)
        {
            weapons[i] = weaponHolder.transform.GetChild(i).gameObject;
            weapons[i].SetActive(false);
        }

        weapons[0].SetActive(true);
        currentWeapon = weapons[0];
        currentWeaponIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWeapon();
    }

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentWeaponIndex == 1)
            {
                weapons[currentWeaponIndex].SetActive(false);
                currentWeaponIndex = 0;
                weapons[currentWeaponIndex].SetActive(true);
                currentWeapon = weapons[currentWeaponIndex];
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentWeaponIndex == 0)
            {
                weapons[currentWeaponIndex].SetActive(false);
                currentWeaponIndex = 1;
                weapons[currentWeaponIndex].SetActive(true);
                currentWeapon = weapons[currentWeaponIndex];
            }
        }
    }
}
