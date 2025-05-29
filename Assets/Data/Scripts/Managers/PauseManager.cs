using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public GameObject pauseMenu;
    public GameObject firstSelected;
    public bool isPaused = false;
    private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        playerInput = EventSystem.current.GetComponent<PlayerInput>();

        EventManager.PauseEvent += TogglePause;
        
    }

    private void OnDestroy()
    { 
        EventManager.PauseEvent -= TogglePause;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            Time.timeScale = 0f;
            PlayerControler.Instance.canMove = false;
            EventSystem.current.SetSelectedGameObject(firstSelected);
            
        }
        else
        {
            PlayerControler.Instance.canMove = true;
            Time.timeScale = 1f;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
