using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DC2DAgent : Agent
{
    #region Member Fields
    [SerializeField]
    private GameObject _rocket;
    [SerializeField]
    private float _rocketSpeed = 5.0f;
    [SerializeField]
    private Transform _cannonBarrel;
    [SerializeField]
    private Transform _barrelEnd;
    private float _maxRotationStep = 5;

    [Header("Agent Inputs")]
    [SerializeField]
    private int _numVisionRays = 16;
    [SerializeField]
    private float _visionRayLength = 5.0f;
    private float _angleStep;
    private List<Ray> _rays;
    [SerializeField]
    private LayerMask _layerMask;
    #endregion

    #region Agent Overrides
    /// <summary>
    /// Initializes the agent's vision, which is based on rays.
    /// </summary>
    public override void InitializeAgent()
    {
        // Initialize rays for the agent's input
        _angleStep = 360.0f / _numVisionRays;
    }

    /// <summary>
    /// Collects the agent's inputs using majorly raycasts.
    /// </summary>
    public override void CollectObservations()
    {
        List<float> state = new List<float>();

        // Update agent's vision
        _rays = new List<Ray>();
        for (int i = 0; i < _numVisionRays; i++)
        {
            Vector3 circumferencePoint = new Vector3(transform.position.x + (_visionRayLength * Mathf.Cos((+0 + (_angleStep * i)) * Mathf.Deg2Rad)),
                                            transform.position.y + (_visionRayLength * Mathf.Sin((transform.rotation.eulerAngles.z + 0 + (_angleStep * i)) * Mathf.Deg2Rad)),
                                            0);
            _rays.Add(new Ray(transform.position, (circumferencePoint - transform.position).normalized));
        }

        // Execute raycasts to query the agent's vision (3 inputs per raycast)
        foreach (var ray in _rays)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _visionRayLength, _layerMask))
            {
                Vector3 cometVelocity = hit.rigidbody.velocity.normalized; // velocity of the comet
                state.Add(cometVelocity.x);
                state.Add(cometVelocity.y);
                state.Add(hit.distance / (_visionRayLength)); // distance to the comet

            }
            else
            {
                // if no comet is spotted
                state.Add(0.0f);
                state.Add(0.0f);
                state.Add(0.0f);
            }
        }

        // Agent's z-rotation
        state.Add(transform.rotation.eulerAngles.z / 360);

        // Agent's shooting direction
        Vector3 shootingDirection = (_barrelEnd.position - _cannonBarrel.position).normalized;
        state.Add(shootingDirection.x);
        state.Add(shootingDirection.y);
        AddVectorObs(state);
    }

    /// <summary>
    /// Execution of actions inside of FixedUpdate()
    /// </summary>
    /// <param name="vectorAction"></param>
    /// <param name="textAction"></param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // External discrete control
        if (brain.brainType.Equals(BrainType.External))
        {
            // Six discrete actions: rotate left, right, shoot and do noting
            // Plus: Combining the rotations with shooting
            int action = (int)vectorAction[0];

            if (action == 0)
            {
                // Rotate left
                transform.Rotate(new Vector3(0, 0, -_maxRotationStep));
            }
            else if (action == 1)
            {
                // Rotate right
                transform.Rotate(new Vector3(0, 0, _maxRotationStep));
            }
            else if (action == 2)
            {
                // Shoot
                Shoot();
            }
            else if (action == 3)
            {
                // Rotate left and shoot
                transform.Rotate(new Vector3(0, 0, -_maxRotationStep));
                Shoot();
            }
            else if (action == 4)
            {
                // Rotate right and shoot
                transform.Rotate(new Vector3(0, 0, -_maxRotationStep));
                Shoot();
            }
            else
            {
                // Do nothing
            }
        }

        // Player control
        if (brain.brainType.Equals(BrainType.Player))
        {
            // Make cannon look at mouse
            transform.up = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        }
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Checks for collisions between the agent and a comet to punish the agent.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Comet"))
        {
            RewardAgent(-1.0f);
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// Executes the player's key inputs.
    /// Draws debug lines for each vision ray.
    /// </summary>
    private void Update()
    {
        // Player control
        if (brain.brainType.Equals(BrainType.Player))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            // Draw vision rays
            foreach(var ray in _rays)
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * _visionRayLength);
            }
        }
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Triggered by the comet, rewards the agent for hitting a comet.
    /// </summary>
    /// <param name="r">The reward to apply to the agent.</param>
    public void RewardAgent(float r)
    {
        AddReward(r);
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Instantiates a projectile and applies a velocity to it.
    /// </summary>
    private void Shoot()
    {
        GameObject rocket = Instantiate(_rocket, _cannonBarrel.position, transform.rotation);
        rocket.GetComponent<Rigidbody>().velocity = (_barrelEnd.position - _cannonBarrel.position).normalized * _rocketSpeed;
        AddReward(-0.05f);
    }
    #endregion
}
