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
}
