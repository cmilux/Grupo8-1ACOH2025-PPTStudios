using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Inputs and speed")]
    [SerializeField] public float _horizontalInput;              //X axis input
    [SerializeField] public float _verticalInput;                //Y axis input
    [SerializeField] float _speed = 5f;         //speed

    [Header("References")]
    Rigidbody2D _playerRb;      //Player rigidbody
    [SerializeField] SpriteRenderer _spriteRenderer;    //player sprite
    [SerializeField] Animator animator;         //player animator
    [SerializeField] Transform _weaponManager;
    [SerializeField] Vector2 _lastDir;
    [SerializeField] Vector2 _moveDir;

    [Header("Is player walking?")]
    bool _isWalking = false;

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
        PlayerMov();
        PlayerRotation();
    }
    private void LateUpdate()
    {
        StoreLastMove();
    }

    //Player movement on X and Y axis
    void PlayerMov()
    {
        //Gets the input
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        //Adds speed to player movement
        _playerRb.linearVelocityX = _horizontalInput * _speed;
        _playerRb.linearVelocityY = _verticalInput * _speed;

        //Set the right animation depending on axis
        animator.SetFloat("Horizontal", _lastDir.x);
        animator.SetFloat("Vertical", _lastDir.y);
        animator.SetFloat("Speed", _speed);
    }

    void StoreLastMove()
    {
        //Saves the player last direction
        _moveDir = new Vector2(_horizontalInput, _verticalInput);
        if (_moveDir != Vector2.zero)
        {
            _lastDir = _moveDir;
        }
    }

    public void PlayerRotation()
    {
        //Flips the sprite depending on movement direction
        if (_horizontalInput > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_horizontalInput < 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    void IsPlayerMoving()
    {
        if (_isWalking)
        {
            //Check if player is walking, rotate the weapon to the player's direction
            Vector3 vector3 = Vector3.left * _lastDir.x + Vector3.down * _lastDir.y;
            _weaponManager.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }

        if ((_horizontalInput == 0 && _verticalInput == 0) && (_playerRb.linearVelocityX != 0 || _playerRb.linearVelocityY != 0))
        {
            //Check if player is moving and sets it's last position
            _isWalking = false;

            Vector3 vector3 = Vector3.left * _lastDir.x + Vector3.down * _lastDir.y;
            _weaponManager.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
        else if (_horizontalInput != 0 && _verticalInput != 0)
        {
            //Checks if player input is different than 0
            _isWalking = true;
        }
    }
}
