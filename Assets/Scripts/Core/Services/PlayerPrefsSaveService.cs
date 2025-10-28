using UnityEngine;

public class PlayerPrefsSaveService : ISaveService
{
    private const string KEY = "HiddenSeed_Save_v1";

    public void Save(GameStateDto state)
    {
        var json = JsonUtility.ToJson(state);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public GameStateDto Load()
    {
        if (!PlayerPrefs.HasKey(KEY)) return null;
        var json = PlayerPrefs.GetString(KEY);
        return JsonUtility.FromJson<GameStateDto>(json);
    }
}