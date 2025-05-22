using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveObject : MonoBehaviour
{
    public int id;

    public string objectName;



    public void GiveObjectToPlayer()
    {

        ObjectTemplate objectTemplate = new ObjectTemplate();

        objectTemplate.id = id;

        objectTemplate.name = objectName;
        
        Player.Instance.uniqueItems.Add(objectTemplate);
    }
}
