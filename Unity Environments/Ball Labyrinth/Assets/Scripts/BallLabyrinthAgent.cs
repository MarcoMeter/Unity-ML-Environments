using System;
using System.Collections.Generic;
using UnityEngine;

public class BallLabyrinthAgent : Agent
{
    #region Member Fields
    BallLabyrinthAcademy _academy;
    [SerializeField]
    private Transform[] _ballPositions;
    [SerializeField]
    private GameObject _ball;
    [SerializeField]
    private float _maxRotation = 45f;
    [SerializeField]
    private Transform[] _cornerPositions;
    private Rigidbody _ballRigidbody;
    private BallBehavior _ballBehavior;
    private float _playerHorizontal = 0f;
    private float _playerVertical = 0f;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Polls player input.
    /// </summary>
    private void Update()
    {
        // Read player inputs
        if (brain.brainType.Equals(BrainType.Player))
        {
            _playerHorizontal = Input.GetAxis("Horizontal");
            _playerVertical = Input.GetAxis("Vertical");
        }
    }
    #endregion

    #region Agent Overrides
    /// <summary>
    /// Initializes members.
    /// </summary>
    public override void InitializeAgent()
    {
        _academy = GameObject.FindGameObjectWithTag("Academy").GetComponent<BallLabyrinthAcademy>();
        _ballRigidbody = _ball.GetComponent<Rigidbody>();
        _ballBehavior = _ball.GetComponent<BallBehavior>();
    }

    /// <summary>
    /// Move the ball back to its original position on the board.
    /// </summary>
    public override void AgentReset()
    {
        _ballRigidbody.velocity = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        _ball.transform.position = _ballPositions[_academy.BallPositionIndex].position;
    }
    
    /// <summary>
    ///  Gathering inputs.
    /// </summary>
    public override void CollectObservations()
    {
        // Total 43 inputs
        List<float> state = _ballBehavior.CollectBallState(); // 40 inputs collected
        state.Add(transform.localEulerAngles.x / 360f);
        state.Add(transform.localEulerAngles.z / 360f);
        state.Add(Convert.ToSingle(_ballBehavior.IsCornered));
        AddVectorObs(state);
    }

    /// <summary>
    /// Executing decisions.
    /// </summary>
    /// <param name="vectorAction"></param>
    /// <param name="textAction"></param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainType.Equals(BrainType.External))
        {
            // Z rotation
            float zRotation = Mathf.Clamp(vectorAction[0], -1f, 1f);
            transform.Rotate(new Vector3(0, 0, -1), zRotation, Space.Self);

            // X rotation
            float xRotation = Mathf.Clamp(vectorAction[1], -1f, 1f);
            transform.Rotate(new Vector3(1, 0, 0), xRotation, Space.World);
        }
        else if (brain.brainType.Equals(BrainType.Player))
        {
            transform.Rotate(new Vector3(0, 0, -1), _playerHorizontal, Space.Self);
            transform.Rotate(new Vector3(1, 0, 0), _playerVertical, Space.World);
        }

        // Clamp rotation
        var rotateX = Mathf.Clamp((transform.localEulerAngles.x <= 180) ? transform.localEulerAngles.x : -(360 - transform.localEulerAngles.x), -_maxRotation, _maxRotation);
        var rotateZ = Mathf.Clamp((transform.localEulerAngles.z <= 180) ? transform.localEulerAngles.z : -(360 - transform.localEulerAngles.z), -_maxRotation, _maxRotation);
        transform.localEulerAngles = new Vector3(rotateX, 0, rotateZ);
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Reward Signal: Ball fell through the wrong hole.
    /// </summary>
    public void BallOutOfBounds()
    {
        Done();
        AddReward(-1.0f);
    }

    /// <summary>
    /// Reward Signal: Ball reached the final hole.
    /// </summary>
    public void BallFinish()
    {
        Done();
        AddReward(1.5f);
    }

    /// <summary>
    /// Reward Signal: Ball is stuck in a corner.
    /// </summary>
    public void BallCornered()
    {
        AddReward(-0.05f);
    }
    #endregion
}