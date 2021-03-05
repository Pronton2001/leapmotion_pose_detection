using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))] // mean that just use for User control
[AddComponentMenu("Control Script/FPS Input")]  // add a menu in Inpsector/ Add Component
public class FPSInput : MonoBehaviour
{
    float speed = 6.0f;
    public float gravity = -9.8f;
    private CharacterController _charController; 
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float delX = Input.GetAxis("Horizontal") * speed;
        float delZ = Input.GetAxis("Vertical") * speed;

        // transform.Translate(delX * Time.deltaTime, 0, delZ * Time.deltaTime);
        // cannot use the above cmd because it will pass through walls
        // To avoid this issue, we use CharacterController that can detect collision.
        
        Vector3 movement = new Vector3(delX, 0, delZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = gravity;
        movement *= Time.deltaTime;

        movement = transform.TransformDirection(movement);
        // Transform the movement vector from local to global coordinates.
        _charController.Move(movement);


    }
}
