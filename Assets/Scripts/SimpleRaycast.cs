using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SimpleRaycast : MonoBehaviour
{
    public ARRaycastManager raycastMgr;
    public GameObject objToPlacePrefab;
    public Camera raycastCam;

    private GameObject objInstance;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Update is called once per frame
    void Update()
    {
        if (raycastMgr == null)
            return;

        if (raycastCam == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = raycastCam.ScreenPointToRay(Input.mousePosition);

            if (raycastMgr.Raycast(ray, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
            {
                Pose pose = hits[0].pose;
                if (objInstance == null)
                {
                    objInstance = Instantiate<GameObject>(objToPlacePrefab, pose.position, pose.rotation);
                }
                else
                {
                    objInstance.transform.SetPositionAndRotation(pose.position, pose.rotation);
                }
            }
        }
    }
}
