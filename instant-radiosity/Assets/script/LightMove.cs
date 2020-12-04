using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMove : MonoBehaviour
{
    public Vector3 Li;
    public float MoveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Transform>().RotateAround(Li,new Vector3(0,1,0), MoveSpeed*Time.deltaTime);
    }
}
