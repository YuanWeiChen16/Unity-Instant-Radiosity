using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mylight : MonoBehaviour
{
    public int InitVPLCount = 64;
    public RenderTexture rendertexture;
    public Cubemap MyCubeMap;


    List<GameObject> VPL = new List<GameObject>();
    List<Vector2> VPTexturePoint = new List<Vector2>();
    List<Vector3> VPLPoints = new List<Vector3>();
    List<Color> colorList;

    GameObject parent;
    GameObject PL;
    // Start is called before the first frame update
    void Start()
    {

        int PassCount = 0;
        while (PassCount != InitVPLCount)
        {
            //float Rx = ((float)Random.Range(0, 1024)) / 1024.0f;
            //float Ry = ((float)Random.Range(0, 1024)) / 1024.0f;
            float Rx = Random48.Get();
            float Ry = Random48.Get();
            if (Mathf.Sqrt((Rx - 0.5f) * (Rx - 0.5f) + (Ry - 0.5f) * (Ry - 0.5f)) <= 0.5f)
            {
                VPTexturePoint.Add(new Vector2(Rx, Ry));
                PassCount++;
            }
        }

        Vector3 org = this.transform.position;
        RaycastHit hit;

        for (int i = 0; i < InitVPLCount; i++)
        {

            Vector3 B = VPTexturePoint[i];
            B.x -= 0.5f;
            B.y -= 0.5f;
            //            float l = Vector3.Dot(B, B);
            float z;
            z = -Mathf.Tan(30.0f / 180.0f * 3.1415926f);

            Vector3 dir = this.transform.right * B.x + this.transform.up * B.y - this.transform.forward * z;
            if (Physics.Raycast(org, dir, out hit, 1000))
            {
                VPLPoints.Add(hit.point);
            }
        }

        Texture2D tempTex = new Texture2D(1024, 1024);
        
        RenderTexture.active = rendertexture;

        tempTex.ReadPixels(new UnityEngine.Rect(0, 0, 1024, 1024), 0, 0);


        parent = new GameObject();
        for (int i = 0; i < InitVPLCount; i++)
        {
            Vector3 p;
            p.x = VPTexturePoint[i].x * 1024.0f;
            p.y = VPTexturePoint[i].y * 1024.0f;

            GameObject temp = new GameObject();
            temp.transform.parent = parent.transform;
            temp.transform.position = VPLPoints[i];
            temp.AddComponent<Light>();
            temp.GetComponent<Light>().color = (tempTex.GetPixel((int)p.x, (int)p.y));
            VPL.Add(temp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //VPL.Clear();
        VPTexturePoint.Clear();
        VPLPoints.Clear();

        int PassCount = 0;
        while (PassCount != InitVPLCount)
        {
            //float Rx = ((float)Random.Range(0, 1024)) / 1024.0f;
            //float Ry = ((float)Random.Range(0, 1024)) / 1024.0f;
            float Rx = Random48.Get();
            float Ry = Random48.Get();
            if (Mathf.Sqrt((Rx - 0.5f) * (Rx - 0.5f) + (Ry - 0.5f) * (Ry - 0.5f)) <= 0.5f)
            {
                VPTexturePoint.Add(new Vector2(Rx, Ry));
                PassCount++;
            }
        }

        Vector3 org = this.transform.position;
        RaycastHit hit;

        for (int i = 0; i < InitVPLCount; i++)
        {

            Vector3 B = VPTexturePoint[i];
            B.x -= 0.5f;
            B.y -= 0.5f;
            //            float l = Vector3.Dot(B, B);
            float z;
            z = -Mathf.Tan(30.0f / 180.0f * 3.1415926f);

            Vector3 dir = this.transform.right * B.x + this.transform.up * B.y - this.transform.forward * z;
            if (Physics.Raycast(org, dir, out hit, 1000))
            {
                VPLPoints.Add(hit.point);
            }
        }

        Texture2D tempTex = new Texture2D(1024, 1024);
        RenderTexture.active = rendertexture;
        RenderBuffer Db = rendertexture.depthBuffer;
        tempTex.ReadPixels(new UnityEngine.Rect(0, 0, 1024, 1024), 0, 0);

        System.IntPtr A = Db.GetNativeRenderBufferPtr();
        
        
        //Debug.Log(A.() + "1");

        //parent = new GameObject();
        for (int i = 0; i < InitVPLCount; i++)
        {
            Vector3 p;
            p.x = VPTexturePoint[i].x * 1024.0f;
            p.y = VPTexturePoint[i].y * 1024.0f;

            //GameObject temp = new GameObject();
            //VPL.transform.parent = parent.transform;
            VPL[i].transform.position = VPLPoints[i];

            //temp.AddComponent<Light>();
            VPL[i].GetComponent<Light>().color = (tempTex.GetPixel((int)p.x, (int)p.y));
            //VPL.Add(temp);
        }
    }


    float halton2(int k)
    {
        int ret = 0;
        int n = 1;

        while (k > 0)
        {
            ret <<= 1;
            if (k % 2 == 1)
                ret |= 1;
            k >>= 1;
            n <<= 1;
        }

        return ret / (float)n;
    }
}
