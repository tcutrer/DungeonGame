using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Nessecary Components
public class MenuScriptOpen : MonoBehaviour
{
    public void play()
    {
# if UNITY_EDITOR
        SceneManager.LoadScene("GameScene_GridMap");
#else
        SceneManager.LoadScene("GameScene_GridMap");
#endif
    }
    


    // Public parameterless method so it can be wired to a UI Button OnClick

}
