using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setCameraCubemap : MonoBehaviour
{
    public Camera MyCam;
    public Cubemap MyCubeMap;
    // Start is called before the first frame update
    void Start()
    {       
        MyCam.RenderToCubemap(MyCubeMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
