using UnityEngine;

public class RocketController : MonoBehaviour
{
    public float dragCoefficient = 0.1f;
    public float thrust = 100f;
    public float gravity = 9.81f;

    public float velocity = 0f;
    public float dragForce = 0f;
    public bool isRunning = false;

    private const float mass = 1f;

    public UIManager uiManager;

    public float partProbeDistance = 0.35f;
    public float airDensity = 1.225f;
    public float detachVelocityMultiplier = 1f;

    DetachablePart[] parts;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null && uiManager.rocket == null) uiManager.rocket = this;
        }
        parts = GetComponentsInChildren<DetachablePart>(true);
    }

    void Update()
    {
        if (!isRunning)
        {
            CheckReattachedPartsOnGround();
            return;
        }

        float vAbs = Mathf.Abs(velocity);
        Vector3 flightDir = velocity >= 0f ? transform.up : -transform.up;

        float sumPartDrag = 0f;
        if (parts != null && parts.Length > 0)
        {
            foreach (var p in parts)
            {
                if (p == null || p.isDetached) continue;
                Vector3 start = p.transform.position + flightDir * 0.01f;
                bool blocked = Physics.Linecast(start, start + flightDir * partProbeDistance);
                if (blocked) continue;
                float partDrag = 0.5f * airDensity * p.Cd * p.area * vAbs * vAbs;
                sumPartDrag += partDrag;
            }
        }

        dragForce = -Mathf.Sign(velocity) * sumPartDrag;

        float netForce = thrust + dragForce - mass * gravity;
        float acceleration = netForce / mass;

        velocity += acceleration * Time.deltaTime;
        transform.position += new Vector3(0f, velocity * Time.deltaTime, 0f);

        if (transform.position.y < 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            velocity = 0f;
            isRunning = false;
        }

        CheckPartsForDetachment(flightDir, vAbs);
    }

    void CheckPartsForDetachment(Vector3 flightDir, float vAbs)
    {
        if (parts == null || parts.Length == 0) return;
        Vector3 inheritVelocity = new Vector3(0f, velocity, 0f);
        foreach (var p in parts)
        {
            if (p == null || p.isDetached) continue;
            Vector3 start = p.transform.position + flightDir * 0.01f;
            bool blocked = Physics.Linecast(start, start + flightDir * partProbeDistance);
            if (blocked) continue;
            float partDrag = 0.5f * airDensity * p.Cd * p.area * vAbs * vAbs;
            if (partDrag > p.breakForce)
            {
                p.Detach(inheritVelocity, flightDir, partDrag);
            }
        }
    }

    void CheckReattachedPartsOnGround()
    {
        if (parts == null || parts.Length == 0) return;
        foreach (var p in parts)
        {
            if (p == null) continue;
            if (p.isDetached) continue;
        }
    }

    public void StartSimulation()
    {
        if (!isRunning)
        {
            isRunning = true;
            if (uiManager != null) uiManager.UpdateUI();
        }
    }

    public void StopSimulation()
    {
        isRunning = false;
        if (uiManager != null) uiManager.UpdateUI();
    }

    public void ResetSimulation()
    {
        isRunning = false;
        velocity = 0f;
        transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
        if (parts != null)
        {
            foreach (var p in parts)
            {
                if (p != null) p.Reattach(this.transform);
            }
            parts = GetComponentsInChildren<DetachablePart>(true);
        }
        if (uiManager != null) uiManager.UpdateUI();
    }

    public string GetStatus()
    {
        if (!isRunning && transform.position.y <= 2.5f) return "Rakieta gotowa do startu";
        if (isRunning) return "Rakieta w locie";
        if (!isRunning && transform.position.y > 2.5f) return "Rakieta zatrzymana";
        return "Rakieta wylądowała";
    }
}
