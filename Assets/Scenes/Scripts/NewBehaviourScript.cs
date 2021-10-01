using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform prefab;
    // Start is called before the first frame update
    void Start()
    {
        Renderer meshR = gameObject.GetComponent<Renderer>();

        meshR.enabled = true;

        if (prefab != null)
        {
            Instantiate(prefab, new Vector3(-13.434f, 1.19f, -1f), Quaternion.identity);
        }
       
    }
}
