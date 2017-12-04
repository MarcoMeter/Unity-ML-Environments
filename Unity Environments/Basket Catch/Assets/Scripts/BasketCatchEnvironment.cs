using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketCatchEnvironment : MonoBehaviour
{
    #region Member Fields
    // Spawn properties
    [SerializeField]
    private GameObject _spawnItemPrefab;
    [SerializeField]
    private int _spawnTimeStep = 1;
    [SerializeField]
    private Transform _spawnBoundaryLeft;
    [SerializeField]
    private Transform _spawnBoundaryRight;
    [SerializeField]
    private float _minSpawnSize = 0.3f;
    [SerializeField]
    private float _maxSpawnSize = 2.0f;
    [SerializeField]
    private float[] _itemDistribution = new float[6]; // Has to sum to 1
    [SerializeField]
    private Material[] _itemMaterials = new Material[6];
    #endregion

    #region Member Properties
    public Transform BoundaryLeft
    {
        get { return _spawnBoundaryLeft; }
    }

    public Transform BoundaryRight
    {
        get { return _spawnBoundaryRight; }
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Initializes members.
    /// </summary>
    private void Start()
    {
        InvokeRepeating("SpawnLoop", 0.0f, _spawnTimeStep);
        InvokeRepeating("SpawnLoop", 0.5f, _spawnTimeStep);
    }
    #endregion

    #region Public Functions
    public void ResetEnvironment()
    {
        
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Each call spawns a new item. Called on Start().
    /// </summary>
    private void SpawnLoop()
    {
        // Randomize spawn size and location
        float posX = Random.Range(_spawnBoundaryLeft.position.x + (_maxSpawnSize + 0.01f), _spawnBoundaryRight.position.x - (_maxSpawnSize + 0.01f));
        float scale = Random.Range(_minSpawnSize, _maxSpawnSize);

        // Spawn and scale item
        GameObject item = Instantiate(_spawnItemPrefab, new Vector3(posX, _spawnBoundaryLeft.position.y + 1, _spawnBoundaryLeft.position.z), Quaternion.identity);
        item.transform.localScale = new Vector3(scale, scale, scale);

        // Initialize item (determine punishment/reward class)
        // Determine item class
        ItemType type = SampleItemType();
        Material mat = DetermineItemMaterial(type);
        item.GetComponent<ItemDrop>().Init(type, mat);
        item.name = type.ToString();
        item.tag = type.ToString();
        // Set physics layer
        if (type.Equals(ItemType.Reward))
        {
            item.layer = 8;
        }
        else
        {
            item.layer = 9;
        }
    }

    /// <summary>
    /// Samples the item type of the supplied probability distribution.
    /// </summary>
    /// <returns>Return the sampled item type.</returns>
    private ItemType SampleItemType()
    {
        ItemType type = ItemType.Reward;
        float rand = Random.Range(0.0f, 1.0f);
        
        if (rand <= _itemDistribution[0])
        {
            type = ItemType.Punishment;
        }
        else
        {
            type = ItemType.Reward;
        }

        return type;
    }

    /// <summary>
    /// Determines the type's materials.
    /// </summary>
    /// <param name="type">Type of the item.</param>
    /// <returns>Returns the related material to the item type.</returns>
    private Material DetermineItemMaterial(ItemType type)
    {
        return _itemMaterials[(int)type];
    }
    #endregion
}
