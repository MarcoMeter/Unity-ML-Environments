using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    #region Member Fields
    [SerializeField]
    BallLabyrinthAgent _agent;
    #endregion

    #region Unity Lifecycle
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Finish"))
        {
            _agent.BallFinish();
        }
        else if (other.tag.Equals("Respawn"))
        {
            _agent.BallOutOfBounds();
        }
    }
    #endregion

    #region Private Functions

    #endregion
}
