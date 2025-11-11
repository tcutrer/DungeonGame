using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Nessecary Components
public class Menu_Script : MonoBehaviour
{
    public void play()
    {
# if UNITY_EDITOR
        SceneManager.LoadScene("GameScene_GridMap");
#else
        SceneManager.LoadScene("GameScene_GridMap");
#endif
    }
    
    public void options()
    {
# if UNITY_EDITOR
        EditorSceneManager.LoadScene("Options");
#else
        SceneManager.LoadScene("Options");
#endif
    }

    // Public parameterless method so it can be wired to a UI Button OnClick
    public void quit()
    {
#if UNITY_EDITOR
        // In the Editor Application.Quit does nothing. Stop play mode so the button is testable during development.
        EditorApplication.isPlaying = false;
#else
        // In a build this will close the application.
        Application.Quit();
#endif
    }
}
