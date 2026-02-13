using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    [Header("HUD Panels")]
    [SerializeField] private GameObject gameplayHUD;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
    }
    private void OnEnable()
    {
        LevelManager.instance.SceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (LevelManager.instance != null)
            LevelManager.instance.SceneLoaded -= OnSceneLoaded;
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

}

