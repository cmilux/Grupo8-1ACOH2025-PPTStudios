using UnityEngine;

public class PlayerAttackTest : MonoBehaviour
{
    public Transform rockSpawnPos;          //Position from where the rock will be spawned
    public GameObject rocksPrefab;          //Get's the rock prefab

    PlayerInventory _playerInventory;       //Player inventory script

    float _rockSpeed = 2f;                  //Speed of the rock when spawned

    private void Start()
    {
        //Get's PlayerInventory script
        _playerInventory = GetComponent<PlayerInventory>();
    }
    private void Update()
    {
        ThrowTheRock();
    }

    void ThrowTheRock()
    {
        //Checks if the player has ammunition and if the space was pressed
        if (_playerInventory.playerHasAmmunition == true && Input.GetButtonDown("Jump"))
        {
            //Creates a new object in rock using the rock prefab in a position and rotation (rockSpawnPos)
            var rock = Instantiate(rocksPrefab, rockSpawnPos.position, rockSpawnPos.rotation);
            //Gets rock rb, sends it to a direction with a certain speed
            rock.GetComponent<Rigidbody2D>().linearVelocity = rockSpawnPos.right * _rockSpeed;

            //Substracts one rock from player's inventory
            _playerInventory.rocks--;
        }
    }
}
