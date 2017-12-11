using System.Collections.Generic;
using UnityEngine;

public class SSCAgent : Agent
{
    #region Member Fields
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private EnvironmentController _env;
    [SerializeField]
    private float _speed = 3.0f;
    private Vector3 _origin;
    #endregion

    #region Unity ML Agents
    public override void InitializeAgent()
    {
        _origin = transform.position;
    }

    public override void AgentReset()
    {
        transform.position = _origin;
    }

    /// <summary>
    /// Gathering inputs based on the direction to the target and the agent's velocity.
    /// </summary>
    /// <returns>List of inputs to the agent.</returns>
    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        Vector3 dir = (_env.TargetPosition - transform.position).normalized;
        state.Add(dir.x);
        state.Add(dir.y);
        Vector3 vel = _rigidbody.velocity.normalized;
        state.Add(vel.x);
        state.Add(vel.y);
        return state;
    }

    /// <summary>
    /// Execuites the agent's movement.
    /// </summary>
    /// <param name="action"></param>
    public override void AgentStep(float[] action)
    {
        // External: Execute the agents movement
        if (brain.brainType.Equals(BrainType.External))
        {
            float moveHorizontal = Mathf.Clamp(action[0], -1, 1) * _speed;
            float moveVertical = Mathf.Clamp(action[1], -1, 1) * _speed;
            _rigidbody.velocity = new Vector3(moveHorizontal, moveVertical, 0);
        }

        // Player: Input behavior
        if(brain.brainType.Equals(BrainType.Player))
        {
            float horizontal = Input.GetAxis("Horizontal") * _speed;
            float vertical = Input.GetAxis("Vertical") * _speed;
            _rigidbody.velocity = new Vector3(horizontal, vertical, 0);
        }
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Rewards the agent if it hit the target.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Target"))
        {
            reward += 1;
            _env.TargetHit();
        }
    }
    #endregion
}
