using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCanvas : MonoBehaviour
{
    private Animator animator;

    private float deathAnimationLength;

    [Header("References")]
    public TextMeshProUGUI deathText;

    public AnimationClip deathAnimation;



    public void Awake()
    {
        animator = GetComponent<Animator>();
        PlayerHealth.OnDeath += PlayerDeath;
        deathAnimationLength = deathAnimation.length;
    }

    private void OnDestroy()
    {
        PlayerHealth.OnDeath -= PlayerDeath;
    }


    private void PlayerDeath()
    {
        StartCoroutine(PlayerDeathCoroutine());
    }

    private IEnumerator PlayerDeathCoroutine()
    {

        yield return new WaitForSeconds(2f); // Delay before playing death animation

        animator.Play("OnDeath");

        yield return new WaitForSeconds(deathAnimationLength);

        StartCoroutine(TypeTextEffect("You have died!"));
    }




    private IEnumerator TypeTextEffect(string text)
    {
        deathText.text = "";

        int charIndex = 0;

        foreach (char c in text.ToCharArray())
        {
            deathText.text += c;

            if (charIndex % 2 == 0)
            {
                SFXManager.Instance.PlayTypingSFX();
            }

            charIndex++;
            yield return new WaitForSeconds(0.3f);
        }

        SceneManager.LoadScene("MainMenu");

    }

}
