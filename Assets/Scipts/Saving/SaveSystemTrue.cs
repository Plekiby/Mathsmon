using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystemTrue
{
    private static string saveFilePath = Application.persistentDataPath + "/savefile.json";
    public static void SavePlayer(PlayerControllers player, List<global::Pokemon> pokemonTeam)
    {
        ClearSaveData();
        if (pokemonTeam == null)
        {
            Debug.LogError("pokemonTeam is null");
            return; // Arr�tez la sauvegarde si la liste est null
        } else if (player == null) {
            Debug.LogError("player is null");
            return; // Arr�tez la sauvegarde si la liste est null
        }
        else
        {
            //string path = Application.persistentDataPath + "/savefile.json";
            PlayerData data = new PlayerData(player, pokemonTeam);
            if (data.position != null)
            {
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(saveFilePath, json);
                Debug.Log("Saved data to file: " + File.ReadAllText(saveFilePath));
                Debug.Log($"Loaded position: {data.position[0]}, {data.position[1]}, {data.position[2]}");
            }
            else
            {
                Debug.Log($"erreur");
            }
            
            //Debug.Log($"Saved position: {data.position[2]}");
        }
    }

    public static PlayerData LoadPlayer()
    {
        //string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            ClearSaveData();
            // Ajoutez une v�rification pour vous assurer que la position est non nulle et contient le nombre correct d'�l�ments
            if (data.position != null && data.position.Length == 3)
            {
                Debug.Log($"Loaded position: {data.position[0]}, {data.position[1]}, {data.position[2]}");
            }
            else
            {
                // S'il y a une erreur, imprimez les d�tails pour aider au d�bogage
                if (data.position == null)
                {
                    Debug.LogError("Position array is null.");
                }
                else
                {
                    Debug.LogError($"Position array does not contain 3 elements, it has {data.position.Length} elements.");
                }
            }
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + saveFilePath);
            return null;
        }
    }


    public static void ClearSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save data cleared.");
        }
        else
        {
            Debug.Log("No save data to clear.");
        }
    }

}


