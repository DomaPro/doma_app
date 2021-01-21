using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveObject(AppSystem appSystem)
    {
        string nameFile = DomaManager.Instance.fileNameTextBox.text;
        nameFile += ".dma";

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + nameFile;
        FileStream stream = new FileStream(path, FileMode.Create);

        SerializeSystem data = new SerializeSystem(appSystem);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SerializeSystem LoadObject(string nameFile)
    {
        string path = Application.persistentDataPath + "/" + nameFile + ".dma";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SerializeSystem data = formatter.Deserialize(stream) as SerializeSystem;

            stream.Close();

            return data;
        }
        else
        {
            //Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static string[] GetNameAllFiles()
    {
        List<string> result = new List<string>();

        string path = Application.persistentDataPath;

        try
        {
            string[] dirs = Directory.GetFiles(path, "*.dma");
            foreach (string dir in dirs)
            {
                var d = dir.Replace(path, "");
                d = d.Replace("\\", "");
                d = d.Replace(".dma", "");

                result.Add(d);
            }
        }
        catch (Exception e)
        {
            Debug.Log("The process failed");
        }

        return result.ToArray();
    }
}
