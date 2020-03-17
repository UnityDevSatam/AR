using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARSubsystems;

public class ARModeManager : MonoBehaviour
{
    public static ARModeManager Instance;

    public ARSessionOrigin arSessionOrigin;
    public ARPlaneManager arPlaneManager;

    [HideInInspector]
    public GameObject selectedModel;
    [HideInInspector]
    public GameObject selectedObject;
    [HideInInspector]
    public GameObject planeObject;
    [HideInInspector]
    public float planeYPose;
    [HideInInspector]
    public bool rotate;
    [HideInInspector]
    public bool drop;
    bool onPlaneDetect = false;

    public List<GameObject> instantiatedObjList = new List<GameObject>();

    [Header("UI Elements")]
    [SerializeField]
    GameObject scanPanel;
    [SerializeField]
    GameObject btnObjectList;
    [SerializeField]
    GameObject btnRotate;
    [SerializeField]
    GameObject btnDelete;
    [SerializeField]
    GameObject btnDrag;
    //[SerializeField]
    //GameObject btnDrop;
    [SerializeField]
    GameObject btnSwitch;
    [SerializeField]
    List<Sprite> switchSprites;

    [Header("3D Objects")]
    [SerializeField]
    GameObject planePrefab;
    //[SerializeField]
    //public GameObject pointerPrefab;


    private void OnEnable()
    {
        arPlaneManager.planesChanged += OnPlaneChanged;
    }

    private void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlaneChanged;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        //TrackableCollection<ARPlane> aRPlane = arPlaneManager.trackables;
        //if (selectedModel != null && aRPlane.count > 0 && !drop)
        //{
        //    if (Input.touchCount == 1)
        //    {
        //        var touch = Input.GetTouch(0);
        //        if (touch.phase == TouchPhase.Ended)
        //        {
        //            if (EventSystem.current.currentSelectedGameObject == null)
        //            {
        //                Debug.Log("Not touching a UI button. Moving on.");
        //                RaycastHit rayHit;
                        
        //                bool placementPoseIsValid = true;
        //                Ray newray = Camera.main.ScreenPointToRay(touch.position);

        //                if (Physics.Raycast(newray, out rayHit))
        //                {
        //                    Debug.Log("position--------->>" + rayHit.collider.tag);
        //                    if (rayHit.collider.CompareTag("Plane"))
        //                    {
        //                        Vector3 pose = rayHit.point;

        //                        Collider[] hitColliders = Physics.OverlapBox(pose, new Vector3(0.3f, 0.3f, 0.3f));
        //                        int i = 0;
        //                        while (i < hitColliders.Length)
        //                        {
        //                            if (hitColliders[i].CompareTag("Object"))
        //                            {
        //                                placementPoseIsValid = false;
        //                                return;
        //                            }
        //                            i++;
        //                        }

        //                        if (placementPoseIsValid)
        //                        {
        //                            GameObject obj = Instantiate(selectedModel, new Vector3(pose.x, pose.y, pose.z), Quaternion.identity);
        //                            selectedObject = obj;
        //                            planeYPose = obj.transform.position.y;
        //                            instantiatedObjList.Add(selectedObject);
        //                            btnRotate.SetActive(true);
        //                            btnDelete.SetActive(true);
        //                            btnDrag.SetActive(true);
        //                            //btnDrop.SetActive(true);
        //                            Debug.Log("position--------->>" + obj.transform.position);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

    private void OnPlaneChanged(ARPlanesChangedEventArgs obj)
    {
        if (!onPlaneDetect)
        {
            Debug.Log("OnPlaneChanged --------->>");
            onPlaneDetect = true;
            foreach (var plane in obj.added)
            {
                var hits = new List<ARRaycastHit>();
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                arSessionOrigin.GetComponent<ARRaycastManager>().Raycast(ray, hits, TrackableType.Planes);

                if (hits.Count > 0)
                {
                    Pose pose = hits[0].pose;

                    planeObject = Instantiate(planePrefab, new Vector3(pose.position.x, pose.position.y, pose.position.z), Quaternion.identity);
                    planeYPose = planeObject.transform.position.y;
                    Debug.Log("planeObject position--------->>" + planeObject.transform.position);
                    SwitchSurfaceDetection();

                    scanPanel.SetActive(false);
                    btnObjectList.SetActive(true);
                    break;
                }
            }
        }
    }

    public void RotateEnable(bool isRotate)
    {
        rotate = isRotate;
    }

    public void DeleteObject()
    {
        if (instantiatedObjList.Count > 0)
        {
            GameObject temp = instantiatedObjList[instantiatedObjList.Count - 1];
            instantiatedObjList.RemoveAt(instantiatedObjList.Count - 1);
            Destroy(temp);
            if (instantiatedObjList.Count > 0)
            {
                selectedObject = instantiatedObjList[instantiatedObjList.Count - 1];
            }
            else
            {
                selectedObject = null;
                btnRotate.SetActive(false);
                btnDelete.SetActive(false);
                btnDrag.SetActive(false);
                //btnDrop.SetActive(false);
                //if (btnDrop.GetComponent<Toggle>().isOn)
                //{
                //    btnDrop.GetComponent<Toggle>().isOn = false;
                //    //SwitchDropFunctinality();
                //}
            }
        }
    }

    public void SwitchSurfaceDetection()
    {
        if (arPlaneManager.enabled)
        {
            btnSwitch.GetComponent<Image>().sprite = switchSprites[1];
            arPlaneManager.enabled = false;
        }
        else
        {
            btnSwitch.GetComponent<Image>().sprite = switchSprites[0];
            arPlaneManager.enabled = true;
        }
    }

    //public void SwitchDropFunctinality()
    //{
    //    Debug.Log("drop-------" + drop);
    //    drop = !drop;
    //    Debug.Log("drop-------" + drop);
    //    if (drop)
    //    {
    //        planeYPose += 0.2f;
    //    }
    //    else
    //    {
    //        planeYPose -= 0.2f;
    //    }
    //}
}
