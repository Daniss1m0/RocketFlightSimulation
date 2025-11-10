using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -6f);
    public Vector3 lookAtOffset = Vector3.up * 0.5f;
    public float followSpeed = 5f;
    public float rotateSpeed = 8f;
    public bool smoothFollow = true;
    public bool smoothRotate = true;

    void Start()
    {
        if (target == null)
        {
            var rc = Object.FindFirstObjectByType<RocketController>();
            if (rc != null) target = rc.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = smoothFollow
            ? Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime)
            : desiredPosition;

        Vector3 lookPoint = target.position + lookAtOffset;

        if (smoothRotate)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(lookPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.LookAt(lookPoint);
        }
    }
}
