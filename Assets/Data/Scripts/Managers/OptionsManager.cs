using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public GameObject optionsPanel;

    public GameObject mainMenuPanel;

    public GameObject firstSelectedMainMenu;

    public GameObject firstSelectedOptions;

    public Slider volumeSlider;

    public static OptionsManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ReturnToMainMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMainMenu);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedOptions);
    }


}
