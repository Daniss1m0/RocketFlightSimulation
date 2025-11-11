using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Base Offset")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -6f);
    public Vector3 lookAtOffset = Vector3.up * 0.5f;

    [Header("Camera Movement")]
    public float followSpeed = 5f;
    public float rotateSpeed = 8f;
    public bool smoothFollow = true;
    public bool smoothRotate = true;

    [Header("Zoom")]
    public float zoomSpeed = 0.5f;
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 30f;
    public bool invertScroll = false;

    private float currentZoomMultiplier = 1f;

    void Start()
    {
        if (target == null)
        {
            var rc = FindAnyObjectByType<RocketController>();
            if (rc != null) target = rc.transform;
        }

        float offsetMag = Mathf.Max(0.0001f, offset.magnitude);
        float minMul = minZoomDistance / offsetMag;
        float maxMul = maxZoomDistance / offsetMag;
        currentZoomMultiplier = Mathf.Clamp(1f, minMul, maxMul);

        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset * currentZoomMultiplier;
            transform.position = desiredPosition;
            transform.rotation = Quaternion.LookRotation((target.position + lookAtOffset) - transform.position);
        }
    }

    void Update()
    {
        HandleZoom();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset * currentZoomMultiplier;

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

    private void HandleZoom()
    {
        float scroll = 0f;
        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.0001f)
        {
            float direction = invertScroll ? 1f : -1f;
            currentZoomMultiplier += scroll * zoomSpeed * direction;

            float offsetMag = Mathf.Max(0.0001f, offset.magnitude);
            float minMul = minZoomDistance / offsetMag;
            float maxMul = maxZoomDistance / offsetMag;

            currentZoomMultiplier = Mathf.Clamp(currentZoomMultiplier, minMul, maxMul);
        }
    }
}
