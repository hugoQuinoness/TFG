using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public Transform player;
    public float maxSpeed = 1.8f;
    public float chaseDistance = 10f;
    public float minDistance = 0.5f;

    public float enemyAttackCooldown;

    public GameObject enemyAttackHitbox;

    private Animator animator;
    private Vector3 originalScale;
    private AudioSource audioSource;
    private bool canAttack = true;

    public bool canMove = true;

    public AudioClip deathSound;


    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        player = Player.Instance.transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!canMove)
        {
            return;
        }


        float distance = Vector3.Distance(transform.position, player.position);

        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        if (distance < chaseDistance && distance > minDistance)
        {
            float currentSpeed = maxSpeed;

            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * currentSpeed * Time.deltaTime;

            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        if (distance <= minDistance && canAttack)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        canAttack = false;
        canMove = false;
        animator.SetTrigger("Attack");
        if (enemyAttackHitbox != null)
        {
            enemyAttackHitbox.SetActive(true);
        }
        yield return new WaitForSeconds(enemyAttackCooldown);

        if (enemyAttackHitbox != null)
        {
            enemyAttackHitbox.SetActive(false);
        }
        canAttack = true;
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDamageCollider"))
        {
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        canMove = false;
        animator.Play("Death");
        audioSource.PlayOneShot(deathSound);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    private void UpdateSFXAudioVolume(float volume)
    {
        audioSource.volume = volume;
    }
}