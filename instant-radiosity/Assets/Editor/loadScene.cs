using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class loadScene : EditorWindow
{
    public TextAsset LoadFile;
    List<GameObject> objs = new List<GameObject>();
    Dictionary<string, int> scriptPlace = new Dictionary<string, int>();
    Dictionary<string, int> objPlace = new Dictionary<string, int>();

    [MenuItem("Tool/Load Scene")]
    public static void MenuOpen()
    {
        EditorWindow.GetWindow(typeof(loadScene), false, "Load Scene", true);
    }

    // Update is called once per frame
    void OnGUI()
    {
       
        LoadFile = (TextAsset)EditorGUILayout.ObjectField("LoadFile: ", LoadFile, typeof(TextAsset), true);
        if (GUILayout.Button("Load Text"))
        {
            RenderTexture renderTex = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/Textures/lightLook.renderTexture", typeof(RenderTexture));
            Cubemap cubeTex = (Cubemap)AssetDatabase.LoadAssetAtPath("Assets/Textures/CubeT.cubemap", typeof(Cubemap));
            if (load())
            {
                // copy light
                objs.Add(new GameObject());
                objPlace["PassLight"] = objs.Count - 1;
                DestroyImmediate(objs[objPlace["PassLight"]]);
                objs[objPlace["PassLight"]] = Instantiate(objs[objPlace["light"]]);
                objs[objs.Count - 1].name = "PassLight";
                objs[objPlace["PassLight"]].transform.position += new Vector3(0, 0, 100);
                objs[objPlace["PassLight"]].GetComponent<Mylight>().enabled = false;
                objs[objPlace["PassLight"]].GetComponent<LightMove>().Li += new Vector3(0, 0, 100);

                // copy camera
                objs.Add(new GameObject());
                objPlace["PassCamera"] = objs.Count - 1;
                DestroyImmediate(objs[objPlace["PassCamera"]]);
                objs[objPlace["PassCamera"]] = Instantiate(objs[objPlace["camera"]]);
                objs[objs.Count - 1].name = "PassCamera";
                objs[objPlace["PassCamera"]].GetComponent<Camera>().depth = 24;
                objs[objPlace["PassCamera"]].GetComponent<Camera>().targetTexture = renderTex;
                objs[objPlace["PassCamera"]].GetComponent<Camera>().allowHDR = false;
                objs[objPlace["PassCamera"]].GetComponent<Camera>().allowMSAA = false;
                objs[objPlace["PassCamera"]].AddComponent<setCameraCubemap>();
                objs[objPlace["PassCamera"]].GetComponent<setCameraCubemap>().MyCam = objs[objPlace["PassCamera"]].GetComponent<Camera>();
                objs[objPlace["PassCamera"]].GetComponent<setCameraCubemap>().MyCubeMap = cubeTex;
                objs[objPlace["PassCamera"]].transform.parent = objs[objPlace["PassLight"]].transform;
                objs[objPlace["PassCamera"]].transform.localPosition = new Vector3(0, 0, 0);
                objs[objPlace["PassCamera"]].transform.localRotation = new Quaternion(0, 0, 0, 0);
                
                // copy obj
                objs.Add(new GameObject());
                objPlace["PassPrefab"] = objs.Count - 1;
                DestroyImmediate(objs[objPlace["PassPrefab"]]);
                objs[objPlace["PassPrefab"]] = Instantiate(objs[objPlace["prefab"]]);
                objs[objs.Count - 1].name = "PassPrefab";
                objs[objPlace["PassPrefab"]].transform.position += new Vector3(0, 0, 100);
            }

        }
    }

    bool load()
    {
        string light_type = "Point Light";
        if (LoadFile == null)
            Debug.LogError("Cannot open the file!");
        else
        {
            string[] lineText = LoadFile.text.Split('\n');
            int size = 0;
            objs.Clear();
            scriptPlace.Clear();
            objPlace.Clear();
            int condition = 0; // 0: init, 1: cam, 2: light, 3: prefab, 4: parameters
            for (int i = 0; i < lineText.Length; i++)
            {
                if (lineText[i].EndsWith("\r"))
                {
                    lineText[i] = lineText[i].Remove(lineText[i].Length - 1);
                }

                string[] data = lineText[i].Split(' ');
                if (data.Length == 1)
                {
                    if (data[0] == "camera")
                    {
                        condition = 1;
                        objs.Add(new GameObject());
                        size = objs.Count;
                        objs[size - 1].AddComponent<Camera>();
                        objs[size - 1].name = "Main Camera";
                        objs[size - 1].tag = "MainCamera";
                        objPlace["camera"] = size - 1;
                    }
                    else if (data[0] == "light")
                    {
                        condition = 2;
                        objs.Add(new GameObject());
                        size = objs.Count;
                        objs[size - 1].AddComponent<Light>();
                        objs[size - 1].GetComponent<Light>().shadows = LightShadows.Hard;
                        objs[size - 1].GetComponent<Light>().shadowBias = 0;
                        objs[size - 1].GetComponent<Light>().shadowNormalBias = 0;
                        objs[size - 1].GetComponent<Light>().shadowNearPlane = 0.2f;
                        objPlace["light"] = size - 1;
                        objs[size - 1].name = "light";
                    }
                    else if (data[0] == "prefab")
                    {
                        condition = 3;
                        objs.Add(new GameObject());
                        size = objs.Count;
                        objPlace["prefab"] = size - 1;
                    }
                    else if (data[0] == "parameters")
                    {
                        condition = 4;
                    }
                    else if (data[0].Length == 0) { }
                    else return false;
                }
                else if (data.Length > 0)
                {
                    if (data[0] == "p")
                        objs[size - 1].transform.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                    else if (data[0] == "r")
                        objs[size - 1].transform.Rotate(new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3])));
                    else if (data[0] == "s")
                        objs[size - 1].transform.localScale = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                    else if (data[0].Length == 0) { }

                    else if (condition == 1)
                    {
                        if (data[0] == "depth")
                            objs[size - 1].GetComponent<Camera>().depth = -1;
                        else if (data[0].Length == 0) { }
                        else return false;
                    }
                    else if (condition == 2)
                    {
                        if (data[0] == "t" && int.Parse(data[1]) == 0)
                        {
                            objs[size - 1].GetComponent<Light>().type = LightType.Spot;
                            light_type = "Spot Light";
                        }
                        else if (data[0] == "t" && int.Parse(data[1]) == 1)
                        {
                            objs[size - 1].GetComponent<Light>().type = LightType.Point;
                            light_type = "Point Light";
                        }
                        else if (data[0] == "c" && data[1] == "Mylight")
                        {
                            objs[size - 1].AddComponent<Mylight>();
                            scriptPlace["Mylight"] = size - 1;

                            objs[size - 1].GetComponent<Mylight>().type = light_type;
                        }
                        else if (data[0] == "c" && data[1] == "LightMove")
                        {
                            objs[size - 1].AddComponent<LightMove>();
                            scriptPlace["LightMove"] = size - 1;
                        }
                        else if (data[0] == "range")
                            objs[size - 1].GetComponent<Light>().range = float.Parse(data[1]);
                        else if (data[0] == "color")
                            objs[size - 1].GetComponent<Light>().color = new Color(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                        else if (data[0] == "intensity")
                            objs[size - 1].GetComponent<Light>().intensity = float.Parse(data[1]);
                        else if (data[0] == "angle")
                            objs[size - 1].GetComponent<Light>().spotAngle = float.Parse(data[1]);
                        else if (data[0].Length == 0) { }
                        else return false;
                    }
                    else if (condition == 3)
                    {
                        if (data[0] == "path")
                        {
                            DestroyImmediate(objs[size - 1]);
                            objs[size - 1] = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(data[1], typeof(GameObject)));
                            objs[size - 1].name = objs[size - 1].name.Remove(objs[size - 1].name.Length - 7);
                        }
                        else if (data[0].Length == 0) { }
                        else return false;
                    }
                    else if (condition == 4)
                    {
                        if (data[0] == "VPL" && scriptPlace.ContainsKey("Mylight"))
                            objs[scriptPlace["Mylight"]].GetComponent<Mylight>().InitVPLCount = int.Parse(data[1]);
                        else if (data[0] == "fov" && scriptPlace.ContainsKey("Mylight"))
                            objs[scriptPlace["Mylight"]].GetComponent<Mylight>().Fov = float.Parse(data[1]);
                        else if (data[0] == "renderTex" && scriptPlace.ContainsKey("Mylight"))
                            objs[scriptPlace["Mylight"]].GetComponent<Mylight>().rendertexture = (RenderTexture)AssetDatabase.LoadAssetAtPath(data[1], typeof(RenderTexture));
                        else if (data[0] == "cubemap" && scriptPlace.ContainsKey("Mylight"))
                            objs[scriptPlace["Mylight"]].GetComponent<Mylight>().MyCubeMap = (Cubemap)AssetDatabase.LoadAssetAtPath(data[1], typeof(Cubemap));
                        else if (data[0] == "Li" && scriptPlace.ContainsKey("LightMove"))
                            objs[scriptPlace["LightMove"]].GetComponent<LightMove>().Li = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                        else if (data[0] == "MoveSpeed" && scriptPlace.ContainsKey("LightMove"))
                            objs[scriptPlace["LightMove"]].GetComponent<LightMove>().MoveSpeed = float.Parse(data[1]);
                        else if (data[0].Length == 0) { }
                        else return false;
                    }
                }
            }
        }
        return true;
    }
}
