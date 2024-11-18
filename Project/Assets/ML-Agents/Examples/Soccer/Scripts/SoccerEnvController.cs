using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class SoccerEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentSoccer Agent;
        [HideInInspector] public Vector3 StartingPos;
        [HideInInspector] public Quaternion StartingRot;
        [HideInInspector] public Rigidbody Rb;
    }

    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    public GameObject ball;
    private Rigidbody ballRb;
    private Vector3 m_BallStartingPos;

    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;

    private void Start()
    {
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = ball.transform.position;

        foreach (var player in AgentsList)
        {
            player.StartingPos = player.Agent.transform.position;
            player.StartingRot = player.Agent.transform.rotation;
            player.Rb = player.Agent.GetComponent<Rigidbody>();

            if (player.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(player.Agent);
            }
            else
            {
                m_PurpleAgentGroup.RegisterAgent(player.Agent);
            }
        }

        ResetScene();
    }

    private void FixedUpdate()
    {
        m_ResetTimer++;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_PurpleAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    private void ResetBall()
    {
        ball.transform.position = m_BallStartingPos + new Vector3(Random.Range(-2.5f, 2.5f), 0f, Random.Range(-2.5f, 2.5f));
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }

    public void GoalTouched(Team scoredTeam)
    {
        if (scoredTeam == Team.Blue)
        {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_PurpleAgentGroup.AddGroupReward(-1);
        }
        else
        {
            m_PurpleAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
        }

        m_BlueAgentGroup.EndGroupEpisode();
        m_PurpleAgentGroup.EndGroupEpisode();
        ResetScene();
    }

    private void ResetScene()
    {
        m_ResetTimer = 0;

        foreach (var player in AgentsList)
        {
            player.Agent.transform.SetPositionAndRotation(
                player.StartingPos + new Vector3(Random.Range(-5f, 5f), 0f, 0f),
                Quaternion.Euler(0f, player.Agent.rotSign * Random.Range(80f, 100f), 0f)
            );

            player.Rb.velocity = Vector3.zero;
            player.Rb.angularVelocity = Vector3.zero;
        }

        ResetBall();
    }
}
