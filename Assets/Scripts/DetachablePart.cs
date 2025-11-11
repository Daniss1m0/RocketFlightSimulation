using UnityEngine;

public class DetachablePart : MonoBehaviour
{
    public float breakForce = 50f;
    public float area = 0.1f;
    public float Cd = 1f;
    public float partMass = 0.2f;
    public bool isDetached = false;

    Transform originalParent;
    Vector3 originalLocalPos;
    Quaternion originalLocalRot;
    Rigidbody rb;
    Collider col;

    void Awake()
    {
        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void Detach(Vector3 inheritVelocity, Vector3 flightDir, float dragForce)
    {
        if (isDetached) return;
        transform.parent = null;
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = Mathf.Max(0.001f, partMass);
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = inheritVelocity;
        float impulse = Mathf.Clamp(dragForce * 0.02f, 0f, dragForce * 0.2f);
        rb.AddForce(-flightDir.normalized * impulse, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * impulse, ForceMode.Impulse);
        isDetached = true;
        if (col != null) col.isTrigger = false;
    }

    public void Reattach(Transform parent)
    {
        if (rb != null) Destroy(rb);
        transform.parent = parent == null ? originalParent : parent;
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        isDetached = false;
        col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
    }
}
