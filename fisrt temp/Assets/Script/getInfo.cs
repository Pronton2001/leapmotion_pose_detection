using UnityEngine;
using Leap.Unity;
public class getInfo : MonoBehaviour
{
    private Transform[] getChild;
    private RigidHand rg_left;
    private RigidHand rg_right;

    private float[] product = new float[5];
    private float[] angle_ = new float[5];
    private bool[] extend = new bool[5];
    private float[] length_ = new float[5];

    private bool initialized = false;
    // 2 plane are the same but have a different purpose
    Plane planeTest; // calculate intersection 
    GameObject plane_; // display in the scene

    public RigidHand Rg_left { get => rg_left; set => rg_left = value; }
    public RigidHand Rg_right { get => rg_right; set => rg_right = value; }
    public Transform[] GetChild { get => getChild; set => getChild = value; }
    public float[] Length_ { get => length_; set => length_ = value; }



    void Start()
    {
        GetChild = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform t in transform)
        {
            GetChild[i++] = t;
        }
        Rg_left = this.GetChild[3].GetComponent<RigidHand>();
        Rg_right = this.GetChild[4].GetComponent<RigidHand>();
        Debug.Log(Rg_left != null ? "rg_left is activate" : "rg_left is null");
        Debug.Log(Rg_right != null ? "rg_right is activate" : "rg_right is null");
        for (int j = 0; j < HandModel.NUM_FINGERS; j++)
        {
            extend[j] = false;
        }
        plane_ = GameObject.CreatePrimitive(PrimitiveType.Plane);
    }

    // There are some basic gesture for classifying. Forward, Stop, Left, Right
    private enum gestures
    {
        Forward,
        Stop,
        Left,
        Right
    }
    private float getAngle(Vector3 tar, Vector3 src)
    {
        float cos_theta = Vector3.Dot(tar, src) / (Mathf.Sqrt(Vector3.Dot(tar, tar)) * Vector3.Dot(src, src));
        cos_theta = Mathf.Min(1, cos_theta);
        cos_theta = Mathf.Max(-1, cos_theta);
        return Mathf.Acos(cos_theta) * 180 / Mathf.PI;
    }
    private float getSquareDistance(Vector3 src, Vector3 des)
    {
        return Mathf.Pow(src.x - des.x, 2) + Mathf.Pow(src.y - des.y, 2) + Mathf.Pow(src.z - des.z, 2);
    }
    public void ClassifyHeuristic1()
    {
        int counter = 0;
        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            angle_[i] = getAngle(Rg_left.fingers[i].GetBoneDirection(1), Rg_left.fingers[i].GetBoneDirection(2));
            extend[i] = (angle_[i] < 10) ? true : false;
        }
        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            if (extend[i]) counter++;
            Debug.Log(Rg_left.fingers[i].fingerType + " : " + angle_[i]);
        }
        Debug.Log("Number:" + counter);
    }
    public void ClassifyHeuristic2()
    {
        int counter = 0; // store number counter for classifying
        int index = -1; // store fingerType 
        /*		0: Thumb 
				1: Index
				2: Middle
				3: Ring
				4: Pinky
		*/
        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            angle_[i] = getAngle(Rg_left.fingers[i].GetBoneDirection(1), Rg_left.fingers[i].GetBoneDirection(2));
            extend[i] = (angle_[i] < 20) ? true : false;
            if (extend[i]) index = i;
        }
        //This if is a trick, because index will get the final fingerType after loop; therefore, if index = 0(thumb), it is 1.
        //Reason: It is hard to deal with thumb because it is different from the others.  
        if (index == 0)
        {
            counter = 1;
            Debug.Log("Number:" + counter);
            return;
        }
        if (index < 0)
        {
            plane_.SetActive(false);
            Debug.Log("Number" + counter);
            return;
        }
        // case: index > 0 
        counter++;
        planeTest = new Plane(Vector3.Normalize(plane_.transform.up), plane_.transform.localPosition);

        plane_.SetActive(true);
        plane_.transform.localScale = new Vector3(.4f, -1, .4f);
        // y = -1 because plane is 2D, cannot be seen in 3D in normal => set y to -1
        plane_.transform.localPosition = Rg_left.fingers[index].GetJointPosition(2); // = ...  
        plane_.transform.up = Rg_left.fingers[index].GetBoneDirection(2);// = planeTest.normal; 

        for (int i = 1; i < HandModel.NUM_FINGERS && i != index; ++i)
        {
            float enter;
            Ray ray = new Ray(Rg_left.fingers[i].GetJointPosition(0), Rg_left.fingers[i].GetBoneDirection(2));
            bool intersect = planeTest.Raycast(ray, out enter);
            if (intersect)
            {
                counter++;
                Debug.Log(Rg_left.fingers[i].fingerType + " -> plane " + Rg_left.fingers[index].fingerType + " is " + enter);
            }
        }
        if (extend[0]) counter++; // special case : thumb, I cannot cover it so I lead it be special.
        Debug.Log("Number:" + counter);
    }

    public void ClassifyHeuristic3()
    {
        float[] length_ = new float[5];
		int extNum = 0;
        float[] distance_ = new float[5];
        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            distance_[i] = getSquareDistance(Rg_left.fingers[i]
                                           .GetTipPosition(), Rg_left.palm.position);
            Debug.Log("Distance/Length " + Rg_left.fingers[i].fingerType + " = " + distance_[i] / Length_[i]);
			if (distance_[i]/Length_[i] >1.0f) extNum++;
        }
		Debug.Log("Number:" + extNum);
    }
    void Update()
    {
        if (initialized is false)
        {
            if (Rg_left != null)
            {
                for (int k = 0; k < HandModel.NUM_FINGERS; ++k)
                {
                    Length_[k] = 0;
                    for (int j = 0; j < FingerModel.NUM_BONES; ++j)
                        Length_[k] += Rg_left.fingers[k].GetBoneLength(j);
                    Debug.Log("Length of" + Rg_left.fingers[k].fingerType + ":" + Length_[k]);
                }
                initialized = true;
            }
        }
        //FingerInfo();   
        ClassifyHeuristic3();
    }
    void FingerInfo()
    {
        for (int i = 0; i < HandModel.NUM_FINGERS; i++)
        {
            Debug.Log("LEFT:" + Rg_left.fingers[i].fingerType + ":" + Rg_left.fingers[i].GetBoneDirection(FingerModel.NUM_BONES - 1));
        }
        for (int i = 0; i < HandModel.NUM_FINGERS; i++)
        {
            Debug.Log("RIGHT:" + Rg_right.fingers[i].fingerType + ":" + Rg_right.fingers[i].GetBoneDirection(FingerModel.NUM_BONES - 1));
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
