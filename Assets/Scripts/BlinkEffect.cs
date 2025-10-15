using System.Collections;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] float _blinkDuration = 2f;             //Blink effect duration
    [SerializeField] Color _ogColor;                        //Sprite og color
    [SerializeField] SpriteRenderer _spriteRenderer;        //Sprite renderer


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();       //Get the sprite renderer
        _ogColor = _spriteRenderer.color;                       //Sets the og color to the variable
    }

    // Update is called once per frame
    void Update()
    {
        Blink();   
    }

    void Blink()
    {
        //Set the transparency to create a fade in and out
        //Mathf.Sin --> Creates a wave that goes between -1 and 1
        //Math.Abs --> Makes it positive so it goes 0-1 0-1
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * _blinkDuration));
        //Apply the alpha (transparency) to the sprite
        //Keeps the rgb values just changes the alpha
        _spriteRenderer.color = new Color(_ogColor.r, _ogColor.g, _ogColor.b, alpha);
    }
}
