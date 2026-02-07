using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager instance;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference startAction;
    [SerializeField] private InputActionReference pauseAction;

    [Header("References")]
    [SerializeField] private string startScreenScene = "StartScreen";
    [SerializeField] private string menuScene = "Menu";
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Camera fallbackCamera;
    
    private Scene activeScene;
    public bool isPaused = false;

    private void Awake()
    {
        PlayerPrefs.DeleteKey("PlayerHealth");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //when the game starts, this load the start screen instantly
        SceneManager.LoadSceneAsync(startScreenScene, LoadSceneMode.Additive);
       
    }
    
    private void Start()
    {
        activeScene = SceneManager.GetSceneByName(startScreenScene);
        SceneManager.SetActiveScene(activeScene);

        if (startAction != null)
        {
            startAction.action.Enable();
            startAction.action.performed += GoToMenu;
        }
        
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += GoToPause;
        }
    }

    private void GoToMenu(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name == startScreenScene)
        {
            LoadScene(menuScene);
            startAction.action.Disable();
        }
    }

    private void GoToPause(InputAction.CallbackContext context)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (!currentScene.Contains("Level") || currentScene.Contains("Selector"))
            return;

        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void LoadScene(string sceneName, float delay = 0.2f)
    {
        StartCoroutine(LoadSceneDelayed(sceneName, delay));
    }

    private IEnumerator LoadSceneDelayed(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);

        Scene previousScene = SceneManager.GetActiveScene();

        //load the new scene
        var scene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        yield return scene;

        if (fallbackCamera != null)
            fallbackCamera.enabled = false;

        activeScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(activeScene);

        Debug.Log("previous scene: " + previousScene.name);
        yield return UnloadAllScenesExceptBoot();
    }

    private IEnumerator UnloadAllScenesExceptBoot()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);

            if (s.name != "Boot" && s.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(s);
            }
        }
    }
    public void LoadNextLevel(float delay = 0.2f)
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        string scenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);

        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogWarning("The game doesn't have next level.");
            return;
        }

        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        LoadScene(sceneName, delay);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        foreach (Animator anim in pausePanel.GetComponentsInChildren<Animator>())
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;

        PlayerPauseHandler playerHandler = GetPlayerHandler();
        if (playerHandler != null)
            playerHandler.PausePlayer();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        //Debug.Log("RESUME GAME");
        if (pausePanel != null)
            pausePanel.SetActive(false);

        PlayerPauseHandler playerHandler = GetPlayerHandler();
        if (playerHandler != null)
            playerHandler.ResumePlayer();
    }

    public void ReturnToMenuFromPause(float delay = 0f)
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

       StartCoroutine(LoadSceneDelayed(menuScene, delay));
    }

    private PlayerPauseHandler GetPlayerHandler()
    {
        return FindFirstObjectByType<PlayerPauseHandler>();
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game...");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
}

