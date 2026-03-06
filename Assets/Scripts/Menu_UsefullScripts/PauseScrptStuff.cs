using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Nessecary Components
public class PauseScriptStuff : MonoBehaviour
{

    


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
