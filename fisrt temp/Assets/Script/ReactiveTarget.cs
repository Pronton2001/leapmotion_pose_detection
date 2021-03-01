using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    public void ReactToHit(){
        StartCoroutine(Die());
    }
    
    private IEnumerator Die(){
        this.transform.Rotate(-75,0,0);

        yield return new WaitForSeconds(1.5f);

        // this = this script
        // this.gameObject = gameObject that this script is attached to. [this is Optional]
        Destroy(this.gameObject);
    }

}
