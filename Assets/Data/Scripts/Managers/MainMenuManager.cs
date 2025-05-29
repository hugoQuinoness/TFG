using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager Instance;

    public bool isTransitioning;

    public Animator transitionPanel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        Addressables.InitializeAsync();
    }
       

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
        {
            return;
        }
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadSceneNoAsync(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void DebugButtonPressed()
    {
        Debug.Log("Debug button pressed");
    }

    public void OpenOptionsMenu()
    {
        if (isTransitioning)
        {
            return;
        }
        StartCoroutine(OpenOptionMenuAsync());
    }


    public IEnumerator LoadSceneAsync(string sceneName)
    {       
        transitionPanel.Play("fade-in");

        isTransitioning = true;

        yield return new WaitForSeconds(1.5f);
        
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator OpenOptionMenuAsync()
    {
        if(isTransitioning)
        {
            yield break;
        }


    }

    public void QuitGame()
    {
        if (isTransitioning)
        {
            return;
        }

        Application.Quit();
    }
}
