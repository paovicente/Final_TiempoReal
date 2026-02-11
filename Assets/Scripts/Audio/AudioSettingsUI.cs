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
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private void OnEnable()
    {
        StartCoroutine(SyncSliders());
    }

    private IEnumerator SyncSliders()
    {
        yield return null;

        if (AudioManager.instance == null) yield break;

        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        playerSlider.onValueChanged.RemoveAllListeners();
        
        musicToggle.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.RemoveAllListeners();

        //to avoid a call to On Value Changed
        musicSlider.SetValueWithoutNotify(AudioManager.instance.GetMusic());
        sfxSlider.SetValueWithoutNotify(AudioManager.instance.GetSFX());
        playerSlider.SetValueWithoutNotify(AudioManager.instance.GetPlayerSound());
        
        musicToggle.SetIsOnWithoutNotify(AudioManager.instance.IsMusicEnabled());
        sfxToggle.SetIsOnWithoutNotify(AudioManager.instance.IsSFXEnabled());

        //listeners for sliders and toggles
        musicSlider.onValueChanged.AddListener(AudioManager.instance.SetMusic);
        sfxSlider.onValueChanged.AddListener(AudioManager.instance.SetSFX);
        playerSlider.onValueChanged.AddListener(AudioManager.instance.SetPlayerSound);
        
        musicToggle.onValueChanged.AddListener(AudioManager.instance.SetMusicEnabled);
        sfxToggle.onValueChanged.AddListener(AudioManager.instance.SetSFXEnabled);

        //to deactivate the slider if music/sfx is deactivate
        musicSlider.interactable = musicToggle.isOn;
        musicToggle.onValueChanged.AddListener(value => musicSlider.interactable = value);

        sfxSlider.interactable = sfxToggle.isOn;
        sfxToggle.onValueChanged.AddListener(value => sfxSlider.interactable = value);
    }

}