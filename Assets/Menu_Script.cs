using UnityEngine;
using UnityEngine.SceneManagement;
//Nessecary Components

public class Menu_Script : MonoBehaviour
{
      public void play()
      {
              SceneManager.LoadScene("Game_Scene");
      }
      public void quit()
      {
              Application.Quit();
      }
}
