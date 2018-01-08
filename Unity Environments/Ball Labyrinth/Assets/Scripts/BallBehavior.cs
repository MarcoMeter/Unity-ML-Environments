using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ball is in charge of signaling rewards to the agent.
/// </summary>
public class BallBehavior : MonoBehaviour
{
    #region Member Fields
    [SerializeField]
    private BallLabyrinthAgent _agent;
    [SerializeField]
    private LayerMask _wallMask;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private Transform _targetGoal;
    [SerializeField]
    private LayerMask _holeMask;
    [SerializeField]
    private int _numRays = 8;
    private float _maxDistance = 11f;
    [SerializeField]
    private float _rayLength = 2.0f;
    private bool _isCornered = false;
    #endregion

    #region Member Properties
    public bool IsCornered
    {
        get { return _isCornered; }
    }
    #endregion

    #region Unity Lifecycle
    private void OnTriggerEnter(Collider other)
    {
        // Check if the ball fell through the right hole
        if(other.tag.Equals("Finish"))
        {
            _agent.BallFinish();
        }
        // Check if the ball fell out of bounds
        else if (other.tag.Equals("Respawn"))
        {
            _agent.BallOutOfBounds();
        }
    }

    private void FixedUpdate()
    {
        CollectBallState();
        // Check if the ball is out of bounds based on position (not collision like above)
        Plane board = new Plane(_agent.transform.up, 0.1f);
        float distance = board.GetDistanceToPoint(transform.localPosition);
        if (distance > 1.0f || distance < -1.0f)
        {
            _agent.BallOutOfBounds();
        }

        // Check if ball is stuck in a corner
        // Setup rays
        Ray[] rays = new Ray[4];
        RaycastHit[] hits = new RaycastHit[rays.Length];
        bool[] wallHits = new bool[rays.Length];
        rays[0] = new Ray(transform.position, -_agent.transform.right); // left
        rays[1] = new Ray(transform.position, _agent.transform.forward); // forward
        rays[2] = new Ray(transform.position, _agent.transform.right); // right
        rays[3] = new Ray(transform.position, -_agent.transform.forward); // back
        // Execute raycasts
        for(int i = 0; i < rays.Length; i++)
        {
            wallHits[i] = Physics.Raycast(rays[i], out hits[i], transform.localScale.x / 2 + 0.005f, _wallMask);
            Debug.DrawLine(rays[i].origin, rays[i].origin + (rays[i].direction.normalized * (transform.localScale.x / 2 + 0.005f)), Color.black, 0.0f);
        }
        // Evaluate raycasts
        _isCornered = false;
        for(int i = 0; i < rays.Length; i++)
        {
            if (i < rays.Length - 1)
            {
                if (wallHits[i] && wallHits[i + 1])
                    _isCornered = true;
            }
            else
            {
                if (wallHits[i] && wallHits[0])
                    _isCornered = true;
            }
        }
        // Signal Reward
        if (_isCornered)
        {
            _agent.BallCornered();
        }
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Collects the ball's state
    /// </summary>
    /// <returns>Returns a list of features to describe the ball's state. Features: Ball position in relation to the final hole, several raycasts to sense its surroundings and its velocity.</returns>
    public List<float> CollectBallState()
    {
        List<float> ballState = new List<float>();
        // Add velocity
        Vector3 normalizedVelocity = _rigidbody.velocity.normalized;
        ballState.Add(normalizedVelocity.x / 2f);
        ballState.Add(normalizedVelocity.y / 2f);
        ballState.Add(normalizedVelocity.z / 2f);
        // Add relative position of the ball to the goal
        Vector3 direction = (transform.position - _targetGoal.position).normalized;
        ballState.Add(direction.x);
        ballState.Add(direction.y);
        ballState.Add(direction.z);
        // Add distance between the ball and the goal
        ballState.Add(Vector3.Distance(transform.position, _targetGoal.position) / _maxDistance);

        // Add ball height
        Ray verticalRay = new Ray(transform.position, -_agent.transform.up);
        RaycastHit floorHit;
        if(Physics.Raycast(verticalRay, out floorHit, 0.5f))
        {
            ballState.Add(floorHit.distance / 0.5f);
            Debug.Log(floorHit.distance);
        }
        else
        {
            ballState.Add(1.0f);
        }

        // Raycast surroundings
        // Create rays
        Ray[] rays = new Ray[_numRays];
        float step = 360 / _numRays;
        for(int i = 0; i < _numRays; i++)
        {
            Vector3 rayDirection = Quaternion.AngleAxis(step * i, _agent.transform.up) * _agent.transform.forward;
            rays[i] = new Ray(transform.position, rayDirection);
        }

        // Draw rays for debugging
        //foreach (var ray in rays)
        //{
        //    Debug.DrawLine(ray.origin, ray.origin + ray.direction * _rayLength, Color.red);
        //}

        // Execute raycasts on walls
        foreach(var ray in rays)
        {
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, _rayLength, _wallMask))
            {
                ballState.Add(hit.distance / _rayLength);
            }
            else
            {
                ballState.Add(1.0f);
            }
        }

        // Execute raycasts on holes
        foreach (var ray in rays)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _rayLength, _holeMask))
            {
                ballState.Add(hit.distance / _rayLength);
            }
            else
            {
                ballState.Add(1.0f);
            }
        }

        return ballState;
    }
    #endregion
}
