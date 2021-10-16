// Grab.cs reside in Hand leapmotion to track with some objects and grab them.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grab : MonoBehaviour
{
    public const int TOTAL_FINGERS = 10;
    private bool[] extended;
    private getInfo info;
    public enum Hand {
        Left, 
        Right
    }
    public Hand side = Hand.Left;
    void Start()
    {
        info = this.GetComponentInParent<getInfo>();
    }
    // public bool isGrab()
    // {
    //     if(side == Hand.Left){
    //     if (extended[0] && (extended[1] || extended[2] || extended[3] || extended[4]))
    //         return true;
    //     return false;
    //     }
    //     // side == Hand.Right
    //     if (extended[5] && (extended[6] || extended[7] || extended[8] || extended[9]))
    //         return true;
    //     return false;
 
    // }
    void Update()
    {
        // extended = info.GetExtend();
    }
    public void _Activate(){
        StartCoroutine(Grabbing());
    }
    
    public void _Deactive(){
        StartCoroutine(NonGrab());
    }
    
    private IEnumerator Grabbing(){
        Debug.Log("I'm grabbed!!!");
        yield break;
    }
    
    private IEnumerator NonGrab(){
        Debug.Log("I've not been grabbed anymore!!!");
        yield break;
    }
}
