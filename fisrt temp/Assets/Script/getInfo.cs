using UnityEngine;
using Leap.Unity;
using System.IO;
using System.Timers;
using System;
public class getInfo : MonoBehaviour
{
    public int MAXDATA = 1000;
    public int FileNumber = 0;
    public string DataPath;
    // 2 plane are the same but have a different purpose
    Plane plane_intersect; // Plane used to calculate intersection 
    GameObject plane_avatar = null; // Avatar of plane_intersect displayed in the scene

    public RigidHand Rg_left { get => rg_left; set => rg_left = value; }
    public RigidHand Rg_right { get => rg_right; set => rg_right = value; }
    public Transform[] GetChild { get => getChild; set => getChild = value; }
    public float[] bone_length { get => length_; set => length_ = value; }

    private Transform[] getChild;
    private RigidHand rg_left;
    private RigidHand rg_right;
    private int n_extend_fingers;
    private string msg_buffer;
    private int[] data_size = new int[10];
    private float[] angle_ = new float[5];
    private bool[] is_extended = new bool[10];
    private float[] length_ = new float[5];
    private bool initialized = false;

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
        Debug.Log(Rg_left != null 
                    ? "rg_left is activate" 
                    : "rg_left is null");
        Debug.Log(Rg_right != null 
                    ? "rg_right is activate" 
                    : "rg_right is null");
        for (int j = 0; j < HandModel.NUM_FINGERS; j++)
        {
            is_extended[j] = false;
            is_extended[j+5] = false; // right index = left index + 5
        }
        plane_avatar = GameObject.CreatePrimitive(PrimitiveType.Plane);

