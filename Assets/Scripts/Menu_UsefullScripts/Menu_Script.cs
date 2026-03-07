using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

// Nessecary Components
public class Menu_Script : MonoBehaviour
{
    public void play()
    {

        SceneManager.LoadScene("Open");

    }
    
    public void options()
    {

        SceneManager.LoadScene("Options");

    }

    // Public parameterless method so it can be wired to a UI Button OnClick
    public void quit()
    {
        // In a build this will close the application.
        Application.Quit();

    }
}
