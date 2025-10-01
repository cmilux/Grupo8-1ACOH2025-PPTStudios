using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _sprayGas;              //Spray game object

    [Header("Variables")]
    [SerializeField] float _holdTimer = 0f;             //Variable used to check for how long the player held the spray on
    [SerializeField] float _requiredHoldTime = 1.5f;    //Max amount of time to hold the spray on
    [SerializeField] bool _isHolding = false;           //Checks if player is holding the input

    private void Start()
    {
        //Spray is off until player press "Fire1"
        _sprayGas.SetActive(false);
    }
    private void Update()
    {
        //Calling method
        IsButtonBeingHold();
        MeleeTimer();
    }

    void IsButtonBeingHold()
    {
        if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame))
        {
            _holdTimer = 0f;                        //Sets holdtimer to 0
            _isHolding = true;                      //Player is holding "Fire1"
            _sprayGas.SetActive(true);          //Turns the spray gameobject on
        }
        if ((Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasReleasedThisFrame))
        {
            _isHolding = false;                     //Player is not longer holding "Fire1"
            _holdTimer = 0f;                        //Sets hold time back to 0
            _sprayGas.SetActive(false);     //Turns off spray gameObject
        }

        /*
           /// 
           /// <summary>
           /// OLD INPUT
           /// </summary>
           /// 
        //Checks if R2/Left click has been pressed
        if (Input.GetButtonDown("Fire1"))
        {
            _holdTimer = 0f;                        //Sets holdtimer to 0
            _isHolding = true;                      //Player is holding "Fire1"
            _sprayGas.SetActive(true);          //Turns the spray gameobject on
        }

        //Checks if player stop pressing R2/Left click
        if (Input.GetButtonUp("Fire1"))
        {
            _isHolding = false;                     //Player is not longer holding "Fire1"
            _holdTimer = 0f;                        //Sets hold time back to 0
            _sprayGas.SetActive(false);     //Turns off spray gameObject
        }
        */
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
}