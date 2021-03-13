
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WanderingAI : MonoBehaviour
{
    float speed = 6.0f;
    float obstacleRange = 3.0f;
    public float gravity = -9.8f;
    private bool _alive;
    private bool Fire;
    [SerializeField] private GameObject Fireball;
    private GameObject _fireball;
    void Start()
    {
        _alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        //no need collision -> can use translate instead of Move 
        if(_alive){
            transform.Translate(0, 0, speed*Time.deltaTime);
            Ray ray = new Ray(this.transform.position, this.transform.forward);
            RaycastHit hit;
        
           if(Physics.SphereCast(ray, .75f, out hit)){
               GameObject hitObject = hit.transform.gameObject;

               PlayerCharacter _player = hitObject.GetComponent<PlayerCharacter>();
               if (_player != null){
                   Debug.Log("Player ahead!!");
                    // Shoot player by fireball by create instance
                    if(_fireball == null){ // 1 fireball each time
                    _fireball = Instantiate(Fireball) as GameObject;
                    _fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
                    _fireball.transform.rotation = transform.rotation;
                    }
               }
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

