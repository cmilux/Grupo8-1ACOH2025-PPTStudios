using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _sprayGas;
    [SerializeField] WeaponManager _weaponManager;

    [Header("Variables")]
    [SerializeField] bool _isAttacking = false;
    [SerializeField] float _attackDuration = 3f;
    [SerializeField] float _attackTimer = 0f;

    /*
     * HAY QUE BORRAR:
     * LINEA 45 DE /ENEMY MANAGER/ QUE ELIMINA EL ARMA CUANDO ATACA
     * 
     * Diferenciar en enemymanager el daño que hace el spray de la piedra     
     * 
     * Enemigos no te siguen si estas detras de una comida, como si fuese un obstaculo
     */

    private void Start()
    {
        //Calling the weaponManager script
        _weaponManager = GetComponentInParent<WeaponManager>();
    }
    private void Update()
    {
        //Calling methods
        CheckMeleeTimer();
        OnAttack();
    }

    public void OnAttack()
    {
        if (!_isAttacking)
        {
            //Check if player is attacking (using the spray)
            //Turns on the spray object (later a particle spray?)
            _sprayGas.SetActive(true);
            _isAttacking = true;
        }
    }

    void CheckMeleeTimer()
    {
        if (_isAttacking)
        {
            //If player is attacking, set the timer to real time
            _attackTimer += Time.deltaTime;

            if (_attackTimer >= _attackDuration)
            {
                //If the timer is minor to the duration, set its back to 0
                _attackTimer = 0f;
                _isAttacking = false;               //Sets the attack bool to false again
                _sprayGas.SetActive(false);     //Turns off spray gameObject
                _weaponManager.SwitchToRock();      //Switch to ThrowingRocks attack
            }
        }
        
    }
}
