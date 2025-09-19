using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player movement")]
    [SerializeField] public float _horizontalInput;             //X axis input
    [SerializeField] public float _verticalInput;               //Y axis input
    [SerializeField] float _speed = 5f;                         //Player speed
    [SerializeField] bool _isWalking = false;                   //Checks if player is moving

    [Header("References")]
    Rigidbody2D _playerRb;                                      //Player rigidbody
    [SerializeField] SpriteRenderer _spriteRenderer;            //Player sprite
    [SerializeField] Animator _animator;                        //Player animator
    [SerializeField] Transform _weaponManager;                  //Weapon manager transform component
    [SerializeField] Vector2 _lastDir;                          //Stores player last direction
    [SerializeField] Vector2 _moveDir;                          //Stores player move direction
    [SerializeField] Vector2 _moveInput;                        //Stores the input from movement

    /*
     * Hay un problema con la rotacion del weapon manager que hay q moverse un par de veces antes de que oficialmente la weapon gire
     */

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
        IsPlayerMoving();
        PlayerRotation();
        ApplyAnimations();
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

    }

    void ApplyAnimations()
    {
        //Set the right animation depending on axis
        _animator.SetFloat("Horizontal", _lastDir.x);
        _animator.SetFloat("Vertical", _lastDir.y);
        _animator.SetFloat("Speed", _speed);
    }

    void IsPlayerMoving()
    {
        if (_isWalking)
        {
            //Check if player is walking, rotate the weapon to the player's direction
            Vector3 vector3 = Vector3.left * _lastDir.x + Vector3.down * _lastDir.y;
            _weaponManager.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
        if ((_moveInput.x == 0 && _moveInput.y == 0) && (_playerRb.linearVelocityX != 0) || _playerRb.linearVelocityY != 0)
        {
            _isWalking = false;         //Player is not walking anymore

            //
            Vector3 vector3 = Vector3.left * _lastDir.x + Vector3.down * _lastDir.y;
            _weaponManager.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
        else if (_moveInput.x != 0 && _moveInput.y != 0)
        {
            _isWalking = true;          //Player is walking
        }
    }
}
