using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayShooter: MonoBehaviour
{
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;   //Hide the mouse cursor at
        Cursor.visible = false;                     //the center of the screen.
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0);
            Ray ray = _camera.ScreenPointToRay(point);   
            // Ray contain Origin position and direction
            // In this case origin position is Screen, direction = screen -> point = ScreenToRay(point)

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                // Detect the gameObject was hit
		        GameObject hitObject = hit.transform.gameObject;
                // Check if hitObject has a ReactiveTarget component (script attached to that hitObject)
		        ReactiveTarget target = hitObject.GetComponent<ReactiveTarget>();
		        if(target != null){
                    Debug.Log("Target hit. Hit point" + hit.point);
                    target.ReactToHit();    
                }
                else {
                StartCoroutine(SphereIndicator(hit.point));
                }
            }
        }        
    }
    // IEnumerator :a kind of coroutine, no asynchronous but similar to
    private IEnumerator SphereIndicator(Vector3 pos){
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;

        // Pause at yeild        
        yield return new WaitForSeconds(1); 
        // Destructor
        Destroy(sphere);
    }

    void OnGUI(){
        //size of Label in the centered screen
        int size = 12;
        float posX = _camera.pixelWidth/2 - size/4;
        float posY = _camera.pixelHeight/2 - size/2;
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }
}
