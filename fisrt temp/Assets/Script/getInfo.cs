using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;



public class getInfo : MonoBehaviour
{
    public Transform[] getChild;
    public RigidHand rg_left; 
    public RigidHand rg_right; 

	private	float[] product = new float[5]; 
	private	float[] angle_ = new float[5];
	private bool[] extend = new bool[5];
	
	// 2 plane are the same but have a different purpose
	Plane planeTest; // calculate intersection 
	GameObject plane_; // display in the scene

    void Start()
    {
        getChild = new Transform[transform.childCount];
        int i=0;
        foreach(Transform t in transform){
            getChild[i++] = t;
        }
        rg_left = this.getChild[3].GetComponent<RigidHand>();
        rg_right= this.getChild[4].GetComponent<RigidHand>();
        Debug.Log(rg_left != null ?"rg_left is activate": "rg_left is null");
        Debug.Log(rg_right!= null ?"rg_right is activate": "rg_right is null");
		for (int j = 0; j< HandModel.NUM_FINGERS; j++){
			extend[j] = false;
		}
		plane_ = GameObject.CreatePrimitive(PrimitiveType.Plane);
    }

    // There are some basic gesture for classifying. Forward, Stop, Left, Right
    private enum gestures{
        Forward,
        Stop,
        Left,
        Right
    }
    private float getAngle(Vector3 tar, Vector3 src){
	    float cos_theta = Vector3.Dot(tar, src) / (Mathf.Sqrt(Vector3.Dot(tar,tar)) * Vector3.Dot(src,src));
		cos_theta = Mathf.Min(1,cos_theta);
		cos_theta = Mathf.Max(-1, cos_theta);
	    return Mathf.Acos(cos_theta) * 180 / Mathf.PI;
    }
    public void ClassifyHeuristic1(){
		int counter = 0;
		for (int i = 0; i< HandModel.NUM_FINGERS; ++i){
			angle_[i] = getAngle( rg_left.fingers[i].GetBoneDirection(1), rg_left.fingers[i].GetBoneDirection(2));
			extend[i] = (angle_[i]<10) ? true : false;
		}
		for (int i = 0; i < HandModel.NUM_FINGERS; ++i){
			if(extend[i]) counter++;
			Debug.Log(rg_left.fingers[i].fingerType + " : " + angle_[i]);
		}
		Debug.Log("Number:"+ counter);
    }

	public void ClassifyHeuristic2(){
		int counter = 0; // store number counter for classifying
		int index = -1; // store fingerType 
		/*		0: Thumb 
				1: Index
				2: Middle
				3: Ring
				4: Pinky
		*/
		for (int i = 0; i< HandModel.NUM_FINGERS; ++i){
			angle_[i] = getAngle( rg_left.fingers[i].GetBoneDirection(1), rg_left.fingers[i].GetBoneDirection(2));
			extend[i] = (angle_[i] < 20) ? true : false;
			if(extend[i]) index = i;
		}
		//This if is a trick, because index will get the final fingerType after loop; therefore, if index = 0(thumb), it is 1.
		//Reason: It is hard to deal with thumb because it is different from the others.  
		if (index ==0) {	
			counter = 1;
			Debug.Log("Number:"+ counter);		
			return;
		}
		if(index < 0) {
			plane_.SetActive(false);
			Debug.Log("Number" + counter);
			return;
		}
		// case: index > 0 
		counter++;
		planeTest = new Plane(Vector3.Normalize(plane_.transform.up), plane_.transform.localPosition);	
	
		plane_.SetActive(true);
		plane_.transform.localScale = new Vector3(.4f, -1,.4f); 
		// y = -1 because plane is 2D, cannot be seen in 3D in normal => set y to -1
		plane_.transform.localPosition = rg_left.fingers[index].GetJointPosition(2); // = ...  
		plane_.transform.up = rg_left.fingers[index].GetBoneDirection(2);// = planeTest.normal; 
		
		for (int i = 1; i < HandModel.NUM_FINGERS && i!= index; ++i){
			float enter;
			Ray ray = new Ray(rg_left.fingers[i].GetJointPosition(0), rg_left.fingers[i].GetBoneDirection(2));
			bool intersect = planeTest.Raycast(ray, out enter); 
			if(intersect){
				counter++;
				Debug.Log(rg_left.fingers[i].fingerType + " -> plane " + rg_left.fingers[index].fingerType + " is " + enter);
			}
		}
		if(extend[0]) counter++; // special case : thumb, I cannot cover it so I lead it be special.
		Debug.Log("Number:"+ counter);
	}

    void Update()
    {
	//FingerInfo();   
	ClassifyHeuristic2();
    }
    void FingerInfo(){
		for (int i=0; i<HandModel.NUM_FINGERS; i++){
            Debug.Log("LEFT:" + rg_left.fingers[i].fingerType + ":"+ rg_left.fingers[i].GetBoneDirection(FingerModel.NUM_BONES- 1));
        }
        for (int i=0; i<HandModel.NUM_FINGERS; i++){
            Debug.Log("RIGHT:" + rg_right.fingers[i].fingerType + ":"+ rg_right.fingers[i].GetBoneDirection(FingerModel.NUM_BONES- 1));
        }
    }
}

// GetFingerJointStretchMecanim(int joint_type) {
//  public Quaternion GetBoneRotation(int bone_type) {
// public Vector3 GetBoneDirection(int bone_type) {
// public Ray GetRay()
// public Vector3 GetJointPosition(int joint)
// public Vector3 GetTipPosition()
// GetBoneDirection(NUM_BONES - 1) ***************
        // Debug.Log(  "GetArmCenter:"+rg_left.GetArmCenter()+
        //             "GetArmRoatation:"+rg_left.GetArmRotation()+
        //             "GetArmDirec:"+rg_left.GetArmDirection()+
        //             // "get"+ rg_left.GetArmWidth()+
        //             "get" + rg_left.GetPalmDirection()+
        //             // "get" + rg_left.GetPalmPosition()+
        //             "get" + rg_left.GetPalmNormal()+
        //             "get" + rg_left.GetLeapHand()+
        //             "get" + rg_left.fingers[0].fingerType
        // );
