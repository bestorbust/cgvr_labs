using System.Collections.Generic;
using UnityEngine;

/// <summary>Lab 10: one cube, two viewports; orthographic left, perspective right.
/// Port of the original 3d_viewing_pipeline_lab10.py experiment.</summary>
[DisallowMultipleComponent]
public sealed class Lab10ViewingPipelineDemo : MonoBehaviour
{
    public Shader faceShader;
    private Transform cubeRoot;
    private Mesh cubeMesh;
    private Material faceMaterial, edgeMaterial;
    private Camera orthoCamera, perspCamera;
    private bool playing = true, showInfo = true;
    private float angle, speed = 30f, fov = 45f, orthoSize = 3f;
    private GUIStyle header, label;

    private static readonly Vector3[] V = {
        new Vector3(-1,-1,-1),new Vector3(1,-1,-1),new Vector3(1,1,-1),new Vector3(-1,1,-1),
        new Vector3(-1,-1,1),new Vector3(1,-1,1),new Vector3(1,1,1),new Vector3(-1,1,1)
    };
    private static readonly int[][] Faces = {
        new[]{0,1,2,3}, new[]{4,5,6,7}, new[]{0,1,5,4},
        new[]{2,3,7,6}, new[]{1,2,6,5}, new[]{0,3,7,4}
    };
    private static readonly Color[] Colors = {
        new Color(1f,.2f,.2f),new Color(.2f,1f,.3f),new Color(.2f,.4f,1f),
        new Color(1f,.8f,.2f),new Color(.8f,.2f,1f),new Color(.2f,1f,1f)
    };
    private static readonly int[,] Edges = {
        {0,1},{1,2},{2,3},{3,0},{4,5},{5,6},{6,7},{7,4},{0,4},{1,5},{2,6},{3,7}
    };

    private void Awake()
    {
        Shader shader = faceShader != null ? faceShader : Shader.Find("CGVR/Lab10/FaceColors");
        if (shader == null || !shader.isSupported)
        {
            Debug.LogError("Missing or unsupported Lab10FaceColors shader. Use the Built-in Render Pipeline.");
            return;
        }
        BuildCube(shader);
        orthoCamera = BuildCamera("LEFT: Orthographic Camera",new Rect(0f,0f,.5f,1f),true);
        perspCamera = BuildCamera("RIGHT: Perspective Camera",new Rect(.5f,0f,.5f,1f),false);
        ApplyProjectionSettings();
    }

    private void BuildCube(Shader shader)
    {
        cubeRoot = new GameObject("Shared rotating cube - model transform").transform;
        GameObject facesObject = new GameObject("6 coloured faces - 12 triangles");
        facesObject.transform.SetParent(cubeRoot,false);
        var vertices = new List<Vector3>();
        var colors = new List<Color>();
        var indices = new List<int>();
        for (int i=0;i<6;i++)
        {
            int start=vertices.Count;
            for (int j=0;j<4;j++) { vertices.Add(V[Faces[i][j]]); colors.Add(Colors[i]); }
            indices.Add(start); indices.Add(start+1); indices.Add(start+2);
            indices.Add(start); indices.Add(start+2); indices.Add(start+3);
        }
        cubeMesh = new Mesh {name="Lab10 six-colour cube"};
        cubeMesh.SetVertices(vertices);
        cubeMesh.SetColors(colors);
        cubeMesh.SetTriangles(indices,0);
        cubeMesh.RecalculateBounds();
        facesObject.AddComponent<MeshFilter>().sharedMesh=cubeMesh;
        faceMaterial=new Material(shader) {name="Lab10 RGB face shader"};
        facesObject.AddComponent<MeshRenderer>().sharedMaterial=faceMaterial;

        Shader lines=Shader.Find("Sprites/Default");
        if (lines==null) lines=Shader.Find("Unlit/Color");
        if (lines==null) {Debug.LogWarning("Edge shader missing; coloured cube is still usable.");return;}
        edgeMaterial=new Material(lines) {color=new Color(.025f,.035f,.05f)};
        for(int i=0;i<Edges.GetLength(0);i++)
        {
            GameObject edge=new GameObject("Outline edge "+i);
            edge.transform.SetParent(cubeRoot,false);
            LineRenderer line=edge.AddComponent<LineRenderer>();
            line.sharedMaterial=edgeMaterial;
            line.useWorldSpace=false;
            line.positionCount=2;
            line.SetPosition(0,V[Edges[i,0]]);
            line.SetPosition(1,V[Edges[i,1]]);
            line.startWidth=.026f;
            line.endWidth=.026f;
            line.startColor=Color.white;
            line.endColor=Color.white;
            line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows=false;
        }
    }

    private Camera BuildCamera(string name,Rect rect,bool isOrtho)
    {
        GameObject go=new GameObject(name);
        Camera cam=go.AddComponent<Camera>();
        cam.clearFlags=CameraClearFlags.SolidColor;
        cam.backgroundColor=new Color(.045f,.065f,.10f);
        cam.nearClipPlane=.1f;
        cam.farClipPlane=100f;
        cam.rect=rect;
        cam.orthographic=isOrtho;
        cam.depth=isOrtho?0f:1f;
        go.transform.position=new Vector3(4f,3f,6f);
        go.transform.LookAt(Vector3.zero,Vector3.up);
        return cam;
    }

