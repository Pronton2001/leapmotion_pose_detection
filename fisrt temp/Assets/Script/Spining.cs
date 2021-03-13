using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spining : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 3.0f;
    public bool local = true;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(local)
            transform.Rotate(0,speed,0);
        else
            transform.Rotate(0,speed,0,Space.World);
    }
}
