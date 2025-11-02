using System.Collections;
using UnityEngine;

public class BossTentacleManager : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] public Animator tentacleAnimator;
    [SerializeField] public BossManager bossManager;
    [SerializeField] bool _sequenceStarted;
    [SerializeField] public bool tentacleStartAnim;
    [SerializeField] public bool tentacleLoopAnim;
    [SerializeField] public bool tentacleFinishAnim = false;
    [SerializeField] public int tentacleDamage;

    [Header("SFX")]
    [SerializeField] AudioSource _tentacleAudioSource;
    [SerializeField] AudioClip _meleeTentacle;

    private void Start()
    {
        tentacleAnimator = GetComponent<Animator>();
        _tentacleAudioSource = GetComponent<AudioSource>();
        StartCoroutine(TentacleSFX());
        bossManager = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossManager>();
    }

    private void Update()
    {
        HandleTentacleAnimations();
    }

    void HandleTentacleAnimations()
    {
        tentacleAnimator.SetBool("Loop", tentacleLoopAnim);
        tentacleAnimator.SetBool("Finish", tentacleFinishAnim);

        if (tentacleLoopAnim == true && !_sequenceStarted)
        {
            _sequenceStarted = true;
            StartCoroutine(TriggerTentacleAnimation());
        }
    }

    public IEnumerator TriggerTentacleAnimation()
    {
        yield return new WaitForSeconds(3.2f);
        tentacleAnimator.Play("Tentacle Finish");
        tentacleLoopAnim = false;
        yield return new WaitForSeconds(0.9f);
        _sequenceStarted = false;
    }

    public IEnumerator TentacleSFX()
    {
        yield return new WaitForSeconds(0.5f);
        _tentacleAudioSource.PlayOneShot(_meleeTentacle);
    }
}
