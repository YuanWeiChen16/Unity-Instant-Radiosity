using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mylight : MonoBehaviour
{
    public int InitVPLCount = 64;
    public RenderTexture rendertexture;
    List<Vector2> VPTexturePoint = new List<Vector2>();
    List<Vector3> VPLPoints = new List<Vector3>();

    List<Color> colorList;


    GameObject parent;
    GameObject PL;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < InitVPLCount; i++)
        {
            float Rx = halton2(i);
            float Ry = halton2(i * 3);
            //float Rz = halton2(i*7);

            VPTexturePoint.Add(new Vector2(Rx, Ry));
        }

        Vector3 org = this.transform.position;
        RaycastHit hit;

        for (int i = 0; i < InitVPLCount; i++)
        {

            Vector3 B = VPTexturePoint[i];
            float l = Vector3.Dot(B, B);
            float z;
            if (l >= 1.0f)
                z = 0.0f;
            else
                z = Mathf.Sqrt(1.0f - l);
           
            Vector3 dir = this.transform.right * VPTexturePoint[i].x + this.transform.up * VPTexturePoint[i].y - this.transform.forward * z;

            Physics.Raycast(org, dir, out hit);
            VPLPoints.Add(hit.point);
        }

        parent = new GameObject();
        parent.name = "VPL Point";
        parent.transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < VPLPoints.Count; i++)
        {
            GameObject temp = new GameObject();
            temp.transform.parent = parent.transform;
            temp.transform.position = VPLPoints[i];
            temp.AddComponent<Light>();
            temp.GetComponent<Light>().color = colorList[i];
        }



    }

    // Update is called once per frame
    void Update()
    {

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
