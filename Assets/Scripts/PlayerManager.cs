using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("Player movement variables")]
    [SerializeField] public float _horizontalInput;                 //X axis input
    [SerializeField] public float _verticalInput;                   //Y axis input
    [SerializeField] float _speed = 5f;                             //Player speed
    public bool _isBeingAttacked;                                   //Bool to check when player is being attacked
    public bool _isDead;                                            //Bool to check if player dies

    [Header("References")]
    Rigidbody2D _playerRb;                                          //Player rigidbody
    [SerializeField] PlayerAttackDistance _playerAttackDistance;    //Distance script reference
    [SerializeField] PlayerAttackMelee _playerAttackMelee;          //Melee script reference
    [SerializeField] SpriteRenderer _spriteRenderer;                //Player sprite
    [SerializeField] Transform _weaponManagerTransform;             //Weapon manager transform component
    [SerializeField] WeaponManager _weaponManager;
    [SerializeField] Transform _initialPos;
    public Animator _animator;                                      //Player animator
    public PlayerInput _playerInput;
    public bool canSwitchWeapon = true;

    [Header("Vectors")]
    [SerializeField] Vector2 _lastDir;                              //Stores player last direction
    [SerializeField] Vector2 _moveDir;                              //Stores player move direction
    public Vector2 _moveInput;                            //Stores the input from movement

    private void Awake()
    {
        //If another instance exist, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;                        //Assign the instance
        DontDestroyOnLoad(gameObject);          //Dont destroy between scenes
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Sets the player's rigidbody in its variable
        _playerRb = GetComponent<Rigidbody2D>();
        if (_playerRb == null)
        {
            Debug.LogError("Rigidbody is null");

        }
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError("Player inout is null");

        }

        _weaponManager = GetComponentInChildren<WeaponManager>();
        if (_weaponManager == null)
        {
            Debug.LogError("Weapon manager is null");

        }
        _playerAttackDistance = GetComponentInChildren<PlayerAttackDistance>(true);
        if (_playerAttackDistance == null)
        {
            Debug.LogError("Player distance attack is null");

        }
        _playerAttackMelee = GetComponentInChildren<PlayerAttackMelee>(true);
        if (_playerAttackMelee == null)
        {
            Debug.LogError("Player melee attack is null");

        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameOver")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Calls methods
        PlayerRotation();
        ApplyAnimations();
    }

    private void LateUpdate()
    {
        StoreLastMove();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        //Calls onMove method from new input
        _moveInput = inputValue;
        _playerRb.linearVelocity = _moveInput * _speed;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        //Prevent shooting if the mouse if clicking on a UI element
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (_weaponManager == null)
        {
            Debug.LogError("PlayerManager.OnAttack: weaponManager is NULL");
            return;
        }

        if (_weaponManager.currentWeaponIndex == 0)
        {
            if (_playerAttackDistance == null)
            {
                Debug.LogError("PlayerManager.OnAttack: distance is NULL");
                return;
            }
            _playerAttackDistance.StartAttack();
        }
        else if (_weaponManager.currentWeaponIndex == 1)
        {
            if (_playerAttackMelee == null)
            {
                Debug.LogError("PlayerManager.OnAttack: melee is NULL");
                return;
            }
            _playerAttackMelee.StartAttack();
        }
    }

    //public void OnSwitchWeapon(InputAction.CallbackContext context)
    //{
    //    if (context.performed && canSwitchWeapon)
    //    {
    //        _weaponManager.SwitchWeapons();
    //    }
    //}

    public void SetCanSwitchWeapon(bool value)
    {
        canSwitchWeapon = value;
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
        if (Time.timeScale == 0 || _isDead)
        {
            return;
        }

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

    public void ApplyAnimations()
    {
        //Set the walk animation depending on axis
        _animator.SetFloat("Horizontal", _moveDir.x);
        _animator.SetFloat("Vertical", _moveDir.y);
        _animator.SetFloat("Speed", _moveDir.sqrMagnitude);

        //Set the idle animation depending on axis
        _animator.SetFloat("LastDirX", _lastDir.x);
        _animator.SetFloat("LastDirY", _lastDir.y);

        //Set the bools to play hit or death animation
        _animator.SetBool("IsBeingAttacked", _isBeingAttacked);
        if (_isBeingAttacked)
        {
            Debug.Log(_isBeingAttacked);
        }
        _animator.SetBool("IsDead", _isDead);
        if (_isDead)
        {
            Debug.Log(_isDead);
            _playerRb.linearVelocity = Vector2.zero;
            _playerInput.enabled = false;
            _moveDir = Vector2.zero;
        }
    }
    public void EndAnimation()
    {
        //Turns the distance attack animation off
        if (_playerAttackDistance._isAttacking == true)
        {
            _playerAttackDistance._isAttacking = false;
        }

        //Turns the melee attack animatio off
        if (_playerAttackMelee._isAttacking == true)
        {
            _playerAttackMelee._isAttacking = false;
        }
    }

    public void UpdateActiveAttack(int weaponIndex)
    {
        //Sets attacks to false when starts
        _playerAttackDistance._isAttacking = false;
        _playerAttackMelee._isAttacking = false;

        if (_playerAttackDistance == null)
        {
            Debug.LogError("PlayerAttackDistance is NULL!");
        }
            
        if (_playerAttackMelee == null)
        {
            Debug.LogError("PlayerAttackMelee is NULL!");
        }
           

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
        Debug.LogError("SpawnRockEvent");
    }
    public void ActivateSprayEvent()
    {
        //Turns on the spray in animation
        GetComponentInChildren<PlayerAttackMelee>().ActivateSpray();
        Debug.LogError("Spray event");
    }
    public void DeactivateSprayEvent()
    {
        //Turns off the spray in animation
        GetComponentInChildren<PlayerAttackMelee>().DeactivateSpray();
        Debug.LogError("Spray event");
    }
}