    private void ApplyProjectionSettings()
    {
        if(orthoCamera!=null) orthoCamera.orthographicSize=orthoSize;
        if(perspCamera!=null) perspCamera.fieldOfView=fov;
    }

    private void ResetDemo()
    {
        angle=0f; speed=30f; fov=45f; orthoSize=3f; playing=true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) playing=!playing;
        if(Input.GetKeyDown(KeyCode.R)) ResetDemo();
        if(Input.GetKeyDown(KeyCode.H)) showInfo=!showInfo;
        if(Input.GetKeyDown(KeyCode.UpArrow)) speed=Mathf.Clamp(speed+5f,0f,120f);
        if(Input.GetKeyDown(KeyCode.DownArrow)) speed=Mathf.Clamp(speed-5f,0f,120f);
        if(playing) angle=Mathf.Repeat(angle+speed*Time.deltaTime,360f);
        if(cubeRoot!=null) cubeRoot.rotation=Quaternion.AngleAxis(angle,new Vector3(1f,1f,0f).normalized);
        ApplyProjectionSettings();
    }

    private void OnGUI()
    {
        float scale=Mathf.Max(.55f,Mathf.Min(Screen.width/1280f,Screen.height/720f));
        float w=Screen.width/scale,h=Screen.height/scale;
        GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,Vector3.one*scale);
        if(header==null)
        {
            header=new GUIStyle(GUI.skin.label) {fontSize=20,fontStyle=FontStyle.Bold};
            header.normal.textColor=Color.white;
            label=new GUIStyle(GUI.skin.label) {fontSize=14,wordWrap=true};
            label.normal.textColor=new Color(.82f,.89f,.97f);
        }
        GUI.color=new Color(.04f,.08f,.14f,.95f);
        GUI.DrawTexture(new Rect(12,12,w-24,123),Texture2D.whiteTexture);
        GUI.color=Color.white;
        GUI.Label(new Rect(28,19,w-56,30),"LAB 10   /   3D VIEWING PIPELINE",header);
        GUI.Label(new Rect(28,52,w/2f-34,30),"LEFT: ORTHOGRAPHIC   |   Parallel rays",label);
        GUI.Label(new Rect(w/2f+12,52,w/2f-42,30),"RIGHT: PERSPECTIVE   |   Foreshortening",label);
        if(GUI.Button(new Rect(28,91,120,30),playing?"Pause (Space)":"Play (Space)")) playing=!playing;
        if(GUI.Button(new Rect(158,91,100,30),"Reset (R)")) ResetDemo();
        GUI.Label(new Rect(273,94,280,27),"Angle "+angle.ToString("F0")+"°  |  Speed "+speed.ToString("F0")+"°/s",label);
        if(GUI.Button(new Rect(w-148,91,124,30),showInfo?"Hide info (H)":"Show info (H)")) showInfo=!showInfo;
        GUI.color=new Color(.25f,.44f,.65f,.75f);
        GUI.DrawTexture(new Rect(w/2f-1f,139,2f,Mathf.Max(0f,h-151f)),Texture2D.whiteTexture);
        GUI.color=Color.white;
        if(showInfo)
        {
            GUI.color=new Color(.04f,.08f,.14f,.95f);
            GUI.DrawTexture(new Rect(12,h-121,w-24,109),Texture2D.whiteTexture);
            GUI.color=Color.white;
            GUI.Label(new Rect(28,h-111,w-56,27),"Model → World → View → Projection → NDC → Viewport  |  Both cameras render the SAME cube and rotation.",label);
            GUI.Label(new Rect(28,h-83,154,24),"Ortho half-size: "+orthoSize.ToString("F1"),label);
            orthoSize=GUI.HorizontalSlider(new Rect(185,h-75,Mathf.Max(55,w/2f-211),18),orthoSize,2.2f,5f);
            GUI.Label(new Rect(w/2f+12,h-83,160,24),"Perspective FOV: "+fov.ToString("F0")+"°",label);
            fov=GUI.HorizontalSlider(new Rect(w/2f+174,h-75,Mathf.Max(55,w/2f-211),18),fov,30f,70f);
            GUI.Label(new Rect(28,h-49,w-56,31),"Space: pause/play     R: reset     ↑/↓: speed     H: toggle info     Viewports: left (0,0,.5,1), right (.5,0,.5,1)",label);
        }
        GUI.matrix=Matrix4x4.identity;
        GUI.color=Color.white;
    }

    private void OnDestroy()
    {
        if(faceMaterial!=null) Destroy(faceMaterial);
        if(edgeMaterial!=null) Destroy(edgeMaterial);
        if(cubeMesh!=null) Destroy(cubeMesh);
    }
}
