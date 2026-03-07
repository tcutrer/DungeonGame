using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Nessecary Components
public class MenuScriptOpen : MonoBehaviour
{
    public void play()
    {

        SceneManager.LoadScene("GameScene_GridMap");

    }
    


    // Public parameterless method so it can be wired to a UI Button OnClick

}
