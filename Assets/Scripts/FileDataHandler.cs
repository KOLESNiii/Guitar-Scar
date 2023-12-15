using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath;
    private string dataFilePath;

    public FileDataHandler(string dataDirPath, string dataFilePath)
    {
        this.dataDirPath = dataDirPath;
        this.dataFilePath = dataFilePath;
    }

    public void SetDataFilePath(string dataFilePath)
    {
        this.dataFilePath = dataFilePath;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);
        GameData data = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string DataSerialized = "";
                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        DataSerialized = reader.ReadToEnd();
                    }
                }
                data = JsonUtility.FromJson<GameData>(DataSerialized);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data from " + fullPath + ": " + e.Message);
            }
        }
        return data;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFilePath);
        try
        {
            Directory.CreateDirectory(dataDirPath);
            string DataSerialized = JsonUtility.ToJson(data);
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(DataSerialized);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data to " + fullPath + ": " + e.Message);
        }

    }
}
