using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class MultiDimentionalArray
{
    public int[] myname;
}

public class ObjMovement : MonoBehaviour
{
    public float heightOfTheObjectWithRespectToPlane = 5;

    public bool isCollidingWithAObject;
    public GameObject basePlane;
    public GameObject pointer;

    public bool HitDetacted;

    public Plane objPlane;
    //public Plane planePerpendicularToCollisionVector;

    Vector3 collisionVector;
    Vector3 transformPositionAtTimeFirstCollisionInitiated;
    Vector3 collidingObjPos;

    private void Start()
    {
        objPlane.SetNormalAndPosition(basePlane.transform.up, new Vector3(0, 0, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.tag.Contains("Object"))
        {
            ContactPoint contactPoint = collision.GetContact(0);
            //planePerpendicularToCollisionVector = new  Plane(contactPoint.normal,contactPoint.point);
            transformPositionAtTimeFirstCollisionInitiated = transform.position;
            collisionVector = contactPoint.normal;

            collidingObjPos = collision.gameObject.transform.position;
            isCollidingWithAObject = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag.Contains("Object"))
        {
            isCollidingWithAObject = false;
        }
    }

    bool drop = true;
    private void OnMouseDrag()
    {
        Vector3 planeHitPos = Vector3.zero;
        Ray newray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 9;
        layerMask = ~layerMask;

        if (Physics.Raycast(newray, out RaycastHit hitInfo, Mathf.Infinity,layerMask ))
        {
            if (hitInfo.collider.CompareTag("Plane"))
            {
                planeHitPos = hitInfo.point;

                if (isCollidingWithAObject)
                {
                    //Vector3 ab = collidingObjPos - transform.position;
                    //Vector3 cd = planeHitPos - transform.position;

                    //if (Vector3.Dot(ab, cd) >= 0)
                    //{
                    //    Vector3 cToB = planeHitPos - transformPositionAtTimeFirstCollisionInitiated;

                    //    Vector3 NewCorrectedPosition = transformPositionAtTimeFirstCollisionInitiated +
                    //        (Vector3.Cross(collisionVector, Vector3.Cross(cToB, collisionVector) / collisionVector.magnitude));

                    //    //transform.position = NewCorrectedPosition;
                    //    transform.position = NewCorrectedPosition + objPlane.normal;
                    //}
                    //else
                    //{
                    //    //transform.position = planeHitPos;
                    //    transform.position = planeHitPos + objPlane.normal;
                    //}

                }
                else
                {
                    if (drop)
                    {
                        transform.position = new Vector3(planeHitPos.x, 2f, planeHitPos.z);
                        //drop = false;
                    }
                    else
                        transform.position = new Vector3(planeHitPos.x, transform.position.y, planeHitPos.z);
                    //transform.position = planeHitPos + heightOfTheObjectWithRespectToPlane * objPlane.normal;
                }
                //RaycastHit hitInfoTile;
                if (Physics.Raycast(transform.position, -objPlane.normal, out RaycastHit hitInfoTile, Mathf.Infinity))
                {


                    if (hitInfoTile.collider.tag.Contains("Object"))
                    {
                        pointer.SetActive(false);
                        HitDetacted = true;

                    }
                    else if (hitInfoTile.collider.CompareTag("Plane"))
                    {

                        pointer.transform.position = hitInfoTile.point + 0.1f*objPlane.normal;
                        pointer.SetActive(true);
                        HitDetacted = false;
                    }
                }
            }
        }
    }

    private void OnMouseUp()
    {

        if (pointer.activeInHierarchy)
        {
            transform.position = pointer.transform.position;
            pointer.SetActive(false);
        }

    }





}