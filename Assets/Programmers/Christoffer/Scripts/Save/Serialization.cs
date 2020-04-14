using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Runtime.Serialization;

public static class Serialization 
{
    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";
        Debug.Log(path);

        FileStream file = File.Create(path);
        formatter.Serialize(file, saveData);

        file.Close();
        return true;
    }

    public static object Load(string p)
    {

        string path = Application.persistentDataPath + "/saves/" + p + ".save";
        if (!File.Exists(path))
            return null;

        Debug.Log(path);
        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at (0)", path);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vec3SerializationSurrogate vector3Surrogate = new Vec3SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionsSurrogate = new QuaternionSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionsSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
