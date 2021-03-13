using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5.0f;
    public float damage = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3.forward = Vector3(0, 0, 1)
        
        this.transform.position += this.transform.forward*speed*Time.deltaTime;
        //[SAME] this.transform.Translate(Vector3.forward*speed*Time.deltaTime, Space.Self);
    }
    void OnTriggerEnter(Collider other) {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (player != null) {
            player.Hurt(damage);
            Debug.Log("Player hit");
        }
        Destroy(this.gameObject);
    }
}
