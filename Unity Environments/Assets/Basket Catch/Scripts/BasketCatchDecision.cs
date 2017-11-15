using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityEngine;

public class BasketCatchDecision : MonoBehaviour, Decision
{
    void Start()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
    }

	public float[] Decide (List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
	{
        // Just stuff to observe inputs
        //float[] array = state.ToArray();
        //string[] strings = array.Select(f => f.ToString(CultureInfo.CurrentCulture)).ToArray();
        //string output =  string.Join(", ", strings);
        //Debug.Log(output);
        return default(float[]);
	}

	public float[] MakeMemory (List<float> state, List<Camera> observation, float reward, bool done, float[] memory)
	{
		return default(float[]);
		
	}
}