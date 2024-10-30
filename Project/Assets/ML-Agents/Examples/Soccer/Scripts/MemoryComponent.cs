using System.Collections.Generic;
using UnityEngine;

public class MemoryComponent : MonoBehaviour
{
    private Queue<Vector3> recentObservations = new Queue<Vector3>();
    public int memoryCapacity = 5; // Customize how many observations to store

    public void AddObservation(Vector3 observation)
    {
        if (recentObservations.Count >= memoryCapacity)
            recentObservations.Dequeue();
        recentObservations.Enqueue(observation);

        Debug.Log($"Added observation at position {observation}. Memory now has {recentObservations.Count} observations.");
    }

    public Vector3[] GetRecentObservations()
    {
        Vector3[] observations = recentObservations.ToArray();
        Debug.Log("Current Memory Observations:");
        foreach (var obs in observations)
        {
            Debug.Log(obs);
        }
        return observations;
    }
}
