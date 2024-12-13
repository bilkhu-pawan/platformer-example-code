using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    // Update is called once per frame
    public Transform target;
    [SerializeField, Range(0.0f, 1.25f)][Tooltip("The amount of time it takes for the camera to 'catch up'")] public float smoothTime = 0.5F;
    private Vector3 velocity = Vector3.zero;
    public float cameraDistance = -10.0f;

    void Update()
    {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y, cameraDistance), ref velocity, smoothTime);
    }
}
