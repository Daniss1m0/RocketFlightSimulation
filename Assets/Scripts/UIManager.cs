using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider airResistanceSlider;
    public Slider thrustSlider;
    public Slider gravitySlider;

    public Button startButton;
    public Button stopButton;
    public Button resetButton;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI thrustText;
    public TextMeshProUGUI dragText;
    public TextMeshProUGUI heightText;

    public RocketController rocket;

    void Start()
    {
        if (airResistanceSlider != null) airResistanceSlider.minValue = 0f; airResistanceSlider.maxValue = 2f;
        if (thrustSlider != null) thrustSlider.minValue = 0f; thrustSlider.maxValue = 2000f;
        if (gravitySlider != null) gravitySlider.minValue = 0f; gravitySlider.maxValue = 20f;

        if (rocket == null) rocket = Object.FindFirstObjectByType<RocketController>();
        if (rocket != null && rocket.uiManager == null) rocket.uiManager = this;

        if (airResistanceSlider != null)
            airResistanceSlider.onValueChanged.AddListener(value => { if (rocket != null) rocket.dragCoefficient = value; UpdateUI(); });

        if (thrustSlider != null)
            thrustSlider.onValueChanged.AddListener(value => { if (rocket != null) rocket.thrust = value; UpdateUI(); });

        if (gravitySlider != null)
            gravitySlider.onValueChanged.AddListener(value => { if (rocket != null) rocket.gravity = value; UpdateUI(); });

        if (startButton != null && rocket != null) startButton.onClick.AddListener(() => { rocket.StartSimulation(); UpdateUI(); });
        if (stopButton != null && rocket != null) stopButton.onClick.AddListener(() => { rocket.StopSimulation(); UpdateUI(); });
        if (resetButton != null && rocket != null) resetButton.onClick.AddListener(() => { rocket.ResetSimulation(); UpdateUI(); });

        if (rocket != null)
        {
            if (airResistanceSlider != null) rocket.dragCoefficient = airResistanceSlider.value;
            if (thrustSlider != null) rocket.thrust = thrustSlider.value;
            if (gravitySlider != null) rocket.gravity = gravitySlider.value;
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (rocket == null)
        {
            speedText.text = "-";
            thrustText.text = "-";
            dragText.text = "-";
            heightText.text = "-";
            statusText.text = "Brak rakiety";
            return;
        }

        speedText.text = "Prędkość: " + Mathf.Abs(rocket.velocity).ToString("F2") + " m/s";
        thrustText.text = "Siła ciągu: " + rocket.thrust.ToString("F0") + " N";
        dragText.text = "Opór powietrza: " + Mathf.Abs(rocket.dragForce).ToString("F2") + " N";
        heightText.text = "Wysokość: " + rocket.transform.position.y.ToString("F2") + " m";
        statusText.text = "Status: „" + rocket.GetStatus() + "\"";
    }
}
