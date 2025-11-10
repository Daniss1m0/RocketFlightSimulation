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

    void Start()
    {
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        if (uiManager == null)
        {
            uiManager = Object.FindFirstObjectByType<UIManager>();
            if (uiManager != null && uiManager.rocket == null) uiManager.rocket = this;
        }
    }

    void Update()
    {
        if (!isRunning) return;

        dragForce = -dragCoefficient * velocity * Mathf.Abs(velocity);

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
    }

    public void StartSimulation()
    {
        if (transform.position.y == 0f && !isRunning)
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
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        if (uiManager != null) uiManager.UpdateUI();
    }

    public string GetStatus()
    {
        if (!isRunning && transform.position.y == 0f) return "Rakieta gotowa do startu";
        if (isRunning) return "Rakieta w locie";
        if (!isRunning && transform.position.y > 0f) return "Rakieta zatrzymana";
        return "Rakieta wylądowała";
    }
}
