using System.Collections;
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
    [SerializeField] float _attackCooldown;                 //Cooldown time between attacks
    [SerializeField] float _currentAttackTime;              //Tracks current cooldown timer

    private void Start()
    {
        //Get's PlayerInventory and PlayerMovement script
        _playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
    }
    private void Update()
    {
        //Calling methods
        HandleThrowDirection();
        ThrowTheRock();

        //Decrease cooldown timer
        _currentAttackTime -= Time.deltaTime;
    }

    void ThrowTheRock()
    {
        //Check if the cooldown time has passed
        if (_currentAttackTime > 0)
        {
            //Does nothing
            return;
        }

        //Saves in the variable if the mouse was clicked or the R2 from joystick was triggered
        bool isAttacking = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

        //Checks if the player has ammunition and if the left click was pressed
        if (_playerInventory.playerHasAmmunition == true && isAttacking)
        {
            //Creates a new object in rock using the rock prefab in a position and rotation (rockSpawnPos)
            var rock = Instantiate(_rocksPrefab, _rockSpawnPos.position, _rockSpawnPos.rotation);
            //Gets rock rb, sends it to a direction with a certain speed
            rock.GetComponent<Rigidbody2D>().linearVelocity = _rockSpawnPos.transform.right * _rockSpeed;
            //Set the cooldown timer
            _currentAttackTime = _attackCooldown;
            //Substracts one rock from player's inventory
            _playerInventory.rocks--;
            //Destroy the rock after certain seconds
            Destroy(rock, 3f);
        }
    }

    void HandleThrowDirection()
    {
        //Initialize vectors to store mouse and joystick directions
        Vector2 mouseDir = Vector2.zero;
        Vector2 stickDir = Vector2.zero;

        if (Mouse.current != null)
        {
            //Si estas dos lineas van dentro del if, el player dispara en la posicion que mira, pero no dispara en diagonal
            //Gets the mouse position
            _worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            //Calculate the direction from spawn position to mouse position
            mouseDir = (_worldPosition - (Vector2)_rockSpawnPos.transform.position).normalized;

            //Only store mouse input if direction is significant
            if (mouseDir.sqrMagnitude > 0.1f)
            {
                //Stores as last input
                _lastInput = "Mouse";
            }
        }
        if (Gamepad.current != null)
        {
            //Gets the right stick direction
            stickDir = Gamepad.current.rightStick.ReadValue();

            //Only store joystick input if direction is significant
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
}
