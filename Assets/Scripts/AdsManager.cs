using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{
    public static AdsManager instance;

    [SerializeField] private string android_ID;
    [SerializeField] private string iOS_ID;
    [SerializeField] private bool testMode = true;

    private string placementID = "Rewarded_";

    [SerializeField] private Gacha gachaManager;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE_WIN
        Advertisement.Initialize(android_ID, testMode, this);
        placementID += "Android";
#elif UNITY_IOS
        Advertisement.Initialize(iOS_ID, testMode, this);
        placementID += "iOS";
#endif
    }

    public void ShowRewardedAd()
    {
        Advertisement.Load(placementID, this);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log("El anuncio ha fallado al mostrarse");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("El anuncio se ha iniciado");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("El anuncio se ha clickeado");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Anuncio completado, otorgando recompensa");
            if (gachaManager != null)
            {
                gachaManager.DarMonedas(3);
            }
        }
        else
        {
            Debug.Log("Anuncio cerrado antes de completarse");
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads inicializado correctamente");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization fallo: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(placementID, this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError("Error al cargar anuncio: " + message);
    }
}