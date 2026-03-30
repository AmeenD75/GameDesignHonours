using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        // 1. Singleton Check: If a music player already exists, kill the new one
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 2. Make this one the "Master" and don't destroy it when changing scenes
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void PlayMusic()
    {
        if (instance != null && !instance.audioSource.isPlaying)
        {
            instance.audioSource.Play();
        }
    }

    // 3. Static method so you can call it from ANY script (like your Battle Trigger)
    public static void StopMusic()
    {
        if (instance != null)
        {
            instance.audioSource.Stop();
            // Optional: Destroy(instance.gameObject); if you want it gone forever
        }
    }
}