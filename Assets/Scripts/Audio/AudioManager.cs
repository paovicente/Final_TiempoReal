using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Mixer")]
    public AudioMixer masterMixer;

    [Header("Default Values")]
    [SerializeField] private float defaultMusic = 0.5f;
    [SerializeField] private float defaultSFX = 1f;
    [SerializeField] private float defaultPlayer = 1f;

    private float currentMusic;
    private float currentSFX;
    private float currentPlayer;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string PLAYER_KEY = "PlayerVolume";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadVolumes();
            ApplyVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolumes()
    {
        currentMusic = PlayerPrefs.GetFloat(MUSIC_KEY, defaultMusic);
        currentSFX = PlayerPrefs.GetFloat(SFX_KEY, defaultSFX);
        currentPlayer = PlayerPrefs.GetFloat(PLAYER_KEY, defaultPlayer);
    }

    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, currentMusic);
        PlayerPrefs.SetFloat(SFX_KEY, currentSFX);
        PlayerPrefs.SetFloat(PLAYER_KEY, currentPlayer);
    }

    private void ApplyVolumes()
    {
        masterMixer.SetFloat("MusicVolume", ToMixerValue(currentMusic));
        masterMixer.SetFloat("SFXVolume", ToMixerValue(currentSFX));
        masterMixer.SetFloat("PlayerVolume", ToMixerValue(currentPlayer));
    }

    private float ToMixerValue(float value)
    {
        //to avoid Log10(0)
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void SetMusic(float value)
    {
        currentMusic = value;
        SaveVolumes();
        ApplyVolumes();
    }

    public void SetSFX(float value)
    {
        currentSFX = value;
        SaveVolumes();
        ApplyVolumes();
    }

    public void SetPlayerSound(float value)
    {
        currentPlayer = value;
        SaveVolumes();
        ApplyVolumes();
    }

    //lets the UI read the current values
    public float GetMusic() => currentMusic;
    public float GetSFX() => currentSFX;
    public float GetPlayerSound() => currentPlayer;
}
