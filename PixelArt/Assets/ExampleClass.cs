// Write black pixels onto the GameObject that is located
// by the script. The script is attached to the camera.
// Determine where the collider hits and modify the texture at that point.
//
// Note that the MeshCollider on the GameObject must have Convex turned off. This allows
// concave GameObjects to be included in collision in this example.
//
// Also to allow the texture to be updated by mouse button clicks it must have the Read/Write
// Enabled option set to true in its Advanced import settings.

using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public Camera cam;
    public GameObject plane;
    Color color;

    void Start()
    {
        color = Color.black;
        Texture2D pixelTexture = new Texture2D(32,32, TextureFormat.ARGB32, false);
        pixelTexture.filterMode = FilterMode.Point;
        plane.GetComponent<Renderer>().material.mainTexture = pixelTexture;
        cam = GetComponent<Camera>();
    }

    public void ColorButton(string nomeCor) {
        if (nomeCor == "black") {
            color = Color.black;
        }
        if (nomeCor == "white")
        {
            color = Color.white;
        }
        if (nomeCor == "red")
        {
            color = Color.red;
        }
        if (nomeCor == "green")
        {
            color = Color.green;
        }
        if (nomeCor == "blue")
        {
            color = Color.blue;
        }
        if (nomeCor == "yellow")
        {
            color = Color.yellow;
        }
    }

    void Update() //try to call faster than frames(to solve bug_01)
    {
        if (Input.touchCount == 1)
        {
            if (!Input.GetMouseButton(0))
                return;

            RaycastHit hit;
            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                return;

            Renderer rend = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                return;

            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;

            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, color);
            tex.Apply();
        }
    }
}
