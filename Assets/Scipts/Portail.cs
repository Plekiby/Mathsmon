using UnityEngine;
using UnityEngine.SceneManagement;

public class Portail : MonoBehaviour
{
    public int sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the portal.");
            Debug.Log("Scene to load: '" + sceneToLoad + "'");
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Non-player object entered. Object Tag: " + other.tag);
        }
    }
}
