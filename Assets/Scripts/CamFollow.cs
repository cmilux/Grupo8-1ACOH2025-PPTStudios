using Unity.Cinemachine;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private CinemachineCamera cam;      //Cinemachine reference

    private void Start()
    {
        //Get the cinemachine camera
        cam = GetComponent<CinemachineCamera>();
        //Set to follow the player on every scene
        cam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
