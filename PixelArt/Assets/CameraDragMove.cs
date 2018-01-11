using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Threading;
using System.Collections.Generic;

public class CameraDragMove : MonoBehaviour
{
    Texture2D ImagePass;
    Texture2D ImageFuture;

    public InputField InputExport;
    public string FileName = "pixelart";
    private bool BugBtn = false;
    public GameObject DrawLineObj;
    public GameObject DrawFloodFillObj;
    public GameObject DrawObj;
    public GameObject DrawDarkObj;
    public GameObject DrawBrightObj;
    public GameObject DrawEraseObj;

    public GameObject ExportCanvas;

    int x1, y1;
    public int width = 32;
    public int height = 32;

    public int test;
    public bool testBool = false;

    public Text TextDebug;
    public Text TextDebug2;
    public GameObject plane;
    public Material Background;

    public Color32 color;
    public Color32 Tmpcolor;

    private Vector3 ResetCamera;
    private float ResetCameraScale;
    private Quaternion ResetCameraRotation;

    private string PixelMode = "draw";

    private int OldTouchCount = 0;
    private int OldTouchCountTmp = 0;
    Vector2 pixelUVTmp;

    Vector2?[] oldTouchPositions = {
        null,
        null
    };
    Vector2 oldTouchVector;
    float oldTouchDistance;
    Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        ImagePass = new Texture2D(width, height, TextureFormat.ARGB32, false);
        ImageFuture = new Texture2D(width, height, TextureFormat.ARGB32, false);
    }

    void CameraMovement()
    {
        if (PixelMode == "draw")
        {
            if (Input.touchCount == 0)
            {
                oldTouchPositions[0] = null;
                oldTouchPositions[1] = null;
            }
            else if (Input.touchCount == 1)
            {
                if (oldTouchPositions[0] == null || oldTouchPositions[1] != null)
                {
                    oldTouchPositions[0] = Input.GetTouch(0).position;
                    oldTouchPositions[1] = null;
                }
                else
                {
                    if (Input.touchCount == 2) {
                        if (OldTouchCount == 1) {//maybe this dont do nothing
                            DrawTemp();
                        }
                        Vector2 newTouchPosition = Input.GetTouch(0).position;

                        transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] - newTouchPosition) * camera.orthographicSize / camera.pixelHeight * 2f));

                        oldTouchPositions[0] = newTouchPosition;
                    }
                }
            }
            else
            {
                if (oldTouchPositions[1] == null)
                {
                    oldTouchPositions[0] = Input.GetTouch(0).position;
                    oldTouchPositions[1] = Input.GetTouch(1).position;
                    oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
                    oldTouchDistance = oldTouchVector.magnitude;
                }
                else
                {
                    Vector2 screen = new Vector2(camera.pixelWidth, camera.pixelHeight);

                    Vector2[] newTouchPositions = {
                    Input.GetTouch(0).position,
                    Input.GetTouch(1).position
                    
                };
                    if (OldTouchCount == 1)
                    {
                        DrawTemp();
                    }
                    Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
                    float newTouchDistance = newTouchVector.magnitude;

                    transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * camera.orthographicSize / screen.y));
                    transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
                    camera.orthographicSize *= oldTouchDistance / newTouchDistance;
                    transform.position -= transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * camera.orthographicSize / screen.y);

                    oldTouchPositions[0] = newTouchPositions[0];
                    oldTouchPositions[1] = newTouchPositions[1];
                    oldTouchVector = newTouchVector;
                    oldTouchDistance = newTouchDistance;
                }
            }
        }
    }

    void Start()
    {
        //folder
        if (!Directory.Exists("/storage/emulated/0/DCIM/PixelDraw"))
        {
            string dir = "/storage/emulated/0/DCIM/";
            Directory.CreateDirectory(dir + "PixelDraw");
        }
        //camera
        ResetCamera = Camera.main.transform.position;
        ResetCameraScale = Camera.main.orthographicSize;
        ResetCameraRotation = Camera.main.transform.rotation;
        //texture
        float scaleX = (float)(1.0f * (float)width)/256.0f;
        float scaleY = (float)(1.0f * (float)height)/256.0f;
        print(scaleX);
        Background.mainTextureScale = new Vector2((float)scaleX, (float)scaleY);

        Color[] colors = new Color[width*height];
        for (int i = 0;i < colors.Length;i++) {
            colors[i] = new Color(1, 1, 1, 0.0f);
        }
        color = Color.black;
        Texture2D pixelTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        pixelTexture.filterMode = FilterMode.Point;
        pixelTexture.SetPixels(colors);
        pixelTexture.Apply();
        plane.GetComponent<Renderer>().material.mainTexture = pixelTexture;
        SaveImageState(pixelTexture);
    }
    public void ColorButton(string nomeCor)
    {
        if (nomeCor == "black")
        {
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
        if (nomeCor == "magenta")
        {
            color = Color.magenta;
        }
        if (nomeCor == "grey")
        {
            color = Color.grey;
        }
        if (nomeCor == "orange")
        {
            color = new Color32 (255,165,0,255);
        }
        if (nomeCor == "brown")
        {
            color = new Color32(139, 69, 19, 255);
        }
    }
    public void ResetCamPos()
    {
        Camera.main.transform.position = ResetCamera;
        Camera.main.orthographicSize = ResetCameraScale;
        Camera.main.transform.rotation = ResetCameraRotation;
    }
    public void MoveBtn()
    {
        PixelMode = "move";
    }
    public void DrawBtn()
    {
        DrawEraseObj.SetActive(false);
        DrawLineObj.SetActive(false);
        DrawFloodFillObj.SetActive(false);
        DrawObj.SetActive(true);
        DrawDarkObj.SetActive(false);
        DrawBrightObj.SetActive(false);
    }
    public void DrawEraseBtn()
    {
        DrawEraseObj.SetActive(true);
        DrawLineObj.SetActive(false);
        DrawFloodFillObj.SetActive(false);
        DrawObj.SetActive(false);
        DrawDarkObj.SetActive(false);
        DrawBrightObj.SetActive(false);
    }
    public void DrawFloodFillBtn()
    {
        DrawEraseObj.SetActive(false);
        DrawLineObj.SetActive(false);
        DrawFloodFillObj.SetActive(true);
        DrawObj.SetActive(false);
        DrawDarkObj.SetActive(false);
        DrawBrightObj.SetActive(false);
    }
    public void DrawLineBtn()
    {
        DrawEraseObj.SetActive(false);
        DrawLineObj.SetActive(true);
        DrawFloodFillObj.SetActive(false);
        DrawObj.SetActive(false);
        DrawDarkObj.SetActive(false);
        DrawBrightObj.SetActive(false);
    }
    public void DrawDarkBtn()
    {
        DrawEraseObj.SetActive(false);
        DrawLineObj.SetActive(false);
        DrawFloodFillObj.SetActive(false);
        DrawObj.SetActive(false);
        DrawDarkObj.SetActive(true);
        DrawBrightObj.SetActive(false);
    }
    public void DrawBrightBtn()
    {
        DrawEraseObj.SetActive(false);
        DrawLineObj.SetActive(false);
        DrawFloodFillObj.SetActive(false);
        DrawObj.SetActive(false);
        DrawDarkObj.SetActive(false);
        DrawBrightObj.SetActive(true);
    }
    public void Draw()
    {
        print("Draw");
        if (PixelMode == "draw")
        {
            if (Input.touchCount == 1 || Application.isEditor)
            {
                /*
                if (!Input.GetMouseButton(0))
                    return;
                */

                RaycastHit hit;
                if (!Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
                    return;

                Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                    return;

                Texture2D tex = rend.material.mainTexture as Texture2D;

                SaveImageState(tex);

                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;



                pixelUVTmp.x = pixelUV.x;
                pixelUVTmp.y = pixelUV.y;
                Tmpcolor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                Debug.Log("Color: "+Tmpcolor);

                tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, color);
                tex.Apply();
            }
        }
    }
    public void DrawErase()
    {
        print("Draw");
        if (PixelMode == "draw")
        {
            if (Input.touchCount == 1 || Application.isEditor)
            {
                /*
                if (!Input.GetMouseButton(0))
                    return;
                */

                RaycastHit hit;
                if (!Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
                    return;

                Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                    return;

                Texture2D tex = rend.material.mainTexture as Texture2D;
                SaveImageState(tex);
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;



                pixelUVTmp.x = pixelUV.x;
                pixelUVTmp.y = pixelUV.y;
                Tmpcolor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                color = new Color32(0, 0, 0, 0);
                Debug.Log("Color: " + Tmpcolor);

                tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, color);
                tex.Apply();
            }
        }
    }
    public void DrawDark()
    {
        print("DrawDark");
        if (PixelMode == "draw")
        {
            if (Input.touchCount == 1 || Application.isEditor)
            {
                /*
                if (!Input.GetMouseButton(0))
                    return;
                */

                RaycastHit hit;
                if (!Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
                    return;

                Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                    return;

                Texture2D tex = rend.material.mainTexture as Texture2D;
                SaveImageState(tex);
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;



                pixelUVTmp.x = pixelUV.x;
                pixelUVTmp.y = pixelUV.y;
                Tmpcolor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                Debug.Log("Color: " + Tmpcolor);

                Color colorDark = Tmpcolor;
                colorDark.r -= 0.05f;
                colorDark.g -= 0.05f;
                colorDark.b -= 0.05f;

                tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, colorDark);
                tex.Apply();
            }
        }
    }
    public void DrawBright()
    {
        print("DrawDark");
        if (PixelMode == "draw")
        {
            if (Input.touchCount == 1 || Application.isEditor)
            {
                /*
                if (!Input.GetMouseButton(0))
                    return;
                */

                RaycastHit hit;
                if (!Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
                    return;

                Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                    return;

                Texture2D tex = rend.material.mainTexture as Texture2D;
                SaveImageState(tex);
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;



                pixelUVTmp.x = pixelUV.x;
                pixelUVTmp.y = pixelUV.y;
                Tmpcolor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                Debug.Log("Color: " + Tmpcolor);

                Color colorDark = Tmpcolor;
                colorDark.r += 0.05f;
                colorDark.g += 0.05f;
                colorDark.b += 0.05f;

                tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, colorDark);
                tex.Apply();
            }
        }
    }
    public void DrawLine() {
        RaycastHit hit;
        if (!Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
            return;

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;

        Texture2D tex = rend.material.mainTexture as Texture2D;
        SaveImageState(tex);
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;



        pixelUVTmp.x = pixelUV.x;
        pixelUVTmp.y = pixelUV.y;
        x1 = (int)pixelUV.x;
        y1 = (int)pixelUV.y;
    }
    public void DrawLineDrop()
    {
        int x2, y2;
        RaycastHit hit;
        if (!Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
            return;

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;

        Texture2D tex = rend.material.mainTexture as Texture2D;
        SaveImageState(tex);
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;



        pixelUVTmp.x = pixelUV.x;
        pixelUVTmp.y = pixelUV.y;
        x2 = (int)pixelUV.x;
        y2 = (int)pixelUV.y;
        Tmpcolor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
        line(x1, y1, x2, y2, color, tex);
        tex.Apply();

    }
    public void DrawFill()
    {
        print("Draw");
        if (PixelMode == "draw")
        {
            if (Input.touchCount == 1 || Application.isEditor)
            {
                /*
                if (!Input.GetMouseButton(0))
                    return;
                */

                RaycastHit hit;
                if (!Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
                    return;

                Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                    return;

                Texture2D tex = rend.material.mainTexture as Texture2D;
                SaveImageState(tex);
                tex.wrapMode = TextureWrapMode.Clamp;
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;



                pixelUVTmp.x = pixelUV.x;
                pixelUVTmp.y = pixelUV.y;
                Tmpcolor = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
                Debug.Log("Color: " + Tmpcolor);
                //fill first
                //tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, color);
                //fill others next matrix blocks
                if (!Tmpcolor.Equals(color)) {
                    FloodFill(tex,color,Tmpcolor,0.0f, (int)pixelUV.x, (int)pixelUV.y);
                    //FillLoop((int)pixelUV.x, (int)pixelUV.y, color, Tmpcolor, tex);
                    //
                    tex.Apply();
                }
            }
        }
    }
    /*
    void FillLoop(int x, int y, Color32 color, Color32 Tmpcolor, Texture2D tex)
    {
        Color32 color32 = tex.GetPixel(x, y);
        if (color32.Equals(Tmpcolor))
        {
            tex.SetPixel(x, y, color);
            FillLoop(x + 1, y, color, Tmpcolor, tex);
            FillLoop(x - 1, y, color, Tmpcolor, tex);
            FillLoop(x, y + 1, color, Tmpcolor, tex);
            FillLoop(x, y - 1, color, Tmpcolor, tex);
        }
    }
    */
    public struct Point
    {

        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public static void FloodFill(Texture2D tex,Color targetColor, Color sourceColor, float tollerance, int x, int y)
    {
        var q = new Queue<Point>(tex.width * tex.height);
        q.Enqueue(new Point(x, y));
        int iterations = 0;

        var width = tex.width;
        var height = tex.height;
        while (q.Count > 0)
        {
            var point = q.Dequeue();
            var x1 = point.x;
            var y1 = point.y;
            if (q.Count > width * height)
            {
                throw new System.Exception("The algorithm is probably looping. Queue size: " + q.Count);
            }

            if (tex.GetPixel(x1, y1) == targetColor)
            {
                continue;
            }

            tex.SetPixel(x1, y1, targetColor);


            var newPoint = new Point(x1 + 1, y1);
            if (CheckValidity(tex, tex.width, tex.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            newPoint = new Point(x1 - 1, y1);
            if (CheckValidity(tex, tex.width, tex.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            newPoint = new Point(x1, y1 + 1);
            if (CheckValidity(tex, tex.width, tex.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            newPoint = new Point(x1, y1 - 1);
            if (CheckValidity(tex, tex.width, tex.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            iterations++;
        }
    }

    static bool CheckValidity(Texture2D texture, int width, int height, Point p, Color sourceColor, float tollerance)
    {
        if (p.x < 0 || p.x >= width)
        {
            return false;
        }
        if (p.y < 0 || p.y >= height)
        {
            return false;
        }

        var color = texture.GetPixel(p.x, p.y);

        var distance = Mathf.Abs(color.r - sourceColor.r) + Mathf.Abs(color.g - sourceColor.g) + Mathf.Abs(color.b - sourceColor.b);
        return distance <= tollerance;
    }
    public void DrawTemp()
    {
        print("Draw_Temp");
        TextDebug.text = "Draw_Temp";
        Texture2D tex = plane.GetComponent<Renderer>().material.mainTexture as Texture2D;   
        tex.SetPixel((int)pixelUVTmp.x, (int)pixelUVTmp.y, Tmpcolor);
        tex.Apply();
    }
    void TouchCount()
    {
        //print("old: "+OldTouchCount);
        //print("oldTemp: " + OldTouchCountTmp);
        if (OldTouchCountTmp != Input.touchCount) {
            OldTouchCount = OldTouchCountTmp;
            OldTouchCountTmp = Input.touchCount;
        }
    }
    public void SaveImageState(Texture2D tex) {
        Color[] cores;
        cores = tex.GetPixels();
        ImagePass.SetPixels(cores);
        //ImagePass = tex as Texture2D;
        ImagePass.Apply();
        Debug.Log("image saved..");
    }
    public void BackBTN()
    {
        Texture2D tex = plane.GetComponent<Renderer>().material.mainTexture as Texture2D;
        Color[] coresFuture;
        coresFuture = tex.GetPixels();
        ImageFuture.SetPixels(coresFuture);
        ImageFuture.Apply();
        Color[] cores;
        cores = ImagePass.GetPixels();
        tex.SetPixels(cores);
        //tex = ImagePass as Texture2D;
        tex.Apply();
        Debug.Log("image restored..");
        BugBtn = true;
    }
    public void FrontBTN()
    {
        if (BugBtn == true) {
            Texture2D tex = plane.GetComponent<Renderer>().material.mainTexture as Texture2D;
            Color[] cores;
            cores = ImageFuture.GetPixels();
            tex.SetPixels(cores);
            //tex = ImagePass as Texture2D;
            tex.Apply();
            Debug.Log("image restored from future..");
        }
    }
    public void ExportButton()
    {
        ExportCanvas.SetActive(true);//show a text to user.
    }
    public void ExportCloseButton()
    {
        ExportCanvas.SetActive(false);//show a text to user.
    }
    public void ExportPNG()
    {
        Texture2D tex = plane.GetComponent<Renderer>().material.mainTexture as Texture2D;

        //scale2X
        Texture2D pixelTextureScale = scaled(tex,512,512,FilterMode.Point);
        //
        FileName = InputExport.text;
        // Encode texture into PNG
        byte[] bytes = pixelTextureScale.EncodeToPNG();

        //ExportCanvas.SetActive(true);//show a text to user.

        // For testing purposes, also write to a file in the project folder
        //File.WriteAllBytes(Application.dataPath + "/pixelart.png", bytes); //pc
        File.WriteAllBytes("/storage/emulated/0/DCIM/PixelDraw/"+FileName+".png", bytes); //android
    }
    //scale Image...
    public static Texture2D scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
    {
        Rect texR = new Rect(0, 0, width, height);
        _gpu_scale(src, width, height, mode);

        //Get rendered data back to a new texture
        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
        result.Resize(width, height);
        result.ReadPixels(texR, 0, 0, true);
        return result;
    }

    
    public static void scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
    {
        Rect texR = new Rect(0, 0, width, height);
        _gpu_scale(tex, width, height, mode);

        // Update new texture
        tex.Resize(width, height);
        tex.ReadPixels(texR, 0, 0, true);
        tex.Apply(true);        //Remove this if you hate us applying textures for you :)
    }

    // Internal unility that renders the source texture into the RTT - the scaling method itself.
    static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
    {
        //We need the source texture in VRAM because we render with it
        src.filterMode = fmode;
        src.Apply(true);

        //Using RTT for best quality and performance. Thanks, Unity 5
        RenderTexture rtt = new RenderTexture(width, height, 32);

        //Set the RTT in order to render to it
        Graphics.SetRenderTarget(rtt);

        //Setup 2D matrix in range 0..1, so nobody needs to care about sized
        GL.LoadPixelMatrix(0, 1, 1, 0);

        //Then clear & draw the texture to fill the entire RTT.
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
    }
    float Distance(int x1,int y1,int x2,int y2) {
        float delta = 0;
        int dx = x2 - x1;
        int dy = y2 - y1;
        delta = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        print("distance: "+delta);
        return delta;
    }
    public void line(int x, int y, int x2, int y2,Color color,Texture2D tex)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            tex.SetPixel(x, y, color);
            //tex.Apply();
            //Debug.Log("x "+x+"y "+y);
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    //scale Image...
    void Update()
    {
        TextDebug.text = PixelMode;
        TextDebug2.text = OldTouchCount.ToString();

        //Draw();//try to execute faster to solve bug_01.
        CameraMovement();
        TouchCount();
        if (Input.GetKeyDown("space")) {
            testBool = true;
        }
        if (testBool == true) {
            DrawTemp();
            testBool = false;
        }
    }
}
