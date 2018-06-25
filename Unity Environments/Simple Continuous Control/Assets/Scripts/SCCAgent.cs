﻿using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SCCAgent : Agent
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
    /// <summary>
    /// Saves the original position of the agent, which indicates the center of the environment.
    /// </summary>
    public override void InitializeAgent()
    {
        _origin = transform.position;
    }

    /// <summary>
    /// Resets the agent to a random position.
    /// </summary>
    public override void AgentReset()
    {
        transform.position = new Vector3(_origin.x + Random.Range((-_env.HorizontalLimit / 2) + 1, (_env.HorizontalLimit / 2) - 1), _origin.y + Random.Range((-_env.VerticalLimit / 2) + 1, (_env.VerticalLimit / 2)) - 1, 0);
        _env.ResetTarget();
    }

    /// <summary>
    /// Gathering inputs based on the direction to the target and the agent's velocity.
    /// </summary>
    public override void CollectObservations()
    {
        List<float> state = new List<float>();
        Vector3 relativePosition = transform.position - _env.TargetPosition;
        state.Add(relativePosition.x / 9);
        state.Add(relativePosition.y / 9);

        Vector3 vel = _rigidbody.velocity.normalized;
        state.Add(vel.x);
        state.Add(vel.y);
        //AddVectorObs(state);
    }

    /// <summary>
    /// Executes the agent's movement based on choosing a moving direction.
    /// Player Input is available as well.
    /// </summary>
    public override void AgentAction(float[] vectorAction, string textAction)
    {

        // One continuous action
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            // Process the action
            float rotationAngle = Mathf.Clamp(vectorAction[0], -1, 1) * 180;
            // Retrieve position form unit circle
            Vector3 circumferencePoint = new Vector3((Mathf.Cos(rotationAngle * Mathf.Deg2Rad)),
                                                    (Mathf.Sin(rotationAngle * Mathf.Deg2Rad)),
                                                    0);
            // Apply velocity based on direction (coming from the unit circle)
            _rigidbody.velocity = circumferencePoint.normalized * _speed;
            
            // Penalize the agent for picking actions, which exceed the absolute threshold of one.
            if(Mathf.Abs(vectorAction[0]) > 1.0f)
            {
                AddReward(-0.0025f * Mathf.Abs(vectorAction[0]));
            }
        }
        else if(brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            int action = Mathf.FloorToInt(vectorAction[0]);
            if (action == 0)
            {
                _rigidbody.velocity = new Vector3(-_speed, 0, 0);
            }
            else if (action == 1)
            {
                _rigidbody.velocity = new Vector3(_speed, 0, 0);
            }
            else if (action == 2)
            {
                _rigidbody.velocity = new Vector3(0, -_speed, 0);
            }
            else if (action == 3)
            {
                _rigidbody.velocity = new Vector3(0, _speed, 0);
            }
            else
            {
                _rigidbody.velocity = new Vector3(0, 0, 0);
            }
            AddReward(-0.001f);
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
            
            AddReward(1.0f);
            Done();
            _env.ResetTarget();
        }
    }
    #endregion
}