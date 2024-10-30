using UnityEngine;
using Unity.MLAgents.Sensors;

public class CustomRayPerceptionSensor : MonoBehaviour
{
    public RayPerceptionSensorComponent3D raySensor;

    public void Awake()
    {
        // Locate RayPerceptionSensorComponent3D
        raySensor = GetComponent<RayPerceptionSensorComponent3D>();
        if (raySensor == null)
        {
            Debug.LogError("RayPerceptionSensorComponent3D not found! Attach it in the Unity Editor.");
        }
    }

    public RayPerceptionOutput GetRayPerceptionResults()
    {
        if (raySensor == null)
        {
            Debug.LogError("RayPerceptionSensorComponent3D is missing. Ensure it’s attached to the GameObject.");
            return null;
        }

        RayPerceptionInput input = raySensor.GetRayPerceptionInput();
        Debug.Log("RayPerceptionInput prepared for processing.");
        return RayPerceptionSensor.Perceive(input, batched: false);
    }
}
