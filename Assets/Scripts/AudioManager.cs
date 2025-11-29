using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip sndNuevoPremio;
    [SerializeField] private AudioClip sndPremioRepetido;
    [SerializeField] private AudioClip sndGachaClick;

    private AudioSource source;

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

        source = GetComponent<AudioSource>();
    }

    public void PlayNuevoPremio() => Play(sndNuevoPremio);
    public void PlayPremioRepetido() => Play(sndPremioRepetido);
    public void PlayClickGacha() => Play(sndGachaClick);

    private void Play(AudioClip clip)
    {
        if (clip != null)
            source.PlayOneShot(clip);
    }
}
