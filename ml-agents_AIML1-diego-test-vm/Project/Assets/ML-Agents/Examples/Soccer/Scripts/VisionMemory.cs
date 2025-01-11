using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using UnityEngine;


public class VisionMemory : ISensor
{
    private readonly string sensorName;
    private readonly int observationSize; // Size of each observation
    private readonly int memoryCapacity;  // Number of observations to store
    private readonly RayPerceptionSensorComponent3D raySensor; // Ray sensor reference


    private readonly Queue<float[]> memoryBuffer;
    private List<float[]> memoryBufferDebug; // Debugging List


    public VisionMemory(string name, int obsSize, int memoryCap, RayPerceptionSensorComponent3D raySensorComponent)
    {
        sensorName = name;
        observationSize = obsSize;
        memoryCapacity = memoryCap;
        raySensor = raySensorComponent;


        memoryBuffer = new Queue<float[]>(memoryCapacity);
        memoryBufferDebug = new List<float[]>();
    }


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
        UpdateDebugMemoryBuffer(); // Update debug list
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
        UpdateMemoryFromRaySensor();


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
        return null; // Optional: Implement compression logic if needed
    }


    public void Update()
    {
        Debug.Log("Starting to update VisionMemory");
        UpdateMemoryFromRaySensor();
    }


    public void Reset()
    {
        memoryBuffer.Clear();
        UpdateDebugMemoryBuffer(); // Clear debug list
    }


    public string GetName() => sensorName;


    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default();
    }
}
