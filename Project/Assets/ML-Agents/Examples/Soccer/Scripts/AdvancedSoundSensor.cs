using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class AdvancedSoundSensor : MonoBehaviour, ISensor
{
    private List<SoundEvent> heardSounds = new List<SoundEvent>();
    private string sensorName = "AdvancedSoundSensor";
    private ObservationSpec observationSpec;

    public float memoryDecayRate = 0.1f; 
    public float maxHearingDistance = 20f; //max distance the sensor can detect sounds
    public LayerMask obstacleMask; //mask for objects that can block sound

    private struct SoundEvent
    {
        public Vector3 position;
        public float strength; //this is scaled by distance and occlusion
        public float timeHeard;
    }

    private void Awake()
    {
        observationSpec = ObservationSpec.Vector(6); 
    }

    public void ReceiveSound(Vector3 soundPosition, float baseStrength = 1f)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);

        //ignore sounds beyond max distance
        if (distance > maxHearingDistance) return;

        //check for obstacles
        if (Physics.Linecast(transform.position, soundPosition, obstacleMask))
        {
            baseStrength *= 0.5f; //halve the strength for occluded sounds
        }

        //calculate sound strength based on distance
        float strength = baseStrength * (1f - (distance / maxHearingDistance));

        //add sound event
        heardSounds.Add(new SoundEvent
        {
            position = soundPosition,
            strength = strength,
            timeHeard = Time.time
        });

        Debug.Log($"{gameObject.name} heard a sound at {soundPosition} with strength {strength}");
    }

    public string GetName()
    {
        return sensorName;
    }

    public int Write(ObservationWriter writer)
    {
        //write the most recent sound information
        if (heardSounds.Count > 0)
        {
            var latestSound = heardSounds[heardSounds.Count - 1];
            Vector3 directionToSound = (latestSound.position - transform.position).normalized;

            writer.Add(directionToSound);
            writer.Add(new Vector3(latestSound.strength, 0f, 0f));
            writer.Add(new Vector3(latestSound.position.x, 0f, latestSound.position.z));
        }
        else
        {
            //write zero if no sound was heard
            writer.Add(Vector3.zero);
            writer.Add(Vector3.zero);
            writer.Add(Vector3.zero);
        }

        return 7;//this has to coincide with ObservationSpec.Vector(n) 
    }

    public byte[] GetCompressedObservation()
    {
        return null;
    }

    public void Update()
    {
        //decay heard sounds over time
        for (int i = heardSounds.Count - 1; i >= 0; i--)
        {
            SoundEvent sound = heardSounds[i];
            sound.strength -= memoryDecayRate * Time.deltaTime;

            if (sound.strength <= 0)
            {
                heardSounds.RemoveAt(i);
            }
            else
            {
                heardSounds[i] = sound;
            }
        }
    }

    public void Reset()
    {
        heardSounds.Clear();
    }

    public ObservationSpec GetObservationSpec()
    {
        return observationSpec;
    }

    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default();
    }

     public bool GetHeardStatus(out Vector3 position)
    {
        position = Vector3.zero;

        if (heardSounds.Count > 0)
        {
            var latestSound = heardSounds[heardSounds.Count - 1];
            position = latestSound.position;
            return true;
        }

        return false;
    }
}



//think if you should add player footsteps to the sensor