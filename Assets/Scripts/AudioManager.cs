using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip sndNuevoPremio; // Sonido cuando se obtiene un premio nuevo
    [SerializeField] private AudioClip sndPremioRepetido;// Sonido cuando el premio ya fue obtenido antes
    [SerializeField] private AudioClip sndGachaClick;// Sonido al presionar el botón del gacha

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

    // Reproduce el sonido de premio nuevo
    public void PlayNuevoPremio() => Play(sndNuevoPremio);

    // Reproduce el sonido de premio repetido
    public void PlayPremioRepetido() => Play(sndPremioRepetido);

    // Reproduce el sonido al hacer clic en el gacha
    public void PlayClickGacha() => Play(sndGachaClick);

    // Método general para reproducir cualquier sonido
    private void Play(AudioClip clip)
    {
        if (clip != null)
            source.PlayOneShot(clip);
    }
}
