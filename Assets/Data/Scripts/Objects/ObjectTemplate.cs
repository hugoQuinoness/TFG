using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectTemplate
{
    public int id;
    public string name;

    public void ToJson()
    {
        string json = JsonUtility.ToJson(this);
        Debug.Log(json);
    }

    public void FromJson(string json)
    {
        ObjectTemplate obj = JsonUtility.FromJson<ObjectTemplate>(json);
        id = obj.id;
        name = obj.name;
    }
}
