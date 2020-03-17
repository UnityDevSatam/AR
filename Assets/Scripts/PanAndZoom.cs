using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PanAndZoom : MonoBehaviour
{
    public float zoomOutMin = 1;
    public float zoomOutMax = 8;

    GameObject gObj;
    Collider gObjCollider;
    Plane objPlane;
    Vector3 m0;
    //GameObject pointer;
    bool isCollideWithDefineArea;
    Transform collideAreaTransform;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void zoom(float increment)
    {
        float scale = Mathf.Clamp(transform.localScale.x + increment, zoomOutMin, zoomOutMax);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void Start()
    {
        //if (pointer == null)
        //{
        //    pointer = Instantiate(ARModeManager.Instance.pointerPrefab);
        //    pointer.SetActive(false);
        //}
    }

    private void OnMouseDrag()
    {
        //Scale Object
        if (Input.touchCount == 2)
        {
            Debug.Log("count : " + Input.touchCount);
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.0001f);
        }

    }

    Ray GenerateMouseRay()
    {
        Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

        Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
        Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);
        Ray mr = new Ray(mousePosN, mousePosF - mousePosN);
        return mr;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && Input.touchCount == 1)
        {
            //Rotate Functionality
            if (ARModeManager.Instance.rotate)
            {
                Ray mouseRay = GenerateMouseRay();
                RaycastHit hits;
                if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hits))
                {
                    if (hits.collider.tag.Contains("Object"))
                    {
                        if (Input.touchCount == 1)
                        {
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Moved)
                            {
                                Debug.Log("Touch phase Moved");
                                //ARModeManager.Instance.selectedObject.transform.Rotate(0,
                                //                     -touch.deltaPosition.x * 0.5f, 0, Space.World);
                                hits.collider.gameObject.transform.Rotate(0,
                                                     -touch.deltaPosition.x * 0.5f, 0, Space.World);
                            }
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray mouseRay = GenerateMouseRay();
                    RaycastHit hits;

                    if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hits))
                    {
                        if (hits.collider.tag.Contains("Object"))
                        {
                            gObj = hits.transform.gameObject;
                            gObjCollider = hits.collider;
                            objPlane = new Plane(Camera.main.transform.forward * -1, gObj.transform.position);
                            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                            float rayDist;
                            objPlane.Raycast(mRay, out rayDist);
                            m0 = gObj.transform.position - mRay.GetPoint(rayDist);
                        }
                    }
                }
                else if (Input.GetMouseButton(0) && gObj && gObj == gameObject)
                {
                    Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float rayDist;
                    if (objPlane.Raycast(mRay, out rayDist))
                    {
                        Vector3 pos = mRay.GetPoint(rayDist) + m0;
                        
                        pos.y = gObj.transform.position.y;

                        Vector3 temp = gObj.transform.position - pos;

                        Ray qwe = new Ray(gObj.transform.position, -temp);
                        //if (Physics.BoxCast(gObjCollider.bounds.center, gObj.transform.localScale, -temp, out RaycastHit hit, gObj.transform.rotation, 1f))
                        if (Physics.Raycast(qwe, out RaycastHit hit, 0.2f))
                        {
                            Debug.Log("BoxCast Collide----------->>" + hit.collider.tag);
                            if (hit.collider.tag.Contains("Object"))
                            {
                                Debug.Log("Building Collide----------->>");
                            }
                            else
                            {
                                gObj.transform.position = pos;
                            }
                        }
                        else
                        {
                            gObj.transform.position = pos;
                        }
                    }
                }
                else if (Input.GetMouseButtonUp(0) && gObj && gObj == gameObject)
                {
                    if (isCollideWithDefineArea)
                    {
                        gObj.transform.position = collideAreaTransform.position;
                    }
                    else
                    {
                        StartCoroutine(SmoothMOvement(gObj));
                        //gObj.transform.position = Vector3.MoveTowards(gObj.transform.position,startPos,5f);
                    }
                    gObj = null;
                }
            }
        }
    }

    IEnumerator SmoothMOvement(GameObject targetObject)
    {
        Vector3 startPos = ARModeManager.Instance.planeObject.transform.Find("StartPointPlane").transform.position;
        while (Vector3.Distance(targetObject.transform.position, startPos) > 0.001f)
        {
            targetObject.transform.position = Vector3.Lerp(targetObject.transform.position, startPos, 0.05f);
            yield return new WaitForEndOfFrame();
        }
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter------>>");
        if(gObj == gameObject)
        {
            if(other.gameObject.tag.Equals("SchoolPlane") && gObj.CompareTag("SchoolObject") ||
               other.gameObject.tag.Equals("HousePlane") && gObj.CompareTag("HouseObject") ||
               other.gameObject.tag.Equals("RoadPlane") && gObj.CompareTag("RoadObject"))
            {
                isCollideWithDefineArea = true;
                collideAreaTransform = other.gameObject.transform;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit------>>");
        if (gObj == gameObject)
        {
            if (other.gameObject.tag.Equals("SchoolPlane") && gObj.CompareTag("SchoolObject") ||
               other.gameObject.tag.Equals("HousePlane") && gObj.CompareTag("HouseObject") ||
               other.gameObject.tag.Equals("RoadPlane") && gObj.CompareTag("RoadObject"))
            {
                isCollideWithDefineArea = false;
                collideAreaTransform = null;
            }  
        }
    }

    //private void Update()
    //{
    //    if (EventSystem.current.currentSelectedGameObject == null)
    //    {
    //        if (!ARModeManager.Instance.rotate && Input.touchCount == 1)
    //        {
    //            if (Input.GetMouseButtonDown(0))
    //            {
    //                Ray mouseRay = GenerateMouseRay();
    //                RaycastHit hits;

    //                if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hits))
    //                {
    //                    if (hits.collider.CompareTag("Object"))
    //                    {
    //                        gObj = hits.transform.gameObject;
    //                        gObjCollider = hits.collider;
    //                        objPlane = new Plane(Camera.main.transform.forward * -1, gObj.transform.position);
    //                        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    //                        float rayDist;
    //                        objPlane.Raycast(mRay, out rayDist);
    //                        m0 = gObj.transform.position - mRay.GetPoint(rayDist);
    //                    }
    //                }
    //            }
    //            else if (Input.GetMouseButton(0) && gObj && gObj == gameObject)
    //            {
    //                Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    //                float rayDist;
    //                if (objPlane.Raycast(mRay, out rayDist))
    //                {
    //                    Vector3 pos = mRay.GetPoint(rayDist) + m0;
    //                    //if (ARModeManager.Instance.drop)
    //                    //{
    //                    //    pos.y = ARModeManager.Instance.planeYPose;
    //                    //    //pointer.SetActive(true);
    //                    //    if (Physics.Raycast(transform.position, -objPlane.normal, out RaycastHit hitInfoTile))
    //                    //    {
    //                    //        if (hitInfoTile.collider.CompareTag("Object"))
    //                    //        {
    //                    //            pointer.SetActive(false);
    //                    //        }
    //                    //        else if (hitInfoTile.collider.CompareTag("Plane"))
    //                    //        {
    //                    //            //pointer.transform.position = hitInfoTile.point + 0.05f * ARModeManager.Instance.planeObject.GetComponent<Plane>().normal;
    //                    //            pointer.SetActive(true);
    //                    //        }
    //                    //    }
    //                    //}
    //                    //else
    //                    //{
    //                        pos.y = gObj.transform.position.y;
    //                    //    pointer.SetActive(false);
    //                    //}

    //                    Vector3 temp = gObj.transform.position - pos;

    //                    //RaycastHit hit = new RaycastHit();
    //                    Ray qwe = new Ray(gObj.transform.position, -temp);
    //                    if (Physics.BoxCast(gObjCollider.bounds.center, gObj.transform.localScale, -temp, out RaycastHit hit, gObj.transform.rotation, 1f))
    //                    //if (Physics.Raycast(qwe, out hit, 0.2f))
    //                    {
    //                        Debug.Log("BoxCast Collide----------->>" + hit.collider.tag);
    //                        if (hit.collider.CompareTag("Object"))
    //                        {
    //                            Debug.Log("Building Collide----------->>");
    //                        }
    //                        else
    //                        {
    //                            gObj.transform.position = pos;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        gObj.transform.position = pos;
    //                        //if (pointer.activeSelf)
    //                        //{
    //                        //    pointer.transform.position = new Vector3(gObj.transform.position.x, gObj.transform.position.y - 0.18f, gObj.transform.position.z);
    //                        //    Debug.Log("pointer.position----------->>" + pointer.transform.position);
    //                        //}
    //                        Debug.Log("gObj.position down----------->>" + gObj.transform.position);
    //                    }
    //                }
    //            }
    //            else if (Input.GetMouseButtonUp(0) && gObj && gObj == gameObject)
    //            {
    //                //Vector3 pos = gObj.transform.position;
    //                //if (ARModeManager.Instance.drop && pointer.activeSelf)
    //                //{
    //                //    pos.y -= 0.2f;
    //                //    pointer.SetActive(false);
    //                //}
    //                //else
    //                //{
    //                //    pos.y = gObj.transform.position.y;
    //                //}
    //                //gObj.transform.position = pos;
    //                //Debug.Log("gObj.position up----------->>" + gObj.transform.position);
    //                gObj = null;
    //            }

    //        }
    //        else
    //        {
    //            Ray mouseRay = GenerateMouseRay();
    //            RaycastHit hits;
    //            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hits))
    //            {
    //                if (hits.collider.CompareTag("Object"))
    //                {
    //                    if (Input.touchCount == 1)
    //                    {
    //                        Touch touch = Input.GetTouch(0);
    //                        if (touch.phase == TouchPhase.Began)
    //                        {
    //                            Debug.Log("Touch phase began at: " + touch.position);
    //                        }
    //                        else if (touch.phase == TouchPhase.Moved)
    //                        {
    //                            Debug.Log("Touch phase Moved");
    //                            ARModeManager.Instance.selectedObject.transform.Rotate(0,
    //                                                 -touch.deltaPosition.x * 0.5f, 0, Space.World);
    //                        }
    //                        else if (touch.phase == TouchPhase.Ended)
    //                        {
    //                            Debug.Log("Touch phase Ended");
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
