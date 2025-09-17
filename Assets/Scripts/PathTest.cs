using UnityEngine;
using UnityEngine.AI;

public class PathTest : MonoBehaviour
{
    [SerializeField] Transform _target;
    NavMeshAgent _agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        _agent.SetDestination(_target.position);
    }
}
