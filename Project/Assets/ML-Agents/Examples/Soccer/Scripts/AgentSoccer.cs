using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

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

    [HideInInspector]
    public Team team;
    float m_KickPower;
    // The coefficient for the reward for colliding with a ball. Set using curriculum.
    float m_BallTouch;
    public Position position;

    const float k_Power = 2000f;
    float m_Existential;
    float m_LateralSpeed;
    float m_ForwardSpeed;

    [HideInInspector]
    public Rigidbody agentRb;
    SoccerSettings m_SoccerSettings;
    BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        // Initialization logic for the agent settings, speeds, and team
        InitializeAgentSettings();
        InitializeTeamSettings();
        InitializeSpeed();
    }

    // Initializes agent-specific settings such as the behavior parameters and Rigidbody component
    private void InitializeAgentSettings()
    {
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        if (envController != null)
        {
            m_Existential = 1f / envController.MaxEnvironmentSteps;
        }
        else
        {
            m_Existential = 1f / MaxStep;
        }

        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();

        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    // Sets the initial position and rotation sign based on the agent's team
    private void InitializeTeamSettings()
    {
        if (m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
            initialPos = new Vector3(transform.position.x - 5f, .5f, transform.position.z);
            rotSign = 1f;
        }
        else
        {
            team = Team.Purple;
            initialPos = new Vector3(transform.position.x + 5f, .5f, transform.position.z);
            rotSign = -1f;
        }
    }

    // Initializes the speed based on the agent's position (Striker, Goalie, Generic)
    private void InitializeSpeed()
    {
        if (position == Position.Goalie)
        {
            m_LateralSpeed = 1.0f;
            m_ForwardSpeed = 1.0f;
        }
        else if (position == Position.Striker)
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.3f;
        }
        else
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.0f;
        }
    }

    // Handles the agent's movement based on received actions
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = GetMoveDirection(act);
        var rotateDir = GetRotationDirection(act[2]);

        ApplyRotation(rotateDir);
        ApplyMovement(dirToGo);
    }

    // Translates the actions into movement directions
    private Vector3 GetMoveDirection(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];

        // Forward/backward movement
        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                m_KickPower = 1f;
                break;
            case 2:
                dirToGo = transform.forward * -m_ForwardSpeed;
                break;
        }

        // Lateral movement
        switch (rightAxis)
        {
            case 1:
                dirToGo += transform.right * m_LateralSpeed;
                break;
            case 2:
                dirToGo += -transform.right * m_LateralSpeed;
                break;
        }

        return dirToGo;
    }

    // Determines the direction for rotation
    private Vector3 GetRotationDirection(int rotateAxis)
    {
        switch (rotateAxis)
        {
            case 1:
                return transform.up * -1f;
            case 2:
                return transform.up * 1f;
            default:
                return Vector3.zero;
        }
    }

    // Applies rotation to the agent
    private void ApplyRotation(Vector3 rotateDir)
    {
        transform.Rotate(rotateDir, Time.deltaTime * 100f);
    }

    // Applies movement forces to the agent's Rigidbody
    private void ApplyMovement(Vector3 dirToGo)
    {
        agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (position == Position.Goalie)
        {
            // Existential bonus for Goalies.
            AddReward(m_Existential);
        }
        else if (position == Position.Striker)
        {
            // Existential penalty for Strikers
            AddReward(-m_Existential);
        }
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
        //right
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 2;
        }
    }

    /// <summary>
    /// Used to provide a "kick" to the ball.
    /// </summary>
    void OnCollisionEnter(Collision c)
    {
        var force = k_Power * m_KickPower;
        if (position == Position.Goalie)
        {
            force = k_Power;
        }
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(.2f * m_BallTouch);
            ApplyKickForce(c, force);
        }
    }

    // Applies the force to the ball on collision
    private void ApplyKickForce(Collision c, float force)
    {
        var dir = c.contacts[0].point - transform.position;
        dir = dir.normalized;
        c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
    }

    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
    }
}
