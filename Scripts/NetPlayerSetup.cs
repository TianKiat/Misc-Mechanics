//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: NetPlayerSetup
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using UnityEngine.Networking;
public class NetPlayerSetup : NetworkBehaviour
{
    public Behaviour[] componentsToDisable;
    public GameObject networkedCamera;
    public LayerMask RemoteLayerMask;
    public LayerMask LocalLayerMask;
    public GameObject m_CameraFocusPoint;
    // Use this for initialization
    void Start()
    {
        //spawn car camera and set the target of the camera
        if (networkedCamera != null)
        {
            GameObject carCamera = Instantiate(networkedCamera, Vector3.zero, Quaternion.identity) as GameObject;
            GetComponent<NetworkCar>().m_Camera = carCamera;
            carCamera.SetActive(false);//default inactive
            if (isLocalPlayer)
            {
                //activate camera and initialize the settings
                carCamera.SetActive(true);
                carCamera.GetComponent<CameraFollow>().SetTarget(m_CameraFocusPoint);
            }
        }
        //if the player is not local disable the components and camera
        if (!isLocalPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
            //enable all components in the array
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else if (isLocalPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerLayer");
        }
    }
}
