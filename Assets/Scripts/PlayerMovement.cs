using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player movement variables")]
    [SerializeField] public float _horizontalInput;                 //X axis input
    [SerializeField] public float _verticalInput;                   //Y axis input
    [SerializeField] float _speed = 5f;                             //Player speed

    [Header("References")]
    Rigidbody2D _playerRb;                                          //Player rigidbody
    [SerializeField] PlayerAttackDistance _playerAttackDistance;    //Distance script reference
    [SerializeField] PlayerAttackMelee _playerAttackMelee;          //Melee script reference
    [SerializeField] SpriteRenderer _spriteRenderer;                //Player sprite
    [SerializeField] SpriteRenderer _spawnPosRenderer;              //SpawnPos Renderer
    [SerializeField] Transform _weaponManager;                      //Weapon manager transform component
    public Animator _animator;                                      //Player animator

    [Header("Vectors")]
    [SerializeField] Vector2 _lastDir;                              //Stores player last direction
    [SerializeField] Vector2 _moveDir;                              //Stores player move direction
    [SerializeField] Vector2 _moveInput;                            //Stores the input from movement

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Sets the player's rigidbody in its variable
        _playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calls methods
        PlayerRotation();
        ApplyAnimations();
        //RotateWeapon();
    }

    private void LateUpdate()
    {
        StoreLastMove();
    }

    private void OnMove(InputValue inputValue)
    {
        //Calls onMove method from new input
        _moveInput = inputValue.Get<Vector2>();
        _playerRb.linearVelocity = _moveInput * _speed;
    }

    void StoreLastMove()
    {
        //Saves the player last direction
        _moveDir = new Vector2(_moveInput.x, _moveInput.y);
        if (_moveDir != Vector2.zero)
        {
            _lastDir = _moveDir;
        }
    }

    public void PlayerRotation()
    {
        //Flips the sprite depending on movement direction
        if (_moveInput.x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_moveInput.x < 0)
        {
            _spriteRenderer.flipX = false;
        }
        
        //Forces the sprite to not flip while player moves around Y axis
        else if (_moveInput.y > 0 || _moveInput.y < 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    void ApplyAnimations()
    {
        //Set the walk animation depending on axis
        _animator.SetFloat("Horizontal", _moveDir.x);
        _animator.SetFloat("Vertical", _moveDir.y);
        _animator.SetFloat("Speed", _moveDir.sqrMagnitude);

        //Set the idle animation depending on axis
        _animator.SetFloat("LastDirX", _lastDir.x);
        _animator.SetFloat("LastDirY", _lastDir.y);
    }
    public void EndAnimation()
    {
        //Turns the distance attack animation off
        if (_playerAttackDistance._isAttacking == true)
        {
            _playerAttackDistance._isAttacking = false;
        }

        //Turns the melee attack animatio off
        if(_playerAttackMelee._isAttacking == true)
        {
            _playerAttackMelee._isAttacking = false;
        }
       
    }

    //Unnecesary since the rotation is controlled on the animations
    //Here just in case for now
    void RotateWeapon()
    {
        //Check if the player has a movement direction stored
        if (_lastDir != Vector2.zero)
        {
            //Convert the 2D direction (x, y) into an angle in degrees
            float angle = Mathf.Atan2(_lastDir.y, _lastDir.x) * Mathf.Rad2Deg;

            //Apply the angle as rotation to the weapon manager (only on Z axis for 2D)
            _weaponManager.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void UpdateActiveAttack(int weaponIndex)
    {
        //Sets attacks to false when starts
        _playerAttackDistance._isAttacking = false;
        _playerAttackMelee._isAttacking = false;

        //Checks if distance attack [index 0 on weapon manager] is set
        if (weaponIndex == 0)
        {
            //Enables distance attack
            _playerAttackDistance.enabled = true;
            //Disables melee attack
            _playerAttackMelee.enabled = false;
        }
        //Checks if melee attack [index 1 on weapon manager] is set
        else if (weaponIndex == 1)
        {
            //Disables distance attack
            _playerAttackDistance.enabled = false;
            //Enables melee attack
            _playerAttackMelee.enabled = true;
        }
    }

    public void SpawnRockEvent()
    {
        //Spawn the rocks at the right time in animation
        GetComponentInChildren<PlayerAttackDistance>().SpawnRock();
    }

    public void ActivateSprayEvent()
    {
        //Turns on the spray in animation
        GetComponentInChildren<PlayerAttackMelee>().ActivateSpray();
    }
    public void DeactivateSprayEvent()
    {
        //Turns off the spray in animation
        GetComponentInChildren<PlayerAttackMelee>().DeactivateSpray();
    }
}
