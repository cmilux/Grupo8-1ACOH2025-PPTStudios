using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int totalRocks;                                       //Where the rocks will be saved
    private readonly int rockPickUpValue = 5;
    public bool playerHasAmmunition = false;                //Check if player has any ammunition

    [Header("Inventory UI")]
    [SerializeField] TextMeshProUGUI _playerInventory;      //UI inventory reference

    [Header("Inventory SFX")]
    [SerializeField] AudioClip _playerPickUpRockSFX;
    private AudioSource _playerInventorySFX;


    private void Start()
    {
        //Get the player inventory Text UI
        _playerInventory = GameObject.Find("Inventory")?.GetComponent<TextMeshProUGUI>();
        _playerInventorySFX = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        //Suscribe to OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        //Desuscribe to OnSceneLoaded
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Zone1" || sceneName == "Zone2" || sceneName == "Zone3")
        {
            //Get the text mesh pro object
            //_playerInventory = GameObject.Find("InventoryText")?.GetComponent<TextMeshProUGUI>();
            //if (_playerInventory == null)
            //{
            //    Debug.Log("Wasnt found (I)");
            //}

            //Call the method in any of the scenes mentioned
            SettingUI();
        }
    }


    void Update()
    {
        //Call method
        AmmunitionAmount();
        SettingUI();
    }

    void AmmunitionAmount()
    {
        if (totalRocks > 0)
        {
            //if player has more than 0 rocks, then it has ammunition
            playerHasAmmunition = true;
        }
        if (totalRocks == 0)
        {
            //if player has 0 rocks, then it doesn't have ammunition
            playerHasAmmunition = false;
        }
    }

    void SettingUI()
    {
        //Get the text mesh pro object
        _playerInventory = GameObject.Find("InventoryText")?.GetComponent<TextMeshProUGUI>();
        _playerInventory.SetText($"{totalRocks}");
    }

    public void AddRock(int rocks)
    {
        totalRocks += rocks;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When colliding with the player, rock is destroyed from scene and gets added to inventory 
        if (other.gameObject.CompareTag("Pickups"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            AddRock(rockPickUpValue);
            _playerInventorySFX.PlayOneShot(_playerPickUpRockSFX, 0.3f);
        }
    }
}

