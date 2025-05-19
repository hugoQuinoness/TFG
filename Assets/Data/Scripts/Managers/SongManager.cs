using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SongManager : MonoBehaviour
{
    private AudioSource audioSource;

    public static SongManager Instance;

    private AsyncOperationHandle<AudioClip>? currentHandle;

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
    }

    public void PlaySongFromAddressable(string song)
    {
        StartCoroutine(PlaySong(song));
        DialogueManager.Instance.tagsToHandle--;
    }

    public IEnumerator PlaySong(string address)
    {

        Debug.Log($"[SongManager] Starting to load addressable: {address}");

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
            Debug.Log("[SongManager] Releasing previous handle");
            Addressables.Release(currentHandle.Value);
        }

        currentHandle = Addressables.LoadAssetAsync<AudioClip>(address);

        if (!currentHandle.HasValue)
        {
            Debug.LogError("[SongManager] currentHandle is null after LoadAssetAsync!");
            yield break;
        }

        yield return currentHandle.Value;

        if (currentHandle.Value.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[SongManager] Failed to load addressable: {address}");
            yield break;
        }

        if (currentHandle.Value.Result == null)
        {
            Debug.LogError($"[SongManager] Loaded AudioClip is null for address: {address}");
            yield break;
        }

        Debug.Log($"[SongManager] Successfully loaded song: {currentHandle.Value.Result.name}");

        audioSource.clip = currentHandle.Value.Result;
        audioSource.Play();

        float fadeInDuration = 0.7f;
        float t2 = 0f;
        while (t2 < fadeInDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, 0.1f, t2 / fadeInDuration);
            t2 += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0.1f;
    }
}
    