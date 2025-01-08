using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float baseSoundStrength = 1f;
    public float soundRange = 20f;

    public void EmitSound()
    {
        Collider[] nearbyAgents = Physics.OverlapSphere(transform.position, soundRange);
        foreach (var collider in nearbyAgents)
        {
            var sensorComponent = collider.GetComponent<AdvancedSoundSensorComponent>();
            if (sensorComponent != null)
            {
                var sensors = sensorComponent.CreateSensors();
                foreach (var sensor in sensors)
                {
                    if (sensor is AdvancedSoundSensor soundSensor)
                    {
                        soundSensor.ReceiveSound(transform.position, baseSoundStrength);
                    }
                }
            }
        }

        Debug.Log($"Sound emitted from {gameObject.name} at {transform.position}");
    }
}