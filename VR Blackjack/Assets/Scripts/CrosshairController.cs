using UnityEngine;
using System.Collections;

public class CrosshairController : MonoBehaviour
{
    public Camera playerCamera;
    
    private Vector3 originalScale;
    private GameObject touchingObject;

    public GameObject GetTouchingObject()
    {
        return touchingObject;
    }

    void Start()
    {
        originalScale = transform.localScale;
        touchingObject = null;
    }

    void Update()
    {
        touchingObject = null;
        
        float distance = playerCamera.farClipPlane * 0.95f;
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.rotation * Vector3.forward, out hit))
        {
            distance = hit.distance;
            touchingObject = hit.collider.gameObject;
        }
        transform.position = playerCamera.transform.position + playerCamera.transform.rotation * Vector3.forward * distance;
        
        // try without this to see the difference with vr on
        if (distance < 10.0f) 
        {
            //distance *= 1 + 5 * Mathf.Exp(-distance);
        }
        transform.localScale = originalScale * distance;
        
        transform.LookAt(playerCamera.transform);
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}
