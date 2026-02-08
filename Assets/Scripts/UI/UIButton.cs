using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private UIButtonSounds sound;
    [SerializeField] private bool isExitButton = false;

    private void Awake()
    {
        sound = GetComponent<UIButtonSounds>();
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            string buttonName = gameObject.name;

            if (isExitButton)
            {
                StartCoroutine(ExitAfterSound());
                return;
            }

            if (LevelManager.isPaused)
            {
                if (buttonName == "Resume_Button")
                {
                    LevelManager.instance.ResumeGame();
                    return;
                }

                if (buttonName == "OptionsPause_Button")
                {
                    LevelManager.instance.ShowOptionsPanel();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(sceneName))
            {
                LevelManager.instance.LoadScene(sceneName, sound.clickSound.length);
            }

        });
    }

    private IEnumerator ExitAfterSound()
    {
        if (sound != null && sound.clickSound != null)
            yield return new WaitForSeconds(sound.clickSound.length);

        LevelManager.instance.ExitGame();
    }

}
