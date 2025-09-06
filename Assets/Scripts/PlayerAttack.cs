using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] Transform _rockSpawnPos; //Transform position from where the rock will be spawned
    [SerializeField] Transform _player;       //Transform of player

    [Header("GameObjects")]
    [SerializeField] GameObject _rocksPrefab; //Get's the rock prefab

    [Header("Vectors")]
    Vector2 worldPosition;      //Get mouse position on screen
    Vector2 direction;          //Used to point which direction the rock will be throwed

    [Header("Scripts")]
    PlayerInventory _playerInventory;    //Player inventory script
    PlayerMovement _playerMovement;      //Player movement script

    [Header("Floats")]
    [SerializeField] float _rockSpeed = 4.0f; //Speed of the rock when spawned

    private void Start()
    { 
        //Get's PlayerInventory and PlayerMovement script
        _playerInventory = GetComponent<PlayerInventory>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //Calling methods
        ThrowTheRock();
        HandleThrowDirection();
        SpawnDirectionRotation();
    }
    void ThrowTheRock()
    { 
        //Checks if the player has ammunition and if the left click was pressed
        if (_playerInventory.playerHasAmmunition == true && Input.GetButtonDown("Fire1"))
        { 
            //Creates a new object in rock using the rock prefab in a position and rotation (rockSpawnPos)
            var rock = Instantiate(_rocksPrefab, _rockSpawnPos.position, _rockSpawnPos.rotation);
            //Gets rock rb, sends it to a direction with a certain speed
            rock.GetComponent<Rigidbody2D>().linearVelocity = _rockSpawnPos.transform.right * _rockSpeed;

            //Substracts one rock from player's inventory
            _playerInventory.rocks--;
        }
    }

    void HandleThrowDirection()
    {
        //Gets the mouse positon on the screen
        worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        //Points where the mouse is
        direction = (worldPosition - (Vector2)_rockSpawnPos.transform.position).normalized;
        //Sets the direction to the spawn variable
        _rockSpawnPos.transform.right = direction;
    }

    void SpawnDirectionRotation()
    {
        /*
        PROBALO MOVIENDO AL JUGADOR A LA DERECHA Y APUNTANDO A LA IZQ O VICEVERSA
        SE DISPARA CON CLICK AHORA JE
        igual creo q teniendo los sprites izq der se soluciona porq saben q tienen q mover al jugador jsja pero si
        */

        if (_playerMovement._horizontalInput > 0)
        {
            //If player is moving to the right, it will spawn to their right
            _rockSpawnPos.position = new Vector2 ((_player.position.x + 0.5f), _rockSpawnPos.position.y);

        }
        else if (_playerMovement._horizontalInput < 0)
        {
            //If player is moving to the left, it will spawn to their left
            _rockSpawnPos.position = new Vector2((_player.position.x + (-0.5f)), _rockSpawnPos.position.y);
        }
    }
}
