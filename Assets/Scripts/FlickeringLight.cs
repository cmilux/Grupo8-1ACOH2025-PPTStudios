using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] Transform flickerLight;
    [SerializeField] Light2D flickerLightComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flickerLight = gameObject.GetComponent<Transform>();
        flickerLightComponent = flickerLight.GetComponent<Light2D>();

        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        for (; ; )
        {
            float randomIntensity = Random.Range(0.25f, 1f);
            flickerLightComponent.intensity = randomIntensity;

            float randomTime = Random.Range(0f, 0.1f);
            yield return new WaitForSeconds(randomTime);
        }
    }
}
