using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    float _xRangeMin = -9.05f;
    float _xRangeMax = 9.05f;
    float _yRangeMax = 21.85f;
    float _yRangeMin = -1.8f;

    private void Update()
    {
        ScreenLimit();
    }
    public void ScreenLimit()
    {
        //Camera left X axis limit
        if (gameObject.transform.position.x < _xRangeMin)
        {

            gameObject.transform.position = new Vector3(_xRangeMin, gameObject.transform.position.y, -10);
        }
        //Camera right X axis limit
        if (gameObject.transform.position.x > _xRangeMax)
        {
            gameObject.transform.position = new Vector3(_xRangeMax, gameObject.transform.position.y, -10);
        }

        //Camera right Y axis limit
        if (gameObject.transform.position.y > _yRangeMax)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, _yRangeMax, -10);
        }
        //Camera left Y axis limit
        if (gameObject.transform.position.y < _yRangeMin)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, _yRangeMin, -10);
        }
    }
}
