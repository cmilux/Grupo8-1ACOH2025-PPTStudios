using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackDistance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _rockSpawnPos;               //Transform position from where the rock will be spawned
    [SerializeField] GameObject _rocksPrefab;               //Get's the rock prefab
    PlayerInventory _playerInventory;                       //Player inventory script

    [Header("Vectors")]
    [SerializeField] Vector2 _worldPosition;                 //Get mouse position on screen
    [SerializeField] Vector2 _direction;                     //Used to point which direction the rock will be throwed

    [Header("Variables")]
    [SerializeField] float _rockSpeed = 4.0f;               //Speed of the rock when spawned
    [SerializeField] string _lastInput;                     //Stores the last input used
    [SerializeField] bool _isAttacking;                     //Bool to check if player is attacking

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
        //Saves in the variable if the mouse was clicked or the R2 from joystick was triggered
        bool _fire = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

        //Checks if the player has ammunition and if the left click was pressed
        if (_playerInventory.playerHasAmmunition == true && _fire)
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

        if (Mouse.current != null)
        {
            //Gets the mouse position
            _worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            //Resta la posicion del mouse con la posicion de spawn y
                //normaliza su "velocidad" para que la piedra sepa donde ir
            mouseDir = (_worldPosition - (Vector2)_rockSpawnPos.transform.position).normalized;
            if (mouseDir.sqrMagnitude > 0.1f)
            {
                //Stores as last input
                _lastInput = "Mouse";
            }
        }
        if (Gamepad.current != null)
        {
            //Gets the right stick position
            stickDir = Gamepad.current.rightStick.ReadValue();
            if (stickDir.sqrMagnitude > 0.1f)
            {
                //Normalizes the right stick "speed"
                stickDir.Normalize();
                //Stores as last input
                _lastInput = "Joystick";
            }
        }

        //Checks which input was last used and sets a direction to take
        if (_lastInput == "Joystick")
        {
            _direction = stickDir;
        }
        else
        {
            _direction = mouseDir;
        }

        //Apply to spawn direction
        if (_direction != Vector2.zero)
        {
            _rockSpawnPos.right = _direction;
        }
    }

    /// <summary>
    /// OLD SCRIPT
    /// </summary>

    /*
    public void HandleThrowDirection(Vector2 input) //OnLook(Vector2 input)
    {
        if (Gamepad.current != null)
        {
            direction = direction.normalized; //= input.normalized;
            Debug.Log("It's working");
        }
        else if(Mouse.current != null)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            direction = (worldPosition - (Vector2)_rockSpawnPos.transform.position).normalized;
        }

        if (direction != Vector2.zero)
        {
            _rockSpawnPos.right = direction;
        }
    }

    public void OnAttack(InputValue value)
    {
        //Checks if the player has ammunition and if the left click was pressed
        if (_playerInventory.playerHasAmmunition)
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

    private void Update()
    {
        //Calling methods
        ThrowTheRock();
        HandleThrowDirection();
    }
    void ThrowTheRock()
    {
        //Checks if the player has ammunition and if the left click was pressed
        if (_playerInventory.playerHasAmmunition)
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

        if (Mouse.current != null)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseDir = (worldPosition - (Vector2)_rockSpawnPos.transform.position).normalized;
            if (mouseDir.sqrMagnitude > 0.1f)
            {
                _lastInput = "Mouse";
            }
        }

        if (Gamepad.current != null)
        {
            stickDir = Gamepad.current.rightStick.ReadValue();
            if (stickDir.sqrMagnitude > 0.1f)
            {
                stickDir.Normalize();
                _lastInput = "Joystick";
            }
        }

        //Checks which input was last used and sets a direction to take
        if (_lastInput == "Joystick")
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
    */
}
