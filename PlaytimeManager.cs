using System;
using System.IO;
using UnityEngine;

public class PlaytimeManager
{
    public PlaytimeData Data { get; private set; } = new PlaytimeData();

    public void LoadPlaytime(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                Data = JsonUtility.FromJson<PlaytimeData>(json);
            }
            catch
            {
                Data = new PlaytimeData();
            }
        }
        else
        {
            Data = new PlaytimeData();
        }
    }

    public void SavePlaytime(string path)
    {
        try
        {
            string json = JsonUtility.ToJson(Data);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to save playtime data: {e.Message}");
        }
    }
}
