using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource musicSource;
    private AudioClip bgm;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0.6f; // volumen inicial
    }

    // Cargar música desde Resources/Music
    public void LoadMusic(string fileName)
    {
        bgm = Resources.Load<AudioClip>("Music/" + fileName);

        if (bgm == null)
        {
            Debug.LogError("No se encontró la música: " + fileName);
            return;
        }

        musicSource.clip = bgm;
    }

    public void Play()
    {
        if (musicSource.clip == null)
        {
            Debug.LogWarning("No hay música cargada aún");
            return;
        }
        musicSource.Play();
    }

    public void Stop()
    {
        musicSource.Stop();
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }
}
