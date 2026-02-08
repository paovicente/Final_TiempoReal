/*using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider playerSlider;

    private void Start()
    {
        musicSlider.value = AudioManager.instance.GetMusic();
        sfxSlider.value = AudioManager.instance.GetSFX();
        playerSlider.value = AudioManager.instance.GetPlayerSound();

        musicSlider.onValueChanged.AddListener(volume => AudioManager.instance.SetMusic(volume));
        sfxSlider.onValueChanged.AddListener(volume => AudioManager.instance.SetSFX(volume));
        playerSlider.onValueChanged.AddListener(volume => AudioManager.instance.SetPlayerSound(volume));
    }
}*/
/*using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider playerSlider;

    private void OnEnable()
    {
        //to synchronize sliders with current values
        musicSlider.value = AudioManager.instance.GetMusic();
        sfxSlider.value = AudioManager.instance.GetSFX();
        playerSlider.value = AudioManager.instance.GetPlayerSound();

        //to avoid duplicate listeners
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        playerSlider.onValueChanged.RemoveAllListeners();

        //connects sliders to audio manager
        musicSlider.onValueChanged.AddListener(AudioManager.instance.SetMusic);
        sfxSlider.onValueChanged.AddListener(AudioManager.instance.SetSFX);
        playerSlider.onValueChanged.AddListener(AudioManager.instance.SetPlayerSound);
    }
}*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider playerSlider;

    private void OnEnable()
    {
        StartCoroutine(SyncSliders());
    }

    private IEnumerator SyncSliders()
    {
        //to avoid that the UI doesnt reflects the sound values previously set
        yield return null;

        if (AudioManager.instance == null) yield break;

        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        playerSlider.onValueChanged.RemoveAllListeners();

        musicSlider.SetValueWithoutNotify(AudioManager.instance.GetMusic());
        sfxSlider.SetValueWithoutNotify(AudioManager.instance.GetSFX());
        playerSlider.SetValueWithoutNotify(AudioManager.instance.GetPlayerSound());

        musicSlider.onValueChanged.AddListener(AudioManager.instance.SetMusic);
        sfxSlider.onValueChanged.AddListener(AudioManager.instance.SetSFX);
        playerSlider.onValueChanged.AddListener(AudioManager.instance.SetPlayerSound);
    }
}