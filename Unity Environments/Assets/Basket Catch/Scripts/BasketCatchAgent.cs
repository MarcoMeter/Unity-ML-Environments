using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketCatchAgent : Agent
{
    #region Member Fields
    
    
    private float _visionRayLength = 11.0f;
    [SerializeField]
    private EnvironmentController _env;
    private float _intervalSize;
    private Vector3 _agentOrigin;
    [Header("Physics")]
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private Text _scoreText;
    private float _rewardScore = 0;
    private float _punishmentScore = 0;
    [SerializeField]
    private float _agentSpeed = 12.0f;
    [SerializeField]
    private LayerMask _punishmentMask;
    [SerializeField]
    private LayerMask _rewardMask;
    [Header("Agent's Inputs")]
    [SerializeField]
    private float _visionSpectrumLength = 10.0f;
    [SerializeField]
    private bool _centerRay = true;
    [SerializeField]
    private int _numAdditionalRays = 12;
    #endregion

    #region Agent Overrides
    /// <summary>
    /// Initializes the vision of the agent by defining rays for raycasts.
    /// </summary>
    public override void InitializeAgent()
    {
        // Define ray interval size
        _intervalSize = (_visionSpectrumLength - transform.localScale.x) / _numAdditionalRays;
        _agentOrigin = transform.position;
    }

    /// <summary>
    /// Executes the raycasts to observe the state
    /// </summary>
    /// <returns></returns>
    public override List<float> CollectState()
	{
        List<float> state = new List<float>();

        // Initialize and update rays
        List<Ray> visionRays = new List<Ray>();
        float yPos = transform.position.y - (transform.localScale.y / 2);
        // Center ray
        if (_centerRay)
        {
            visionRays.Add(new Ray(new Vector3(transform.position.x, yPos, transform.position.z), transform.up));
        }
        
        // Additional rays start from the outer edges of the basket
        float left = transform.position.x - (transform.localScale.x / 2);
        for (int i = 0; i < _numAdditionalRays / 2; i++)
        {
            Vector3 origin = new Vector3(left - _intervalSize * i, yPos, transform.position.z);
            visionRays.Add(new Ray(origin, transform.up));
        }

        float right = transform.position.x + (transform.localScale.x / 2);
        for (int i = 0; i < _numAdditionalRays / 2; i++)
        {
            Vector3 origin = new Vector3(right + _intervalSize * i, yPos, transform.position.z);
            visionRays.Add(new Ray(origin, transform.up));
        }

        // Execute raycasts (vision of the agent)
        foreach (var ray in visionRays)
        {
            Color debugColor = Color.black;

            // Raycast Reward
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, _visionRayLength, _rewardMask))
            {
                state.Add(hit.distance / (_visionRayLength));
                debugColor = Color.blue;
            }
            else
            {
                state.Add(0.0f);
            }

            // Raycast Punishment
            hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, _visionRayLength, _punishmentMask))
            {
                debugColor = Color.red;
                state.Add(hit.distance / (_visionRayLength));
            }
            else
            {
                state.Add(0.0f);
            }

            Debug.DrawLine(ray.origin, ray.origin + Vector3.up * _visionRayLength, debugColor, 0.0f);
        }

        // Add distance to left wall
        state.Add(Mathf.Abs(_env.BoundaryLeft.position.x - transform.position.x) / 18);
        // Add distance to right wall
        state.Add(Mathf.Abs(_env.BoundaryRight.position.x - transform.position.x) / 18);
        
        return state;
	}

    /// <summary>
    /// Execution of actions inside of FixedUpdate()
    /// </summary>
    /// <param name="act">Action vector</param>
	public override void AgentStep(float[] act)
	{
        if (brain.brainType != BrainType.Heuristic)
        {
            if (brain.brainParameters.actionSpaceType == StateType.discrete)
            {
                int action = (int)act[0];
                if (action == 0)
                {
                    // Move Left
                    _rigidbody.velocity = new Vector3(-1 * _agentSpeed, 0, 0);
                }
                else if (action == 1)
                {
                    // Move Right
                    _rigidbody.velocity = new Vector3(1 * _agentSpeed, 0, 0);
                }
                else
                {
                    // Don't/Stop move
                    _rigidbody.velocity = Vector3.zero;
                }
            }
            else
            {
                Debug.LogError("Action Space should be discrete");
            }
        }
    }

	public override void AgentReset()
	{
        transform.position = _agentOrigin;
        _scoreText.text = "Score: 0";
        _rewardScore = 0;
	}
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Update is used to draw the rays.
    /// </summary>
    //private void Update()
    //{
    //    foreach(var ray in _visionRays)
    //    {
    //        Debug.DrawLine(ray.origin, ray.origin + transform.up * _visionRayLength, Color.red);
    //    }
    //}
    #endregion

    #region Public Functions
    /// <summary>
    /// Rewards the agent
    /// </summary>
    /// <param name="rewardSignal">Reward to signal</param>
    public void EvaluateReward(ItemType itemType)
    {
        if (itemType.Equals(ItemType.Reward))
        {
            reward += 1;
            _rewardScore += 1;
        }
        else
        {
            reward -= 1;
            _punishmentScore -= 1;
        }
        _scoreText.text = "<color=blue>R +" + _rewardScore + "</color>: <color=red>P " + _punishmentScore + "</color>: S: " + (_rewardScore + _punishmentScore);
    }
    #endregion
}