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
    // Note that that the detectable tags are different for the blue and purple teams. The order is
    // * ball
    // * own goal
    // * opposing goal
    // * wall
    // * own teammate
    // * opposing player
    
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

    private VisionMemory visionMemory;
    private AdvancedSoundSensorComponent soundSensorComponent;
    private AdvancedSoundSensor soundSensor;
    private SoundEmitter soundEmitter;

    // Role-specific thresholds
    public float strikerReactionThreshold = 15f;
    public float goalieReactionThreshold = 10f;

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

        // Initialize VisionMemory
        VisionMemorySensorComponent visionMemorySensorComponent = GetComponent<VisionMemorySensorComponent>();
        if (visionMemorySensorComponent != null)
        {
            var sensors = visionMemorySensorComponent.CreateSensors();
            foreach (var sensor in sensors)
            {
                if (sensor is VisionMemory)
                {
                    visionMemory = (VisionMemory)sensor;
                    Debug.Log($"{gameObject.name}: VisionMemory successfully initialized.");
                    break;
                }
            }
        }
        else
        {
            Debug.LogError($"{gameObject.name}: VisionMemorySensorComponent not found! Attach it to the GameObject.");
        }

        // Initialize AdvancedSoundSensor
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
                    Debug.Log($"{gameObject.name}: AdvancedSoundSensor successfully initialized.");
                    break;
                }
            }
        }

        if (soundSensor == null)
        {
            Debug.LogWarning($"{gameObject.name}: AdvancedSoundSensor is not properly initialized.");
        }

        if (soundEmitter == null)
        {
            Debug.LogWarning($"{gameObject.name}: SoundEmitter is not attached.");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect VisionMemory observations
        if (visionMemory != null)
        {
            visionMemory.UpdateMemoryFromRaySensor();
            var visionObservations = visionMemory.GetObservations();
            sensor.AddObservation(visionObservations);
        }

        // Collect AdvancedSoundSensor observations
        if (soundSensor != null)
        {
            Vector3 heardPosition;
            if (soundSensor.GetHeardStatus(out heardPosition))
            {
                sensor.AddObservation(heardPosition);
            }
            else
            {
                sensor.AddObservation(Vector3.zero); // No sound heard
            }
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
        if (position == Position.Goalie) AddReward(m_Existential);
        else if (position == Position.Striker) AddReward(-m_Existential);

        MoveAgent(actionBuffers.DiscreteActions);

        // React to sounds
        if (soundSensor != null)
        {
            Vector3 heardPosition;
            if (soundSensor.GetHeardStatus(out heardPosition))
            {
                float distanceToSound = Vector3.Distance(transform.position, heardPosition);
                Vector3 directionToSound = (heardPosition - transform.position).normalized;

                if (position == Position.Striker && distanceToSound <= strikerReactionThreshold)
                {
                    agentRb.AddForce(directionToSound * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
                    AddReward(0.01f);
                }
                else if (position == Position.Goalie && distanceToSound <= goalieReactionThreshold)
                {
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