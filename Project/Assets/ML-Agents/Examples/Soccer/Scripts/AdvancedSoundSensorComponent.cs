using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(AdvancedSoundSensor))] // Ensure the sensor is attached
[AddComponentMenu("ML Agents/Advanced Sound Sensor Component")]
public class AdvancedSoundSensorComponent : SensorComponent
{
    [Tooltip("The name of the sensor as it appears in the training logs.")]
    public string sensorName = "AdvancedSoundSensor";

    [Tooltip("The maximum hearing distance of the sensor.")]
    public float maxHearingDistance = 20f;

    [Tooltip("The rate at which memory of sound events decays over time.")]
    public float memoryDecayRate = 0.1f;

    [Tooltip("The layer mask for obstacles that can block sound.")]
    public LayerMask obstacleMask;

    private AdvancedSoundSensor sensor;

    public override ISensor[] CreateSensors()
    {
        // Get the attached AdvancedSoundSensor component
        sensor = GetComponent<AdvancedSoundSensor>();

        if (sensor == null)
        {
            Debug.LogError("AdvancedSoundSensor is missing! Ensure it is attached to the GameObject.");
            return null;
        }

        // Configure the sensor with current settings
        sensor.maxHearingDistance = Mathf.Max(maxHearingDistance, 0.1f); // Ensure positive distance
        sensor.memoryDecayRate = Mathf.Max(memoryDecayRate, 0f); // Prevent negative decay rate
        sensor.obstacleMask = obstacleMask;

        // Return the sensor as an ISensor array
        return new ISensor[] { sensor };
    }

    private void OnValidate()
    {
        // Warn about invalid settings in the Unity Inspector
        if (maxHearingDistance <= 0)
        {
            maxHearingDistance = 0.1f;
            Debug.LogWarning($"{nameof(maxHearingDistance)} must be greater than 0.");
        }

        if (memoryDecayRate < 0)
        {
            memoryDecayRate = 0f;
            Debug.LogWarning($"{nameof(memoryDecayRate)} cannot be negative.");
        }
    }
}
