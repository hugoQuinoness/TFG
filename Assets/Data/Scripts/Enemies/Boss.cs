using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour, IEnemy
{
    public static Boss Instance;

    [Header("Cinemachine")]
    public Cinemachine.CinemachineVirtualCamera virtualCamera;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Dialogue")]
    public string onDeathDialogueAddress;

    private Animator animator;
    private WizardMovement bossMovement;
    private EnemyHealth enemyHealth;
    private AudioSource audioSource;
    private bool canBeHit = true;

    public Sprite sprite;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else 
        {
            Destroy(gameObject);
        } 
        

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        bossMovement = GetComponent<WizardMovement>();
        virtualCamera = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();

        TryGetComponent(out enemyHealth);

        DialogueManager.InvokeOnDialogueEnter += OnEnterDialogueMode;
        DialogueManager.InvokeOnDialogueExit += OnExitDialogueMode;
    }

    void OnDestroy()
    {
        DialogueManager.InvokeOnDialogueEnter -= OnEnterDialogueMode;
        DialogueManager.InvokeOnDialogueExit  -= OnExitDialogueMode;
    }

    private void OnEnterDialogueMode()
    {
        bossMovement.enabled = false;
    }

    private void OnExitDialogueMode()
    {
        bossMovement.enabled = true;
    }

    public void ChangeCameraPriority()
    {
        if (CMCameraManager.Instance.currentCinemachineVCamera == null)
        {
            CMCameraManager.Instance.currentCinemachineVCamera = virtualCamera;
        }
        CMCameraManager.Instance.ChangeVcamPriority(virtualCamera);
    }

    public void PlayAnimation(string animation)
    {
        animator.Play(animation);
    }

    public IEnumerator MoveTo(float x, float y, float duration)
    {
        Vector2 startPos = transform.position;
        Vector2 endPos   = new Vector2(x, y);
        float elapsed    = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        DialogueManager.Instance.tagsToHandle--;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("PlayerDamageCollider") || enemyHealth == null)
            return;

        var dmg = col.GetComponent<PlayerWeapon>().damage;

        enemyHealth.health -= dmg;

        if (enemyHealth.health <= 0)
        {
            if (canBeHit)
            {
                StartCoroutine(HandleDeath());
            } 
        }
        else
        {
            if (canBeHit)
            {
                StartCoroutine(HandleHit());
            } 
        }
    }

    private IEnumerator HandleDeath()
    {
        canBeHit = false;
        bossMovement.StopMovement();
        animator.Play("Death");
        audioSource.PlayOneShot(deathSound);
        yield return DialogueManager.Instance.StartDialogueByAdress(onDeathDialogueAddress);
    }

    private IEnumerator HandleHit()
    {
        canBeHit = false;
        bossMovement.StopMovement();
        audioSource.PlayOneShot(hitSound);
        animator.Play("TakeHit");

        var sr = GetComponent<SpriteRenderer>();
        Color original = sr.color;
        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = original;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        animator.Play("Idle");
        canBeHit = true;
        bossMovement.ResumeMovement();
    }

    public void OnPlayerDetected()
    {
        throw new System.NotImplementedException();
    }
}
