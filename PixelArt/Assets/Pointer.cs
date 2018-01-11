using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    public Camera cam;

    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	public void Click () {
        print("ok");
        RaycastHit hit;
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            print("ok");
            print(hit.textureCoord);
        }
    }
}
