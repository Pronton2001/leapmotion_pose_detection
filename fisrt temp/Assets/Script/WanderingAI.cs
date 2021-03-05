
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WanderingAI : MonoBehaviour
{
    float speed = 6.0f;
    float obstacleRange = 3.0f;
    public float gravity = -9.8f;
    private bool _alive;

    void Start()
    {
        _alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        //no need collision -> can use translate
        if(_alive){
            transform.Translate(0, 0, speed*Time.deltaTime);
            Ray ray = new Ray(this.transform.position, this.transform.forward);
            RaycastHit hit;
        
           if(Physics.SphereCast(ray, .75f, out hit)){
               if (hit.distance < obstacleRange){
                   float angle = Random.Range(90,180);
                   this.transform.Rotate(0,angle,0);
               }
           }
        }
    }
    public void setAlive(bool alive){
        _alive = alive;
    }
}

