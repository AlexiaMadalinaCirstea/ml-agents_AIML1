using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class VisionMemory : MonoBehaviour, ISensor
{
    [SerializeField] private string sensorName = "VisionMemory";
    [SerializeField] private int observationSize = 28; // Size of each observation
    [SerializeField] private int memoryCapacity = 5;  // Number of observations to store
    [SerializeField] private RayPerceptionSensorComponent3D raySensor; // Assign in Inspector

    private Queue<float[]> memoryBuffer;

    [SerializeField] private List<float[]> memoryBufferDebug; // Debugging List

    private void Awake()
    {
        memoryBuffer = new Queue<float[]>(memoryCapacity);

        if (raySensor == null)
        {
            raySensor = GetComponent<RayPerceptionSensorComponent3D>();
            if (raySensor == null)
            {
                Debug.LogError("RayPerceptionSensorComponent3D not assigned or found on this GameObject.");
            }
            else
            {
                Debug.Log("RayPerceptionSensorComponent3D successfully assigned in Awake.");
            }
        }
    }

    // Debugging utility: Convert memoryBuffer (Queue) to memoryBufferDebug (List)
    private void UpdateDebugMemoryBuffer()
    {
        memoryBufferDebug = new List<float[]>(memoryBuffer);
    }

    public void AddObservation(float[] observation)
    {
        if (memoryBuffer.Count >= memoryCapacity)
        {
            memoryBuffer.Dequeue();
        }
        memoryBuffer.Enqueue(observation);
        UpdateDebugMemoryBuffer(); // Update debug list for Inspector
        Debug.Log($"Memory updated. Current buffer size: {memoryBuffer.Count}");
    }

    public void UpdateMemoryFromRaySensor()
    {
        if (raySensor == null) return;

        // Fetch ray sensor observations
        var rayPerceptionInput = raySensor.GetRayPerceptionInput();
        var rayPerceptionOutput = RayPerceptionSensor.Perceive(rayPerceptionInput, false); // Use the Perceive method
        var currentObservation = new List<float>();

        // Gather observations from ray perception outputs
        foreach (var rayOutput in rayPerceptionOutput.RayOutputs)
        {
            currentObservation.Add(rayOutput.HitFraction);        // Normalized hit distance
            currentObservation.Add(rayOutput.HitTaggedObject ? 1f : 0f); // Whether a tagged object was hit
        }

        // Store the ray-sensor observations in memory
        AddObservation(currentObservation.ToArray());
        Debug.Log("Updated memory with observations: " + string.Join(", ", currentObservation));
    }

    public float[] GetObservations()
    {
        var observations = new List<float>();
        foreach (var memory in memoryBuffer)
        {
            observations.AddRange(memory);
        }
        return observations.ToArray();
    }

    // ISensor Implementation
    public ObservationSpec GetObservationSpec()
    {
        return ObservationSpec.Vector(observationSize * memoryCapacity);
    }

    public int Write(ObservationWriter writer)
    {
        int offset = 0;
        foreach (var memory in memoryBuffer)
        {
            foreach (var value in memory)
            {
                writer[offset++] = value;
            }
        }
        return offset;
    }

    public byte[] GetCompressedObservation()
    {
        return null; // Optional: Implement compression logic
    }

    public void Update(){
        Debug.Log("Starting to update VisionMemory"); 
        UpdateMemoryFromRaySensor(); 
    }

    public void Reset()
    {
        if (memoryBuffer != null)
        {
            memoryBuffer.Clear();
        }
        UpdateDebugMemoryBuffer(); // Clear debug list as well
    }

    public string GetName() => sensorName;

    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default();
    }

    
}
