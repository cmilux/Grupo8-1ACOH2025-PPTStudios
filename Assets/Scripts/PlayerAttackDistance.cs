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
    public bool _isAttacking;                               //Checks if player is attacking

    [Header("Animator")]
    [SerializeField] PlayerManager _playerAnimator;        //Player animator

    private void Start()
    {
        //Get's PlayerInventory and PlayerMovement script
        _playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        _playerAnimator = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();

        //Initialize cooldown to 0 so player cant shoot as soon as game starts
        _currentAttackTime = 0f;
    }

    private void Update()
    {
        //Calling methods
        HandleThrowDirection();
        AttackInput();
        ApplyAnimations();
        OnPlayerAttacking();

        //Decrease cooldown timer per frame
        if (_currentAttackTime > 0f)
        {
            _currentAttackTime -= Time.deltaTime;
        }
    }

    void AttackInput()
    {
        //Prevent shooting while cooldown is active
        if (_currentAttackTime > 0) return;

        //Save if the mouse click or controller trigger was pressed
        bool shoot = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

        //Checks if the player has ammunition and if the shoot button was pressed
        if (_playerInventory.playerHasAmmunition == true && shoot)
        {
            _currentAttackTime = _attackCooldown;           //Restart cooldown       
            _isAttacking = true;                            //Player is attackin
        }
    }

    /// <summary>
    /// Instantiantes the rock with a speed and decreases the ammo
    /// This method called from an animation event in PlayerMovement script
    /// </summary>
    public void SpawnRock()
    {
        //Creates a new object in rock using the rock prefab in a position and rotation (rockSpawnPos)
        var rock = Instantiate(_rocksPrefab, _rockSpawnPos.position, _rockSpawnPos.rotation);
        //Gets rock rb, sends it to a direction with a certain speed
        rock.GetComponent<Rigidbody2D>().linearVelocity = _rockSpawnPos.transform.right * _rockSpeed;
        //Substracts one rock from player's inventory
        _playerInventory.totalRocks--;
        //Destroy the rock after certain seconds
        Destroy(rock, 3f);
        //After animation ends, reset attacking state
        StartCoroutine(ResetAttackFlag());
    }

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(0.1f);        //Waits 0.1 seconds
        _isAttacking = false;                               //Returns that player is not attacking
    }

    /// <summary>
    /// Detects what the player is using (mouse or gamepad)
    /// Rotates the spawn point toward that direction
    /// </summary>
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

    void OnPlayerAttacking()
    {
        if (_isAttacking == true)
        {
            PlayerManager.Instance._playerInput.enabled = false;
        }
        else if (_isAttacking == false)
        {
            PlayerManager.Instance._playerInput.enabled = true;
        }
    }

    void ApplyAnimations()
    {
        //Applies the bool _isAttacking to the distance animation
        _playerAnimator._animator.SetBool("IsDistanceAttacking", _isAttacking);
    }
}
