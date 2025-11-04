using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _sprayGas;              //Spray game object
    [SerializeField] GameObject _sprayParticle;         //Spray particles game object
    [SerializeField] PlayerManager _playerAnimator;    //PlayerMovement script to get a reference of the animator
    [SerializeField] ParticleSystem _sprayEffect;       //Spray particles   
    [SerializeField] WeaponManager _weaponManager;               //Weapon manager script

    [Header("Variables")]
    [SerializeField] float _holdTimer = 0f;             //Variable used to check for how long the player held the spray on
    [SerializeField] float _requiredHoldTime = 1.5f;    //Max amount of time to hold the spray on
    [SerializeField] bool _isHolding = false;           //Checks if player is holding the input
    public bool _isAttacking;                           //Bool to check when player is attacking

    [Header("SFX")]
    private AudioSource _meleeAttackSFX;
    [SerializeField] AudioClip _mA_SFX;

    private void Start()
    {
        //Spray is off until player press "Fire1"
        _sprayGas.SetActive(false);

        //Find the playerMovement script to get a reference of the player's animator
        _playerAnimator = GetComponentInParent<PlayerManager>();
        if (_playerAnimator == null)
        {
            Debug.Log($"playyer manager is null in melee attack");
        }

        //Get the weapon manager script
        _weaponManager = GetComponentInParent<WeaponManager>();
        if (_weaponManager == null)
        {
            Debug.Log($"weapon manager is null in melee attack");
        }

        //Get the audio source
        _meleeAttackSFX = GetComponent<AudioSource>();
        if (_meleeAttackSFX == null)
        {
            Debug.Log($"sfx is null in melee attack");
        }

        if (_sprayEffect != null)
        {
            //Just to make sure spray doesnt start playing when unrequired
            _sprayEffect.Stop();
        }
    }
    private void Update()
    {
        //Calling methods
        //IsButtonBeingHold();
        MeleeTimer();
        ApplyAnimation();
        OnPlayerAttacking();
    }

    public void StartAttack()
    {
        if (!_isAttacking)
        {
            PlayerManager.Instance.SetCanSwitchWeapon(false);
            _isAttacking = true;
            ActivateSpray();
            StartCoroutine(StopMeleeAttackAfterTime());
        }
    }

    private IEnumerator StopMeleeAttackAfterTime()
    {
        yield return new WaitForSeconds(_requiredHoldTime);
        DeactivateSpray();
    }

    public void IsButtonBeingHold()
    {
        //Prevent shooting if the mouse if clicking on a UI element
        //if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}

        //If the mouse or gamepad was pressed
        if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame))
        {
            _holdTimer = 0f;                        //Sets holdtimer to 0
            _isHolding = true;                      //Player is holding the mouse left button
            _isAttacking = true;                    //Player is attacking
            //_sprayGas.SetActive(true);            //Turns the spray gameobject on
        }

        //If the mouse or gamepad was released
        if ((Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasReleasedThisFrame))
        {
            _isHolding = false;                     //Player is not longer holding the mouse left button
            _isAttacking = false;                   //Player is not longer attacking
            _holdTimer = 0f;                        //Sets hold time back to 0
            _sprayGas.SetActive(false);         //Turns off spray gameObject with it's collider
            _sprayParticle.SetActive(false);    //Turn off the spray particle
            _meleeAttackSFX.Stop();                 //Stop the spray SFX
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

    public void ActivateSpray()
    {
        _isAttacking = true;                    //Player is attacking
        _sprayGas.SetActive(true);        //Turns the spray gameobject on with it's collider
        _sprayParticle.SetActive(true);     //Turns the spray particle on
        if (_sprayEffect != null)
        {
            //Play the particles
            _sprayEffect.Play();
            //Play the spray SFX
            _meleeAttackSFX.PlayOneShot(_mA_SFX, 0.3f);
        }
    }
    public void DeactivateSpray()
    {
        _isAttacking = false;                       //Player is not longer attacking
        _sprayGas.SetActive(false);             //Turns off spray gameObject with it's collider
        _sprayParticle.SetActive(false);        //Turns partcles off
        if (_sprayEffect != null)
        {
            //Stop the particles if not attacking
            _sprayEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
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
        //Applies the bool _isAttacking to the melee animation
        PlayerManager.Instance._animator.SetBool("IsMeleeAttacking", _isAttacking);
    }
}