using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Floats")]
    [SerializeField] float _speed = 5f;         //speed

    [Header("PlayerInput")]
    public float _horizontalInput;              //X axis input
    public float _verticalInput;                //Y axis input

    [Header("References")]
    Rigidbody2D _playerRb;      //Player rigidbody
    [SerializeField] SpriteRenderer _spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Sets the player's rigidbody in its variable
        _playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calls PlayerMove() method
        PlayerMov();
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

        PlayerRotation();
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
}
