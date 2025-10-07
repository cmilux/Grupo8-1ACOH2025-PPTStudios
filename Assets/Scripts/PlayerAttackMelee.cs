using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _sprayGas;              //Spray game object
    [SerializeField] PlayerMovement _playerAnimator;    //PlayerMovement script to get a reference of the animator

    [Header("Variables")]
    [SerializeField] float _holdTimer = 0f;             //Variable used to check for how long the player held the spray on
    [SerializeField] float _requiredHoldTime = 1.5f;    //Max amount of time to hold the spray on
    [SerializeField] bool _isHolding = false;           //Checks if player is holding the input
    public bool _isAttacking;                           //Bool to check when player is attacking

    private void Start()
    {
        //Spray is off until player press "Fire1"
        _sprayGas.SetActive(false);

        //Find the playerMovement script to get a reference of the player's animator
        _playerAnimator = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        //Calling methods
        IsButtonBeingHold();
        MeleeTimer();
        ApplyAnimation();
    }

    public void IsButtonBeingHold()
    {
        if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame))
        {
            _holdTimer = 0f;                        //Sets holdtimer to 0
            _isHolding = true;                      //Player is holding "Fire1"
            _isAttacking = true;                    //Player is attacking
            //_sprayGas.SetActive(true);        //Turns the spray gameobject on
        }
        if ((Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasReleasedThisFrame))
        {
            _isHolding = false;                     //Player is not longer holding "Fire1"
            _isAttacking = false;                   //Player is not longer attacking
            _holdTimer = 0f;                        //Sets hold time back to 0
            _sprayGas.SetActive(false);       //Turns off spray gameObject
        }
    }

    public void ActivateSpray()
    {
        _isAttacking = true;                    //Player is attacking
        _sprayGas.SetActive(true);        //Turns the spray gameobject on
    }
    public void DeactivateSpray()
    {
        _isAttacking = false;                   //Player is not longer attacking
        _sprayGas.SetActive(false);       //Turns off spray gameObject
    }

    void MeleeTimer()
    {
        //Checks if player is holding the R2/Left click
        if (_isHolding && (Mouse.current != null && Mouse.current.leftButton.isPressed) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.isPressed))//Input.GetButton("Fire1"))
        {
            //Sets time to the holder timer
            _holdTimer += Time.deltaTime;

            //Checks if hold timer is higher or equal than required hold timer
            if (_holdTimer >= _requiredHoldTime)
            {
                _isHolding = false;                 //Player is not longer "holding" R2/Left click
                _sprayGas.SetActive(false);     //Turns off spray gameObject
            }
        }
    }

    void ApplyAnimation()
    {
        ////Applies the bool _isAttacking to the melee animation
        _playerAnimator._animator.SetBool("IsMeleeAttacking", _isAttacking);
    }
}