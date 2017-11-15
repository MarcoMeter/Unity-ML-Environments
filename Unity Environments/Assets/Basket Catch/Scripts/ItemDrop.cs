using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    #region Member Fields
    private ItemType _type;
    #endregion

    #region Member Properties
    public ItemType Type
    {
        get { return _type; }
    }
    #endregion

    #region Unity LifeCycle
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Agent"))
        {
            if (_type.Equals(ItemType.Reward))
            {
                other.GetComponent<BasketCatchAgent>().Reward(1);
            }
            else
            {
                other.GetComponent<BasketCatchAgent>().Reward(-1);
            }
            Destroy(gameObject);
        }
        else if (other.tag.Equals("Environment"))
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Functions
    public void Init(ItemType type, Material mat)
    {
        _type = type;
        GetComponent<MeshRenderer>().material = mat;
    }
    #endregion
}
