// ===============================
// AUTHOR: Ng Tian Kiat
// CREATE DATE: 13 Feb 2016
// PURPOSE: MainController Architecture
// NOTES: ~
// ===============================
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainController : MonoBehaviour
{
    // Public Members
    public string StartingSceneName;
    public Canvas LoadingScreen;
    public Slider LoadingBar;
    // Private Members
    private static MainController singleton;

    private string m_CurrentSceneName;
    private string m_NextSceneName;

    private bool IsLoadingScreenActive = false;

    private AsyncOperation ResourceUnloadTask;
    private AsyncOperation SceneLoadTask;

    private enum SceneState
    {
        Reset, Preload, Load, Unload, Postload, Ready, Run, Count
    };
    private SceneState m_SceneState;
    private delegate void UpdateDelegate();
    private UpdateDelegate[] UpdateDelegates;

    // -----------------------------------------------------------------------------------------
    // Public static methods
    // -----------------------------------------------------------------------------------------
    public static void LoadScene(string _sceneName)
    {
        if (singleton != null)
        {
            if (singleton.m_CurrentSceneName != _sceneName)
            {
                singleton.m_NextSceneName = _sceneName;
            }
        }
    }

    // -----------------------------------------------------------------------------------------
    // Protected mono methods
    // -----------------------------------------------------------------------------------------
    protected void Awake()
    {
        // Keep this alive between scene changes
        DontDestroyOnLoad(gameObject);

        // Setup the singleton
        singleton = this;

        // Setup array of UpdateDelegates
        UpdateDelegates = new UpdateDelegate[(int)SceneState.Count];

        // Setting up each element in UpdateDelegates
        UpdateDelegates[(int)SceneState.Reset] = UpdateSceneReset;
        UpdateDelegates[(int)SceneState.Preload] = UpdateScenePreload;
        UpdateDelegates[(int)SceneState.Load] = UpdateSceneLoad;
        UpdateDelegates[(int)SceneState.Unload] = UpdateSceneUnload;
        UpdateDelegates[(int)SceneState.Postload] = UpdateScenePostload;
        UpdateDelegates[(int)SceneState.Ready] = UpdateSceneReady;
        UpdateDelegates[(int)SceneState.Run] = UpdateSceneRun;

        // Setup loading screen
        LoadingScreen.enabled = IsLoadingScreenActive;

        if (StartingSceneName != string.Empty)
            m_NextSceneName = StartingSceneName;
        m_SceneState = SceneState.Reset;
    }
    protected void Update()
    {
        if (UpdateDelegates[(int)m_SceneState] != null)
        {
            UpdateDelegates[(int)m_SceneState]();
        }
    }
    // -----------------------------------------------------------------------------------------
    // Private methods
    // -----------------------------------------------------------------------------------------
    // Attach the new scene controller to start the cascade of loads
    private void UpdateSceneReset()
    {
        // Run a Garbage Collection pass
        System.GC.Collect();
        m_SceneState = SceneState.Preload;
    }

    // Handle anything that needs to happen before loading
    private void UpdateScenePreload()
    {
        SceneLoadTask = SceneManager.LoadSceneAsync(m_NextSceneName);
        m_SceneState = SceneState.Load;
    }

    // Handle loading, you can show the loading screen until it's done
    private void UpdateSceneLoad()
    {
        if (!LoadingScreen.enabled)
            LoadingScreen.enabled = true;

        if (SceneLoadTask.isDone)
        {
            m_SceneState = SceneState.Unload;
        }
        else
        {
            // Update the scene loading progress, you can show a progress bar here
            LoadingBar.value = SceneLoadTask.progress;
        }
    }

    // clean up unused resources by unloading them
    private void UpdateSceneUnload()
    {
        // Turn off the loading screen and reset the loading bar
        LoadingScreen.enabled = false;
        LoadingBar.value = 0;
        // Is it cleaning up resources yet?
        if (ResourceUnloadTask == null)
        {
            ResourceUnloadTask = Resources.UnloadUnusedAssets();
        }
        else
        {
            // Done cleaning up?
            if (ResourceUnloadTask.isDone)
            {
                ResourceUnloadTask = null;
                m_SceneState = SceneState.Postload;
            }
        }
    }

    // Handle anything that needs to happen immediately after loading
    private void UpdateScenePostload()
    {
        m_CurrentSceneName = m_NextSceneName;
        m_SceneState = SceneState.Ready;
    }

    // Handle anything that needs to happen immediately before running
    private void UpdateSceneReady()
    {
        // Run a GC pass
        // if you have assets loaded in the scene that are
        // currently unused but may be used later
        // Don't do this here

        System.GC.Collect();
        m_SceneState = SceneState.Run;
    }

    // Wait here for scene changes
    private void UpdateSceneRun()
    {
        if (m_CurrentSceneName != m_NextSceneName)
        {
            m_SceneState = SceneState.Reset;
        }
    }

    protected void OnDestroy()
    {
        // Clean up the UpdateDelegates
        if (UpdateDelegates != null)
        {
            for (int i = 0; i < (int)SceneState.Count; i++)
            {
                UpdateDelegates[i] = null;
            }
            UpdateDelegates = null;
        }
        // Clean up the singleton instance
        if (singleton != null)
        {
            singleton = null;
        }
    }
}
