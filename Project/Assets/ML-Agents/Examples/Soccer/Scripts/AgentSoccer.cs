using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class AgentSoccer : Agent
{
    public enum Team { Blue = 0, Purple = 1 }
    public enum Position { Striker, Goalie, Defender }

    // Sensor references
    public CustomRayPerceptionSensor customRaySensor;
    public MemoryComponent memoryComponent;
    public SoundDetection soundDetection;

    [HideInInspector] public Team team;

    private float m_KickPower;
    private float m_BallTouch;
    public Position position;

    const float k_Power = 2000f;
    private float m_Existential;
    private float m_LateralSpeed;
    private float m_ForwardSpeed;

    [HideInInspector] public Rigidbody agentRb;
    private SoccerSettings m_SoccerSettings;
    private BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    private EnvironmentParameters m_ResetParams;
    private float gameScore;
    private bool isAttacker = false;

    public override void Initialize()
    {
        customRaySensor = GetComponent<CustomRayPerceptionSensor>();
        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        agentRb = GetComponent<Rigidbody>();

        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        m_Existential = envController != null ? 1f / envController.MaxEnvironmentSteps : 1f / MaxStep;

        m_BehaviorParameters = GetComponent<BehaviorParameters>();
        team = (m_BehaviorParameters.TeamId == (int)Team.Blue) ? Team.Blue : Team.Purple;
        initialPos = team == Team.Blue
            ? new Vector3(transform.position.x - 5f, .5f, transform.position.z)
            : new Vector3(transform.position.x + 5f, .5f, transform.position.z);
        rotSign = team == Team.Blue ? 1f : -1f;

        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        gameScore = 0;
        UpdateAgentSpeed();
    }

    public void UpdateAgentSpeed()
    {
        m_LateralSpeed = gameScore > 0 ? 1.2f : 0.5f;
        m_ForwardSpeed = gameScore > 0 ? 0.8f : 1.5f;
    }

    public void SwitchRole()
    {
        isAttacker = (team == Team.Blue && SoccerEnvController.ballPosition.z > 0) ||
                     (team == Team.Purple && SoccerEnvController.ballPosition.z < 0);

        if (isAttacker)
        {
            position = Position.Striker;
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.5f;
        }
        else
        {
            position = Position.Defender;
            m_LateralSpeed = 1.2f;
            m_ForwardSpeed = 0.8f;
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

        if (forwardAxis == 1) { dirToGo = transform.forward * m_ForwardSpeed; m_KickPower = 1f; }
        else if (forwardAxis == 2) { dirToGo = transform.forward * -m_ForwardSpeed; }

        if (rightAxis == 1) { dirToGo += transform.right * m_LateralSpeed; }
        else if (rightAxis == 2) { dirToGo += transform.right * -m_LateralSpeed; }

        if (rotateAxis == 1) { rotateDir = transform.up * -1f; }
        else if (rotateAxis == 2) { rotateDir = transform.up * 1f; }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        ProcessCustomSensors();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        SwitchRole();
        AddReward(m_Existential * (position == Position.Goalie ? 1 : (position == Position.Striker ? -1 : 0)));
        UpdateAgentSpeed();
        MoveAgent(actionBuffers.DiscreteActions);
    }

    private void ProcessCustomSensors()
    {
        var perceptionResults = customRaySensor.GetRayPerceptionResults();
        foreach (var hitInfo in perceptionResults.RayOutputs)
        {
            if (hitInfo.HasHit)
            {
                Debug.Log($"Ray hit: {hitInfo.HitGameObject.name}");
            }
        }

        var recentObservations = memoryComponent.GetRecentObservations();
        foreach (var observation in recentObservations) { Debug.Log(observation); }

        var nearbyObjects = soundDetection.DetectNearbyObjects();
        foreach (var obj in nearbyObjects) { Debug.Log($"Detected object at {obj}"); }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
        discreteActionsOut[1] = Input.GetKey(KeyCode.E) ? 1 : (Input.GetKey(KeyCode.Q) ? 2 : 0);
        discreteActionsOut[2] = Input.GetKey(KeyCode.A) ? 1 : (Input.GetKey(KeyCode.D) ? 2 : 0);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("ball"))
        {
            float force = position == Position.Goalie ? k_Power : k_Power * m_KickPower;
            AddReward(.2f * m_BallTouch);
            var dir = (c.contacts[0].point - transform.position).normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
    }

    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
        gameScore = 0;
    }

    public void UpdateGameScore(float score)
    {
        gameScore = score;
        UpdateAgentSpeed();
    }
}
