//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: CameraRig
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
public class CameraRigScript : MonoBehaviour
{
    public Transform target;//camera target
    public float distanceToTarget = 3f;// the distance away from the target
    void Start()
    {
        transform.position += new Vector3(0, 0, distanceToTarget);
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}
