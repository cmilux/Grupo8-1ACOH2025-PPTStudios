using UnityEngine;

public class PlayerAttackMelee : MonoBehaviour
{
    [SerializeField] Transform _radiusT;

    [SerializeField] float _radiusD = 2f;
    
    /*
     * no se como hacer para que solo lo destruya/haga daño presionando un boton/tecla
     * intente nonstop
    */
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            Destroy(collider.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_radiusT.position, _radiusD);
    }
}
