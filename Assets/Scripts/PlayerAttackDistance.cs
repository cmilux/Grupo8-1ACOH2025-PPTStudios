using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackDistance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _rockSpawnPos; //Transform position from where the rock will be spawned
    [SerializeField] GameObject _rocksPrefab; //Get's the rock prefab
    [SerializeField] PlayerInventory _playerInventory;    //Player inventory script

    [Header("Vectors")]
    Vector2 worldPosition;      //Get mouse position on screen
    Vector2 direction;          //Used to point which direction the rock will be throwed

    [Header("Variables")]
    [SerializeField] float _rockSpeed = 4.0f; //Speed of the rock when spawned
    [SerializeField] string _lastInput;         //Stores the last input used

    private void Start()
    { 
        //Get's PlayerInventory and PlayerMovement script
        _playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        //Calling methods
        ThrowTheRock();
        HandleThrowDirection();
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

            //Destroy the rock after certain seconds
            Destroy(rock, 3f);
        }
    }

    void HandleThrowDirection()
    {
        //Sets the mouse and joy stick to zero in vector
        Vector2 mouseDir = Vector2.zero;
        Vector2 stickDir = Vector2.zero;

        //Mouse new input system
        worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseDir = (worldPosition - (Vector2)_rockSpawnPos.transform.position).normalized;

        //Right stick joystick old input system
        float stickX = Input.GetAxis("Horizontal_Joystick_R");
        float stickY = Input.GetAxis("Vertical_Joystick_R");
        stickDir = new Vector2(stickX, stickY);

        //Checks if joystick is being used
        if (stickDir.sqrMagnitude > 0.1f)
        {
            //Stores the input
            _lastInput = "joystick";
            //Normalizes the joystick direction
            stickDir = stickDir.normalized;
        }
        //Checks if mouse is being used
        else if (mouseDir.sqrMagnitude > 0.1f)
        {
            //Stores the input
            _lastInput = "mouse";
        }

        //Checks which input was last used and sets a direction to take
        if (_lastInput == "joystick")
        {
            direction = stickDir;
        }
        else
        {
            direction = mouseDir;
        }

        //Apply to spawn direction
        if (direction != Vector2.zero)
        {
            _rockSpawnPos.right = direction;
        }
    }
}
