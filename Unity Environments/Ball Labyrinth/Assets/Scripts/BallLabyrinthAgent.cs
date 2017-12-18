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
    private Rigidbody _ballRigidbody;
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
    }

    /// <summary>
    /// Move the ball back to its original position on the board.
    /// </summary>
    public override void AgentReset()
    {
        _ball.transform.position = _ballPositions[_academy.BallPositionIndex].position;
        _ballRigidbody.velocity = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    ///  Gathering inputs.
    /// </summary>
    /// <returns></returns>
    public override List<float> CollectState()
    {
        List<float> state = new List<float>
        {
            _ballRigidbody.velocity.x / 2f,
            _ballRigidbody.velocity.x / 2f,
            _ballRigidbody.velocity.x / 2f,
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.z
        };
        return state;
    }

    /// <summary>
    /// Executing decisions.
    /// </summary>
    /// <param name="action"></param>
    public override void AgentStep(float[] action)
    {
        if (brain.brainType.Equals(BrainType.External))
        {
            // X rotation
            float action_x = Mathf.Clamp(action[1], -2f, 2f);
            if ((gameObject.transform.rotation.x < 0.25f && action_x > 0f) ||
                (gameObject.transform.rotation.x > -0.25f && action_x < 0f))
            {
                gameObject.transform.Rotate(new Vector3(1, 0, 0), action_x);
            }

            // Z rotation
            float action_z = Mathf.Clamp(action[0], -2f, 2f);
            if ((gameObject.transform.rotation.z < 0.25f && action_z > 0f) ||
                (gameObject.transform.rotation.z > -0.25f && action_z < 0f))
            {
                gameObject.transform.Rotate(new Vector3(0, 0, 1), action_z);
            }
        }
        else if(brain.brainType.Equals(BrainType.Player))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), _playerVertical);
            gameObject.transform.Rotate(new Vector3(0, 0, -1), _playerHorizontal);
        }

        // TODO: Maybe clamp rotation
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Ball fell through the wrong hole.
    /// </summary>
    public void BallOutOfBounds()
    {
        done = true;
        reward += -1f;
    }

    /// <summary>
    /// Ball reached the final hole.
    /// </summary>
    public void BallFinish()
    {
        done = true;
        reward += 1f;
    }
    #endregion

    #region Private Functions
    
    #endregion
}
