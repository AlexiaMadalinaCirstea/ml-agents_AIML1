using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float baseSoundStrength = 1f;

    public void EmitSound()
    {
        Collider[] nearbyAgents = Physics.OverlapSphere(transform.position, 20f);
        foreach (var collider in nearbyAgents)
        {
            var soundSensor = collider.GetComponent<AdvancedSoundSensor>();
            if (soundSensor != null)
            {
                soundSensor.ReceiveSound(transform.position, baseSoundStrength);
            }
        }

        Debug.Log($"Sound emitted from {gameObject.name} at {transform.position}");
    }
}
