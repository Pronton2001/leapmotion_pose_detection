using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    public void ReactToHit(){
        StartCoroutine(Die());
        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null){
            behavior.setAlive(false);
        }
    }
 
    private IEnumerator Die(){
        this.transform.Rotate(-75,0,0);

        yield return new WaitForSeconds(1.5f);

        // this = this script
        // this.gameObject = gameObject that this script is attached to. [this is Optional]
        Debug.Log("Before Dead enemy gameObject has ID"+this.gameObject.GetInstanceID());
        Destroy(this.gameObject);
        Debug.Log("After Dead enemy gameObject has ID"+this.gameObject.GetInstanceID());

    }

}
