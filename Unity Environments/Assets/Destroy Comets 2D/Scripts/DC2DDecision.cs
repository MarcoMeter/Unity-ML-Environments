using System.Collections.Generic;
using UnityEngine;

public class DC2DDecision : MonoBehaviour, Decision
{
    public float[] Decide(List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
    {
        return null;
    }

    public float[] MakeMemory(List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
    {
        return null;
    }
}