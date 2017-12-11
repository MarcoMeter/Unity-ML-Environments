using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC2DAgent : Agent
{
    #region Member Fields
    [SerializeField]
    private GameObject _rocket;
    [SerializeField]
    private float _rocketSpeed = 5.0f;
    [SerializeField]
    private float _rocketCooldown = 0.33f;
    private bool _isCooldown = false;
    [SerializeField]
    private Transform _cannonBarrel;
    [SerializeField]
    private Transform _barrelEnd;

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
    /// Executes the raycasts to observe the state
    /// </summary>
    /// <returns></returns>
    public override List<float> CollectState()
    {
        List<float> state = new List<float>();

        // Update agent's vision
        _rays = new List<Ray>();
        for (int i = 0; i < _numVisionRays; i++)
        {
            Vector3 circumferencePoint = new Vector3(transform.position.x + (_visionRayLength * Mathf.Cos((transform.rotation.eulerAngles.z + (_angleStep * i)) * Mathf.Deg2Rad)),
                                            transform.position.y + (_visionRayLength * Mathf.Sin((transform.rotation.eulerAngles.z + (_angleStep * i)) * Mathf.Deg2Rad)),
                                            0);
            _rays.Add(new Ray(transform.position, (circumferencePoint - transform.position).normalized));
        }

        // Execute raycasts to query the agent's vision
        foreach (var ray in _rays)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _visionRayLength, _layerMask))
            {
                state.Add(hit.distance / (_visionRayLength)); // distance to the comet
            }
            else
            {
                state.Add(0.0f); // if no comet is spotted
            }
        }

        // Agent's z-rotation
        state.Add(transform.rotation.eulerAngles.z / 360);

        // Agent's cooldown
        if(_isCooldown)
        {
            state.Add(1);
        }
        else
        {
            state.Add(0);
        }

        return state;
    }

    /// <summary>
    /// Execution of actions inside of FixedUpdate()
    /// </summary>
    /// <param name="act">Action vector</param>
    public override void AgentStep(float[] action)
    {
        // External control
        if(brain.brainType.Equals(BrainType.External))
        {
            // Agent's rotation
            float zRotation = Mathf.Clamp(action[0], -5, 5);
            transform.Rotate(new Vector3(0, 0, zRotation));

            // Shoot
            float shootAction = Mathf.Clamp(action[1], 0, 1);
            if(shootAction <= 0.8f)
            {
                // Don't shoot
            }
            else if(shootAction > 0.8f)
            {
                Shoot();
            }
        }

        // Player control
        if(brain.brainType.Equals(BrainType.Player))
        {
            // Make cannon look at mouse
            transform.up = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
            CollectState();
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
            reward -= 1;
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
        reward += r;
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Instantiates a projectile and applies a velocity to it.
    /// </summary>
    private void Shoot()
    {
        if (!_isCooldown)
        {
            GameObject rocket = Instantiate(_rocket, _cannonBarrel.position, transform.rotation);
            rocket.GetComponent<Rigidbody>().velocity = (_barrelEnd.position - _cannonBarrel.position).normalized * _rocketSpeed;
            _isCooldown = true;
            StartCoroutine(ProcessCooldown());
        }
    }

    /// <summary>
    /// Resets the cool down.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProcessCooldown()
    {
        yield return new WaitForSeconds(_rocketCooldown);
        _isCooldown = false;
    }
    #endregion
}
