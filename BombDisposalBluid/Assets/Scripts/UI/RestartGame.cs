using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
       public void Restart()
    {
        // Zet de tijd weer op normaal voordat de gameplay-scene opnieuw laadt.
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}