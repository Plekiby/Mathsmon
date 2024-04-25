using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;  // This namespace is required for LoadSceneMode


public class SaveSystem : MonoBehaviour
{
    public PlayerControllers player; // Assign this field in the Unity Editor
    public string filePath = "savefile.json"; // Optionally expose this field if necessary

    public void SaveGameData()
    {
        if (player == null)
        {
            Debug.LogError("PlayerControllers not set.");
            return;
        }

        GameData data = new GameData
        {
            PlayerPosition = new Vector3Serializable(player.transform.position),
            SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            //PokemonData = player.GetComponent<PokemonParty>().Pokemons.Select(p => p.Serialize()).ToList()
        };

        string json = JsonUtility.ToJson(data);
        string fullPath = Path.Combine(Application.persistentDataPath, filePath);
        File.WriteAllText(fullPath, json);
        Debug.Log("Game Saved to " + fullPath);
    }

    public void LoadGameData()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filePath);
        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            // Stop the player from moving immediately after loading
            player.enabled = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene(data.SceneName, LoadSceneMode.Single);
            StartCoroutine(WaitForSceneLoad(data, player));
        }
        else
        {
            Debug.LogError("Save file not found in " + fullPath);
        }
    }

    IEnumerator WaitForSceneLoad(GameData data, PlayerControllers playerController)
    {
        // Wait for the new scene to finish loading
        yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == data.SceneName);

        playerController.transform.position = data.PlayerPosition.ToVector();
        playerController.enabled = true;
    }
}
