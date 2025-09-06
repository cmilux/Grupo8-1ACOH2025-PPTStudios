using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Floats")]
    [SerializeField] float _speed = 5f;         //speed

    [Header("PlayerInput")]
    public float _horizontalInput;              //X axis input
    public float _verticalInput;                //Y axis input

    [Header("Rigidbody")]
    Rigidbody2D _playerRb;      //Player rigidbody

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
    }
}
