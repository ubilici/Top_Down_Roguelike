using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleGraphicRandomizer : MonoBehaviour {

    public List<GameObject> graphicsList;
    GameObject objectToInstantiate;
    GameObject selectedObject;

	void Start () 
    {
        PickRandomObject();
	}

    void PickRandomObject()
    {
        objectToInstantiate = graphicsList[Random.Range(0, graphicsList.Count)];
        selectedObject = Instantiate(objectToInstantiate, transform.position, RandomRotation()) as GameObject;
        selectedObject.transform.SetParent(transform);
    }

    Quaternion RandomRotation()
    {
        Quaternion randomRotation = new Quaternion();
        randomRotation.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        return randomRotation;
    }


}