        // Initialize datasize of each type to zero.
        for (i = 0; i<10; i++){
            data_size[i] = 0;
        }
    }
    void Update()
    {
        if (initialized is false)
        {
            if (Rg_left.fingers[0] != null)
            {
                for (int k = 0; k < HandModel.NUM_FINGERS; ++k)
                {
                    bone_length[k] = 0;
                    for (int j = 0; j < FingerModel.NUM_BONES; ++j)
                        // Length of LEFT hand and RIGHT hand are almost 
                        // the same, thus I just need to find one of them.
                        bone_length[k] += Rg_left.fingers[k].GetBoneLength(j);
                }
                initialized = true;
            }
        }
        n_extend_fingers = 0;
        heuristic2();
        WriteDataToTxt();
    }

    private float getAngle(Vector3 tar, Vector3 src)
    {
        float cos_theta = Vector3.Dot(tar, src) 
                            / (Mathf.Sqrt(Vector3.Dot(tar, tar)) 
                            * Vector3.Dot(src, src));
        cos_theta = Mathf.Min(1, cos_theta);
        cos_theta = Mathf.Max(-1, cos_theta);
        return Mathf.Acos(cos_theta) * 180 / Mathf.PI;
    }

    private float getSquareDistance(Vector3 src, Vector3 des)
    {
        return Mathf.Pow(src.x - des.x, 2) + Mathf.Pow(src.y - des.y, 2) 
                    + Mathf.Pow(src.z - des.z, 2);
    }

    /// <summary>
    /// This function calculate the angle between 2 bone directions of 
    /// fingers and set the angle threshold of extended fingers
    /// </summary>
    public int heuristic1()
    {
        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            angle_[i] = getAngle(Rg_left.fingers[i].GetBoneDirection(1), 
                                    Rg_left.fingers[i].GetBoneDirection(2));
            is_extended[i] = (angle_[i] < 10) ? true : false;
        }
        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            if (is_extended[i]) n_extend_fingers++;
            Debug.Log(Rg_left.fingers[i].fingerType + " : " + angle_[i]);
        }
        Debug.Log("Number:" + n_extend_fingers);
        return n_extend_fingers;
    }

    /// <summary>
    /// This function try to draw a plane that is perpendicular to bone 
    /// direction of Pinky or Ring finger. Then, we count the number of 
    /// intersections between the that plane and the other fingers'bone 
    /// directions.
    /// </summary>
    public int heuristic2()
    {
        int index = -1; // store fingerType 
        /*		
            The Rg_left.fingers indices: (the right one is the same)
                0: Thumb 
				1: Index
				2: Middle
				3: Ring
				4: Pinky
		*/
        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            angle_[i] = getAngle(Rg_left.fingers[i].GetBoneDirection(1), 
                                    Rg_left.fingers[i].GetBoneDirection(2));
            is_extended[i] = (angle_[i] < 20) ? true : false;
            if (is_extended[i]) index = i;
        }

        if (index == 0)
        {
            n_extend_fingers = 1;
            Debug.Log("Number:" + 1);
            return 1;
        }

        if (index < 0)
        {
            plane_avatar.SetActive(false);
            Debug.Log("Number" + n_extend_fingers);
            return n_extend_fingers;
        }
        // index > 0 
        n_extend_fingers++;
        plane_avatar.SetActive(true);
        plane_avatar.transform.localScale = new Vector3(.4f, -1, .4f);
        plane_intersect = new Plane(Vector3.Normalize(plane_avatar.transform.up), 
                                plane_avatar.transform.localPosition);

        // Plane is 2D that cannot be seen in 3D => set y = -1
        plane_avatar.transform.localPosition = 
            Rg_left.fingers[index].GetJointPosition(2);

        // Normal vector of the plane 
        plane_avatar.transform.up = Rg_left.fingers[index].GetBoneDirection(2);

        for (int i = 1; i < HandModel.NUM_FINGERS && i != index; ++i)
        {
            float project_distance;
            Ray ray = new Ray(Rg_left.fingers[i].GetJointPosition(0), 
                                Rg_left.fingers[i].GetBoneDirection(2));
            bool intersect = plane_intersect.Raycast(ray, out project_distance);
            if (intersect)
            {
                n_extend_fingers++;
                Debug.Log(Rg_left.fingers[i].fingerType + " -> plane " 
                    + Rg_left.fingers[index].fingerType + " is " + project_distance);
            }
        }

        // corner case
        if (is_extended[0]) n_extend_fingers++;

        string message = $"Number:{n_extend_fingers}";
        Debug.Log(message: message);
        return n_extend_fingers;
    }

    /// <summary>
    /// This function calculate the distance between tipPosition and 
    /// palmPosition. If the distance is less than bone length
    /// </summary>
    public int heuristic3()
    {
        // Square distance between tipPosition and palmPosition.
        float[] SqrDistance = new float[10];

        for (int i = 0; i < HandModel.NUM_FINGERS; ++i)
        {
            SqrDistance[i] = getSquareDistance(
                                Rg_left.fingers[i].GetTipPosition(), 
                                Rg_left.palm.position);
            SqrDistance[i + 5] = getSquareDistance(
                                    Rg_right.fingers[i].GetTipPosition(), 
                                    Rg_right.palm.position);

            if (SqrDistance[i] > bone_length[i] && Rg_left)
            {
                is_extended[i] = true;
            }
            else { is_extended[i] = false; }
            if (SqrDistance[i + 5] > bone_length[i] && Rg_right)
            {
                is_extended[i + 5] = true;
            }
            else { is_extended[i + 5] = false; }
        }
        for (int i = 0; i <2* HandModel.NUM_FINGERS; i++){
            if (is_extended[i]) n_extend_fingers++;
        }

        string message = $"Number:{n_extend_fingers}";
        Debug.Log(message: message);
        return n_extend_fingers;
    }

    private void WriteDataToTxt(){
        if (data_size[FileNumber] >= MAXDATA) {
            Debug.Log("Stop. Data is full");
            return;
            }
        data_size[FileNumber]++;
        for (int i = 0; i < HandModel.NUM_FINGERS; i++)
        {
            msg_buffer = Rg_left.fingers[i].GetTipPosition().ToString() + ','; 
            File.AppendAllText($"{DataPath}/{FileNumber}.txt",msg_buffer);
        }
        msg_buffer = Rg_left.palm.position.ToString() + ',';
        File.AppendAllText($"{DataPath}/{FileNumber}.txt", 
                                msg_buffer + n_extend_fingers.ToString() 
                                + Environment.NewLine); 
    }
}

/*
Other functions:

GetFingerJointStretchMecanim(int joint_type) 
Quaternion GetBoneRotation(int bone_type) 
Vector3 GetBoneDirection(int bone_type) 
Ray GetRay()
Vector3 GetJointPosition(int joint)
Vector3 GetTipPosition()
GetBoneDirection(int bone_type) 
rg_left.GetArmCenter()
rg_left.GetArmRotation()
rg_left.GetArmDirection()
rg_left.GetArmWidth()
rg_left.GetPalmDirection()
rg_left.GetPalmPosition()
rg_left.GetPalmNormal()
rg_left.GetLeapHand()
rg_left.fingers[0].fingerType
*/