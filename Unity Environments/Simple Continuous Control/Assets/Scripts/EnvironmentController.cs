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
    private void Start()
    {
        BuildWalls();
        BuildObstacles();
        SpawnTarget();
    }
    #endregion

    #region Public Functions
   public void TargetHit()
    {
        Destroy(_currentTarget);
        SpawnTarget();
    }
    #endregion

    #region Private Functions
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

    private void BuildObstacles()
    {
        // randomly spawn obstacles
    }

    private void SpawnTarget()
    {
        float hLimit = (_horizontalLimit / 2) - 0.25f;
        float vLimit = (_verticalLimit / 2) - 0.25f;
        Vector3 randomPosition = new Vector3(Random.Range(-hLimit, hLimit), Random.Range(-vLimit, vLimit), 0);
        _currentTarget = Instantiate(_targetPrefab, randomPosition, Quaternion.identity);
    }
    #endregion
}
