using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float camSpeed;
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * camSpeed);
        transform.position = smoothedPosition;
    }
}
