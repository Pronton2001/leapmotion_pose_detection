using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other){
       var collider = other.GetComponent<PlayerCharacter>();
       Debug.Log(collider);
       if(collider != null){
           collider.Hurt(1);
           Debug.Log("Hand is grabbing something.");
            //communicate with grabed object  => collider.begrab()      
       }
       else Debug.Log("TriggerEnter run");
    }
    private void OnTriggerExit(Collider other){
        var collider = other.GetComponent<Grab>();
        if(collider != null){
            collider._Deactive();
            Debug.Log("Hand does not grab anything");
        }
        else Debug.Log("TriggerExit run");
    }
    
}
