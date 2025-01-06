using Npgsql;
using UnityEngine;

public class DatabaseLogger
{
    private NpgsqlConnection connection;

    public void ConnectToDatabase(string host, string database, string username, string password)
    {
        string connString = $"Host={host};Username={username};Password={password};Database={database}";
        connection = new NpgsqlConnection(connString);
        connection.Open();
        Debug.Log("Database connected.");
    }

    public void InsertObservation(int agentId, Vector3 soundPosition, float soundStrength)
    {
        using (var cmd = new NpgsqlCommand("INSERT INTO AgentObservations (agent_id, sound_position_x, sound_position_y, sound_position_z, sound_strength) VALUES (@agent_id, @x, @y, @z, @strength)", connection))
        {
            cmd.Parameters.AddWithValue("agent_id", agentId);
            cmd.Parameters.AddWithValue("x", soundPosition.x);
            cmd.Parameters.AddWithValue("y", soundPosition.y);
            cmd.Parameters.AddWithValue("z", soundPosition.z);
            cmd.Parameters.AddWithValue("strength", soundStrength);
            cmd.ExecuteNonQuery();
        }
    }

    public void InsertReward(int agentId, float reward)
    {
        using (var cmd = new NpgsqlCommand("INSERT INTO AgentRewards (agent_id, reward_value) VALUES (@agent_id, @reward)", connection))
        {
            cmd.Parameters.AddWithValue("agent_id", agentId);
            cmd.Parameters.AddWithValue("reward", reward);
            cmd.ExecuteNonQuery();
        }
    }
}
