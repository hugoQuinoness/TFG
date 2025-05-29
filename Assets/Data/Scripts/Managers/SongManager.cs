using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SongManager : MonoBehaviour
{
    private AudioSource audioSource;

    public static SongManager Instance;

    private AsyncOperationHandle<AudioClip>? currentHandle;

    private Slider volumeSlider;

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

        audioSource = GetComponent<AudioSource>();

        if(OptionsManager.Instance != null)
        {
            volumeSlider = OptionsManager.Instance.volumeSlider;
        }
    }

    public void PlaySongFromAddressable(string song)
    {
        StartCoroutine(PlaySong(song));
        DialogueManager.Instance.tagsToHandle--;
    }

    public void PlaySongFromAddressableOnce(string song)
    {
        StartCoroutine(PlaySongOnce(song));
        DialogueManager.Instance.tagsToHandle--;
    }

    public IEnumerator PlaySong(string address)
    {

        if (!audioSource.loop)
        {
            audioSource.loop = true;
        }

        if (audioSource.isPlaying && audioSource.volume > 0f)
        {
            float startVolume = audioSource.volume;
            float fadeOutDuration = 1.5f;
            float t = 0f;
            while (t < fadeOutDuration)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
                t += Time.deltaTime;
                yield return null;
            }
            audioSource.volume = 0f;
            audioSource.Stop();
        }

        if (currentHandle.HasValue && currentHandle.Value.IsValid())
        {
            Addressables.Release(currentHandle.Value);
        }

        currentHandle = Addressables.LoadAssetAsync<AudioClip>(address);

        while (!currentHandle.Value.IsDone)
        {
            yield return null;
        }

        if (currentHandle.Value.Status != AsyncOperationStatus.Succeeded || currentHandle.Value.Result == null)
        {
            yield break;
        }

        audioSource.clip = currentHandle.Value.Result;
        audioSource.Play();

        float fadeInDuration = 0.7f;
        float t2 = 0f;
        float maxVolume = 0.1f;
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            maxVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        while (t2 < fadeInDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, maxVolume, t2 / fadeInDuration);
            t2 += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = maxVolume;
    }
    
    public IEnumerator PlaySongOnce(string address)
    {

        audioSource.loop = false;

        if (audioSource.isPlaying && audioSource.volume > 0f)
        {
            float startVolume = audioSource.volume;
            float fadeOutDuration = 1.5f;
            float t = 0f;
            while (t < fadeOutDuration)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
                t += Time.deltaTime;
                yield return null;
            }
            audioSource.volume = 0f;
            audioSource.Stop();
        }

        if (currentHandle.HasValue && currentHandle.Value.IsValid())
        {
            Addressables.Release(currentHandle.Value);
        }

        currentHandle = Addressables.LoadAssetAsync<AudioClip>(address);

        while (!currentHandle.Value.IsDone)
        {
            yield return null;
        }

        if (currentHandle.Value.Status != AsyncOperationStatus.Succeeded || currentHandle.Value.Result == null)
        {
            yield break;
        }

        audioSource.clip = currentHandle.Value.Result;
        audioSource.Play();

        float fadeInDuration = 0.7f;
        float t2 = 0f;
        float maxVolume = 0.1f;
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            maxVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        while (t2 < fadeInDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, maxVolume, t2 / fadeInDuration);
            t2 += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = maxVolume;
    }

    public void ChangeVolume()
    {
        if (volumeSlider != null)
        {
            audioSource.volume = volumeSlider.value;
        }

        SaveVolume();
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", audioSource.volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume");

            if (volumeSlider != null)
            {
                volumeSlider.value = audioSource.volume;
            }

        }
        else
        {
            audioSource.volume = 0.3f;
            if (volumeSlider != null)
            {
                volumeSlider.value = audioSource.volume;
            }
        }
    }

    private void OnEnable()
    {
        LoadVolume();
        ChangeVolume();
    }
}
