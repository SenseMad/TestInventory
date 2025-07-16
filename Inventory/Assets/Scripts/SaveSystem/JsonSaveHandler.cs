using System.IO;
using UnityEngine;

public static class JsonSaveHandler
{
  private static string SavePath => Path.Combine(Application.persistentDataPath, "Save.json");

  public static void Save(SaveData parData)
  {
    var json = JsonUtility.ToJson(parData, true);
    File.WriteAllText(SavePath, json);
  }

  public static SaveData Load()
  {
    if (!File.Exists(SavePath))
      return null;

    var json = File.ReadAllText(SavePath);
    return JsonUtility.FromJson<SaveData>(json);
  }

  public static bool HasSava() => File.Exists(SavePath);
}