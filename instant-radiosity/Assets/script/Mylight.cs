using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mylight : MonoBehaviour
{
    public int InitVPLCount = 64;
    public float Fov = 60;
    public float radius = 0.5f;
    public RenderTexture rendertexture;
    public Cubemap MyCubeMap;


    public string type = "Point Light";


    List<GameObject> VPL = new List<GameObject>();
    List<Vector2> VPTexturePoint = new List<Vector2>();
    List<Vector3> VPLVector = new List<Vector3>();
    List<Vector3> VPLPoints = new List<Vector3>();
    List<Color> colorList;
    float SpotAngle = 60;
    GameObject parent;
    GameObject PL;
    // Start is called before the first frame update
    void Start()
    {
        SpotAngle = GetComponent<Light>().spotAngle;
        if (type == "Point Light")
        {
            Vector3 org = this.transform.position;
            RaycastHit hit;
            parent = new GameObject();
            for (int i = 0; i < InitVPLCount; i++)
            {
                float Rx = Random48.Get();
                float Ry = Random48.Get();
                float Rz = Random48.Get();
                Vector3 tempDir = new Vector3(Rx * 2.0f - 1.0f, Ry * 2.0f - 1.0f, Rz * 2.0f - 1.0f);
                //tempDir.Normalize();
                //tempDir -= new Vector3(0.5f,0.5f,0.5f);
                VPLVector.Add(tempDir);
                Vector3 B = VPLVector[i];
                Vector3 dir = this.transform.right * B.x + this.transform.up * B.y - this.transform.forward * B.z;
                if (Physics.Raycast(org, dir, out hit, 1000))
                {
                    VPLPoints.Add(hit.point);
                }
                GameObject temp = new GameObject();
                temp.transform.parent = parent.transform;
                temp.transform.position = VPLPoints[i];
                temp.AddComponent<Light>();

                Vector3 CubeUV = convert_xyz_to_cube_uv(B);
                if (CubeUV.x == 0.0f)
                {
                    temp.GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.PositiveX, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                }
                if (CubeUV.x == 1.0f)
                {
                    temp.GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.NegativeX, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                }
                if (CubeUV.x == 2.0f)
                {
                    temp.GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.PositiveY, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                }
                if (CubeUV.x == 3.0f)
                {
                    temp.GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.NegativeY, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                }
                if (CubeUV.x == 4.0f)
                {
                    temp.GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.PositiveZ, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                }
                if (CubeUV.x == 5.0f)
                {
                    temp.GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.NegativeZ, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                }
                VPL.Add(temp);
            }

        }
        else
        {
            int PassCount = 0;
            Vector3 org = this.transform.position;
            RaycastHit hit;
            Texture2D tempTex = new Texture2D(1024, 1024);
            RenderTexture.active = rendertexture;
            tempTex.ReadPixels(new UnityEngine.Rect(0, 0, 1024, 1024), 0, 0);
            parent = new GameObject();
            Debug.DrawLine(transform.position + transform.forward * 10, this.transform.position);
            while (PassCount != InitVPLCount)
            {
                //float Rx = ((float)Random.Range(0, 1024)) / 1024.0f;
                //float Ry = ((float)Random.Range(0, 1024)) / 1024.0f;

                float Rx = Random48.Get();
                float Ry = Random48.Get();
                if (Mathf.Sqrt((Rx - 0.5f) * (Rx - 0.5f) + (Ry - 0.5f) * (Ry - 0.5f)) <= radius)
                {                    
                    Vector3 B = new Vector2(Rx, Ry);
                    B.x -= 0.5f;
                    B.y -= 0.5f;
                    //            float l = Vector3.Dot(B, B);
                    float z;
                    z = -Mathf.Tan((Fov / 2.0f) / 180.0f * 3.1415926f);

                    Vector3 dir = this.transform.right * B.x + this.transform.up * B.y - this.transform.forward * z;
                    
                    if (Physics.Raycast(org, dir, out hit, 1000))
                    {                        
                        Vector3 p;
                        p.x = B.x * 1024.0f;
                        p.y = B.y * 1024.0f;

                        GameObject temp = new GameObject();
                        temp.transform.parent = parent.transform;
                        temp.transform.position = hit.point;
                        temp.AddComponent<Light>();
                        temp.GetComponent<Light>().color = (tempTex.GetPixel((int)p.x, (int)p.y));
                        VPL.Add(temp);
                        PassCount++;
                    }
                    else
                    {
                        continue;
                    }

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //VPL.Clear();       
        //VPLPoints.Clear();
        //VPLVector.Clear();
        if (type == "Point Light")
        {
            Vector3 org = this.transform.position;
            RaycastHit hit;
            for (int i = 0; i < InitVPLCount; i++)
            {
                Vector3 checkdir = (VPLPoints[i] - org).normalized;
                float checkdistance = (VPLPoints[i] - org).magnitude;
                if (Physics.Raycast(org, checkdir, out hit, checkdistance))
                {
                    float Rx = Random48.Get();
                    float Ry = Random48.Get();
                    float Rz = Random48.Get();
                    Vector3 tempDir = new Vector3(Rx * 2.0f - 1.0f, Ry * 2.0f - 1.0f, Rz * 2.0f - 1.0f);
                    tempDir.Normalize();
                    //tempDir -= new Vector3(0.5f, 0.5f, 0.5f);
                    VPLVector[i] = (tempDir);
                    Vector3 B = VPLVector[i];
                    Vector3 dir = this.transform.right * B.x + this.transform.up * B.y - this.transform.forward * B.z;
                    if (Physics.Raycast(org, dir, out hit, 1000))
                    {
                        VPLPoints[i] = (hit.point);
                    }

                    VPL[i].transform.position = VPLPoints[i];

                    Vector3 CubeUV = convert_xyz_to_cube_uv(B);
                    if (CubeUV.x == 0.0f)
                    {
                        VPL[i].GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.PositiveX, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                    }
                    if (CubeUV.x == 1.0f)
                    {
                        VPL[i].GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.NegativeX, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                    }
                    if (CubeUV.x == 2.0f)
                    {
                        VPL[i].GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.PositiveY, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                    }
                    if (CubeUV.x == 3.0f)
                    {
                        VPL[i].GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.NegativeY, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                    }
                    if (CubeUV.x == 4.0f)
                    {
                        VPL[i].GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.PositiveZ, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                    }
                    if (CubeUV.x == 5.0f)
                    {
                        VPL[i].GetComponent<Light>().color = MyCubeMap.GetPixel(CubemapFace.NegativeZ, (int)(CubeUV.y * 1024.0), (int)(CubeUV.z * 1024.0));
                    }
                }

                
            }
        }
        else
        {

            Vector3 org = this.transform.position;
            RaycastHit hit;
            Texture2D tempTex = new Texture2D(1024, 1024);
            RenderTexture.active = rendertexture;
            tempTex.ReadPixels(new UnityEngine.Rect(0, 0, 1024, 1024), 0, 0);
            Vector3 temp = (this.transform.right + this.transform.up + this.transform.forward);
            Debug.DrawLine(transform.position + transform.forward * 10, this.transform.position);
            for (int i = 0; i < InitVPLCount; i++)
            {
                Vector3 checkdir = (VPL[i].transform.position - org).normalized;
                float checkdistance = (VPL[i].transform.position - org).magnitude;
                float checkAngle = Vector3.Angle(checkdir, transform.forward);

                if (Physics.Raycast(org, checkdir, out hit, checkdistance) || checkAngle > (SpotAngle / 2.0f))
                {
                    float Rx = Random48.Get();
                    float Ry = Random48.Get();
                    if (Mathf.Sqrt((Rx - 0.5f) * (Rx - 0.5f) + (Ry - 0.5f) * (Ry - 0.5f)) <= radius)
                    {
                        Vector3 B = new Vector3(Rx, Ry, 0);

                        B.x -= 0.5f;
                        B.y -= 0.5f;
                        //            float l = Vector3.Dot(B, B);
                        float z;
                        z = -Mathf.Tan((Fov / 2.0f) / 180.0f * 3.1415926f);

                        Vector3 dir = this.transform.right * B.x + this.transform.up * B.y - this.transform.forward * z;
                        if (Physics.Raycast(org, dir, out hit, 1000))
                        {
                            Vector3 p;
                            p.x = B.x * 1024.0f;
                            p.y = B.y * 1024.0f;

                            VPL[i].transform.position = hit.point;
                            VPL[i].GetComponent<Light>().color = (tempTex.GetPixel((int)p.x, (int)p.y));
                        }
                    }
                }

                

            }
        }
    }

    //random
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

    Vector3 convert_xyz_to_cube_uv(Vector3 In)
    {
        float x = In.x;
        float y = In.y;
        float z = In.z;

        float absX = Mathf.Abs(x);
        float absY = Mathf.Abs(y);
        float absZ = Mathf.Abs(z);

        bool isXPositive = x > 0 ? true : false;
        bool isYPositive = y > 0 ? true : false;
        bool isZPositive = z > 0 ? true : false;

        float maxAxis = 100, uc = 0, vc = 0;

        float index = 0;
        float u = 0;
        float v = 0;

        // POSITIVE X
        if (isXPositive & (absX >= absY) && (absX >= absZ))
        {
            // u (0 to 1) goes from +z to -z
            // v (0 to 1) goes from -y to +y
            maxAxis = absX;
            uc = -z;
            vc = y;
            index = 0;
        }
        // NEGATIVE X
        if (!isXPositive && absX >= absY && absX >= absZ)
        {
            // u (0 to 1) goes from -z to +z
            // v (0 to 1) goes from -y to +y
            maxAxis = absX;
            uc = z;
            vc = y;
            index = 1;
        }
        // POSITIVE Y
        if (isYPositive && absY >= absX && absY >= absZ)
        {
            // u (0 to 1) goes from -x to +x
            // v (0 to 1) goes from +z to -z
            maxAxis = absY;
            uc = x;
            vc = -z;
            index = 2;
        }
        // NEGATIVE Y
        if (!isYPositive && absY >= absX && absY >= absZ)
        {
            // u (0 to 1) goes from -x to +x
            // v (0 to 1) goes from -z to +z
            maxAxis = absY;
            uc = x;
            vc = z;
            index = 3;
        }
        // POSITIVE Z
        if (isZPositive && absZ >= absX && absZ >= absY)
        {
            // u (0 to 1) goes from -x to +x
            // v (0 to 1) goes from -y to +y
            maxAxis = absZ;
            uc = x;
            vc = y;
            index = 4;
        }
        // NEGATIVE Z
        if (!isZPositive && absZ >= absX && absZ >= absY)
        {
            // u (0 to 1) goes from +x to -x
            // v (0 to 1) goes from -y to +y
            maxAxis = absZ;
            uc = -x;
            vc = y;
            index = 5;
        }

        // Convert range from -1 to 1 to 0 to 1
        u = 0.5f * (uc / maxAxis + 1.0f);
        v = 0.5f * (vc / maxAxis + 1.0f);


        return new Vector3(index, u, v);
    }
}
