using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public enum Team
{
    Blue = 0,
    Purple = 1
}

public class AgentSoccer : Agent
{
    public enum Position
    {
        Striker,
        Goalie,
        Generic
    }

    [HideInInspector] public Team team;
    private float m_KickPower;
    private float m_BallTouch;
    public Position position;

    private const float k_Power = 2000f;
    private float m_Existential;
    private float m_LateralSpeed;
    private float m_ForwardSpeed;

    [HideInInspector] public Rigidbody agentRb;
    private SoccerSettings m_SoccerSettings;
    private BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    private EnvironmentParameters m_ResetParams;

    private AdvancedSoundSensorComponent soundSensorComponent; // Reference to the sound sensor component
    private AdvancedSoundSensor soundSensor; // Reference to the instantiated AdvancedSoundSensor
    private SoundEmitter soundEmitter;

    // Role-specific thresholds
    public float strikerReactionThreshold = 15f; // Striker reacts to sounds within 15 units
    public float goalieReactionThreshold = 10f;  // Goalie reacts to sounds within 10 units

    public override void Initialize()
    {
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        m_Existential = envController != null
            ? 1f / envController.MaxEnvironmentSteps
            : 1f / MaxStep;

        m_BehaviorParameters = GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
            initialPos = transform.position + new Vector3(-5f, 0.5f, 0f);
            rotSign = 1f;
        }
        else
        {
            team = Team.Purple;
            initialPos = transform.position + new Vector3(5f, 0.5f, 0f);
            rotSign = -1f;
        }

        // Set position-specific movement speeds
        switch (position)
        {
            case Position.Goalie:
                m_LateralSpeed = 1.0f;
                m_ForwardSpeed = 1.0f;
                break;
            case Position.Striker:
                m_LateralSpeed = 0.3f;
                m_ForwardSpeed = 1.3f;
                break;
            default:
                m_LateralSpeed = 0.3f;
                m_ForwardSpeed = 1.0f;
                break;
        }

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        // Initialize sound sensor and emitter
        soundSensorComponent = GetComponent<AdvancedSoundSensorComponent>();
        soundEmitter = GetComponent<SoundEmitter>();

        if (soundSensorComponent != null)
        {
            var sensors = soundSensorComponent.CreateSensors();
            foreach (var sensor in sensors)
            {
                if (sensor is AdvancedSoundSensor)
                {
                    soundSensor = (AdvancedSoundSensor)sensor;
                    break;
                }
            }
        }

        if (soundSensor == null)
        {
            Debug.LogWarning($"{gameObject.name}: AdvancedSoundSensor is not properly initialized. Observations may be incomplete.");
        }

        if (soundEmitter == null)
        {
            Debug.LogWarning($"{gameObject.name}: SoundEmitter is not attached. No sound events will be emitted.");
        }
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        if (forwardAxis == 1) dirToGo = transform.forward * m_ForwardSpeed;
        if (forwardAxis == 2) dirToGo = transform.forward * -m_ForwardSpeed;

        if (rightAxis == 1) dirToGo = transform.right * m_LateralSpeed;
        if (rightAxis == 2) dirToGo = transform.right * -m_LateralSpeed;

        if (rotateAxis == 1) rotateDir = transform.up * -1f;
        if (rotateAxis == 2) rotateDir = transform.up * 1f;

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Rewards/penalties based on role
        if (position == Position.Goalie) AddReward(m_Existential);
        else if (position == Position.Striker) AddReward(-m_Existential);

        // Process agent movement actions
        MoveAgent(actionBuffers.DiscreteActions);

        // Role-specific reactions to sounds
        if (soundSensor != null)
        {
            Vector3 heardPosition;
            if (soundSensor.GetHeardStatus(out heardPosition))
            {
                float distanceToSound = Vector3.Distance(transform.position, heardPosition);
                Debug.Log($"{gameObject.name} heard sound at {heardPosition} (distance: {distanceToSound})");

                // Calculate the direction to the sound
                Vector3 directionToSound = (heardPosition - transform.position).normalized;

                // React based on role and threshold
                if (position == Position.Striker && distanceToSound <= strikerReactionThreshold)
                {
                    // Striker moves toward the sound if within threshold
                    agentRb.AddForce(directionToSound * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
                    AddReward(0.01f);
                }
                else if (position == Position.Goalie && distanceToSound <= goalieReactionThreshold)
                {
                    // Goalie moves away from the sound if within threshold
                    Vector3 directionAwayFromSound = -directionToSound;
                    agentRb.AddForce(directionAwayFromSound * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
                    AddReward(0.01f);
                }
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(.2f * m_BallTouch);
            var dir = (c.contacts[0].point - transform.position).normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * k_Power * m_KickPower);

            // Emit sound on collision
            if (soundEmitter != null)
            {
                soundEmitter.EmitSound();
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? 2 : 0;
        discreteActionsOut[2] = Input.GetKey(KeyCode.A) ? 1 : Input.GetKey(KeyCode.D) ? 2 : 0;
        discreteActionsOut[1] = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? 2 : 0;
    }
}
