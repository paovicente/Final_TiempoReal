using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    [Header("HUD Panels")]
    [SerializeField] private GameObject gameplayHUD;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsLevelScene(scene.name))
        {
            gameplayHUD.SetActive(true);
        }
        else
        {
            gameplayHUD.SetActive(false);
        }
    }

    private bool IsLevelScene(string sceneName)
    {
        if (sceneName == "LevelSelector") //dont activate the HUD if the scene is LevelSelector
            return false;

        return sceneName.StartsWith("Level");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

