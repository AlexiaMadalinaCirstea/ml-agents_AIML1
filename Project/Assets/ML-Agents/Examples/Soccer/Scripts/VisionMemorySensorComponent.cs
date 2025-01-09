using UnityEngine;
using Unity.MLAgents.Sensors;

[AddComponentMenu("ML Agents/Vision Memory Sensor Component")]
public class VisionMemorySensorComponent : SensorComponent
{
    [SerializeField] private string sensorName = "VisionMemorySensor";
    [SerializeField] private int observationSize = 28; // Size of a single observation
    [SerializeField] private int memoryCapacity = 5;  // Number of observations to store
    [SerializeField] private RayPerceptionSensorComponent3D raySensor; // Link the ray sensor

    public override ISensor[] CreateSensors()
    {
        if (raySensor == null)
        {
            Debug.LogError("RayPerceptionSensorComponent3D is not assigned.");
            return new ISensor[0];
        }

        // Create the VisionMemory sensor and return it as an array
        var visionMemorySensor = new VisionMemory(sensorName, observationSize, memoryCapacity, raySensor);
        return new ISensor[] { visionMemorySensor };
    }

    
    private void OnEnable()
    {
        if (raySensor == null)
        {
            raySensor = GetComponent<RayPerceptionSensorComponent3D>();
            if (raySensor == null)
            {
                Debug.LogError("RayPerceptionSensorComponent3D not found on this GameObject. Please assign it in the Inspector.");
            }
            else
            {
                Debug.Log("RayPerceptionSensorComponent3D automatically assigned.");
            }
        }
    }

    // Expose parameters for customization in the Unity Inspector
    public string SensorName
    {
        get => sensorName;
        set => sensorName = value;
    }

    public int ObservationSize
    {
        get => observationSize;
        set => observationSize = value;
    }

    public int MemoryCapacity
    {
        get => memoryCapacity;
        set => memoryCapacity = value;
    }

    public RayPerceptionSensorComponent3D RaySensor
    {
        get => raySensor;
        set => raySensor = value;
    }
}
