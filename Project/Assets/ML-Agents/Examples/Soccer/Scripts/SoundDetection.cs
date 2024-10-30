using System.Collections.Generic;
using UnityEngine;

public class SoundDetection : MonoBehaviour
{
    public float detectionRange = 5f;

    public List<Vector3> DetectNearbyObjects()
    {
        List<Vector3> nearbyObjects = new List<Vector3>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);

        Debug.Log($"Sound detection triggered. Detecting objects within range: {detectionRange}");
        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                nearbyObjects.Add(collider.transform.position);
                Debug.Log($"Detected object: {collider.gameObject.name} at position {collider.transform.position}");
            }
        }

        return nearbyObjects;
    }
}
