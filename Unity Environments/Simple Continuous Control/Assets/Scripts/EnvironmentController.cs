using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    #region Member Fields
    [SerializeField]
    private GameObject _targetPrefab;
    [SerializeField]
    private GameObject _wallPrefab;
    [SerializeField]
    private float _horizontalLimit = 16.0f;
    [SerializeField]
    private float _verticalLimit = 9.0f;
    [SerializeField]
    private int _numObstacles = 0;
    private GameObject _currentTarget;
    #endregion

    #region Member Properties
    public float HorizontalLimit
    {
        get { return _horizontalLimit; }
    }

    public float VerticalLimit
    {
        get { return _verticalLimit; }
    }

    public Vector3 TargetPosition
    {
        get { return _currentTarget.transform.position; }
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initial setup of the environment by placing walls and obstalces.
    /// </summary>
    private void Start()
    {
        BuildWalls();
        BuildObstacles(); // Not implemented yet
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Destroys the existing target and instantiates a new one.
    /// </summary>
   public void ResetTarget()
    {
        Destroy(_currentTarget);
        SpawnTarget();
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Establishes the boundaries of the environment.
    /// </summary>
    private void BuildWalls()
    {
        // Wall Left
        Vector3 wallPos = new Vector3(transform.position.x - (_horizontalLimit / 2) - 0.5f, transform.position.y, 0);
        GameObject wall = Instantiate(_wallPrefab, wallPos, Quaternion.identity, transform);
        wall.transform.localScale = new Vector3(1, _verticalLimit + 2, 1);
        // Wall Right
        wallPos = new Vector3(transform.position.x + (_horizontalLimit / 2) + 0.5f, transform.position.y, 0);
        wall = Instantiate(_wallPrefab, wallPos, Quaternion.identity, transform);
        wall.transform.localScale = new Vector3(1, _verticalLimit + 2, 1);
        // Wall Top
        wallPos = new Vector3(transform.position.x, transform.position.y + (_verticalLimit / 2) + 0.5f, 0);
        wall = Instantiate(_wallPrefab, wallPos, Quaternion.identity, transform);
        wall.transform.localScale = new Vector3(_horizontalLimit + 2, 1, 1);
        // Wall Bottom
        wallPos = new Vector3(transform.position.x, transform.position.y - (_verticalLimit / 2) - 0.5f, 0);
        wall = Instantiate(_wallPrefab, wallPos, Quaternion.identity, transform);
        wall.transform.localScale = new Vector3(_horizontalLimit + 2, 1, 1);
    }

    /// <summary>
    /// TODO. Randomly places obstacles inside the envionment's limits.
    /// </summary>
    private void BuildObstacles()
    {
        // randomly spawn obstacles
    }

    /// <summary>
    /// Spawns the target at a random position inside the environment's boundaries.
    /// </summary>
    private void SpawnTarget()
    {
        float hLimit = (_horizontalLimit / 2) - 0.25f;
        float vLimit = (_verticalLimit / 2) - 0.25f;
        Vector3 randomPosition = new Vector3(Random.Range(-hLimit, hLimit), Random.Range(-vLimit, vLimit), 0);
        _currentTarget = Instantiate(_targetPrefab, randomPosition, Quaternion.Euler(0, 0, 45), transform);
        _currentTarget.transform.localPosition = randomPosition;
    }
    #endregion
}