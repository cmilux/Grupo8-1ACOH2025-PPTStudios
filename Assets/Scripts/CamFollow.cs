using Unity.Cinemachine;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private CinemachineCamera cam;

    private void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        cam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
