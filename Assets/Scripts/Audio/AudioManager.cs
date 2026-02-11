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
    private const string MUSIC_ENABLED_KEY = "MusicEnabled";
    private const string SFX_ENABLED_KEY = "SFXEnabled";
    private bool musicEnabled = true;
    private bool sfxEnabled = true;

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

        musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
    }

    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, currentMusic);
        PlayerPrefs.SetFloat(SFX_KEY, currentSFX);
        PlayerPrefs.SetFloat(PLAYER_KEY, currentPlayer);

        PlayerPrefs.Save();
    }

    private void ApplyVolumes()
    {
        if (musicEnabled)
            masterMixer.SetFloat("MusicVolume", ToMixerValue(currentMusic));
        else
            masterMixer.SetFloat("MusicVolume", -80f); //mute
        
        if (sfxEnabled)
            masterMixer.SetFloat("SFXVolume", ToMixerValue(currentSFX));
        else
            masterMixer.SetFloat("SFXVolume", -80f); //mute

        masterMixer.SetFloat("PlayerVolume", ToMixerValue(currentPlayer));

        Debug.Log("MusicEnabled: " + musicEnabled);
        Debug.Log("SFXEnabled: " + sfxEnabled);
    }

    private float ToMixerValue(float value)
    {
        //to avoid Log10(0)
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public bool IsMusicEnabled()
    {
        return musicEnabled;
    }

    public void SetSFXEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        PlayerPrefs.SetInt(SFX_ENABLED_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public bool IsSFXEnabled()
    {
        return sfxEnabled;
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
