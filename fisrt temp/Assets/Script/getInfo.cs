using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;



public class getInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] getChild;
    public RigidHand rg_left; 
    public RigidHand rg_right; 

	private	float[] product = new float[5]; 
	private	float[] angle_ = new float[5];
	private bool[] extend = new bool[5];


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
	    return Mathf.Acos(cos_theta) * 180 / Mathf.PI;
    }
    public void ClassifyHeuristic(){
	int counter = 0;
	for (int i = 0; i< HandModel.NUM_FINGERS; ++i){
		angle_[i] = getAngle( rg_left.fingers[i].GetBoneDirection(1),
					rg_left.fingers[i].GetBoneDirection(2));
		extend[i] = (angle_[i]<10) ? true : false;
	}
	for (int i = 0; i < HandModel.NUM_FINGERS; ++i){
		if(extend[i]) counter++;
		Debug.Log(rg_left.fingers[i].fingerType + " : " + angle_[i]);
	}
	Debug.Log("Number:"+ counter);

        //Debug.Log("get" + rg_left.GetPalmDirection()+
	//	   "get" + rg_left.GetPalmNormal());
    }
    void Update()
    {
	//FingerInfo();   
	ClassifyHeuristic();
    }
    void FingerInfo(){
	for (int i=0; i<HandModel.NUM_FINGERS; i++){
            Debug.Log("LEFT:" + rg_left.fingers[i].fingerType + ":"+
                rg_left.fingers[i].GetBoneDirection(FingerModel.NUM_BONES- 1));
        }
        for (int i=0; i<HandModel.NUM_FINGERS; i++){
            Debug.Log("RIGHT:" + rg_right.fingers[i].fingerType + ":"+
                rg_right.fingers[i].GetBoneDirection(FingerModel.NUM_BONES- 1));
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
