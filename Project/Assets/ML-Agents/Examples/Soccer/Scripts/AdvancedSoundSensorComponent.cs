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

    private void Awake()
    {
        sensor = GetComponent<AdvancedSoundSensor>();
        if (sensor == null)
        {
            Debug.LogError($"{gameObject.name}: AdvancedSoundSensor is missing in Awake!");
        }
        else
        {
            Debug.Log($"{gameObject.name}: AdvancedSoundSensor initialized in Awake.");
        }
    }

    public override ISensor[] CreateSensors()
    {
        sensor = GetComponent<AdvancedSoundSensor>();

        if (sensor == null)
        {
            Debug.LogError($"{gameObject.name}: AdvancedSoundSensor is missing or not initialized!");
            return new ISensor[0];
        }

        Debug.Log($"{gameObject.name}: AdvancedSoundSensor found and initialized successfully.");
        sensor.maxHearingDistance = Mathf.Max(maxHearingDistance, 0.1f);
        sensor.memoryDecayRate = Mathf.Max(memoryDecayRate, 0f);
        sensor.obstacleMask = obstacleMask;

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
