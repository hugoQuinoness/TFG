using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public bool isSaving = false;

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
    }

    public void SavePlayerData()
    {
        isSaving = true;
        Debug.Log(SceneManager.GetActiveScene().name);
        SaveGame.Save<string>("Level", SceneManager.GetActiveScene().name);
        PauseManager.Instance.TogglePause();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadAndStart()
    {
        SceneManager.LoadScene(SaveGame.Load<string>("Level"));
    }
}
