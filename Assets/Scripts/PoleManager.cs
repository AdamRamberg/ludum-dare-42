using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleManager : MonoBehaviour
{
    public void CleanPoles()
    {
        var poles = GameObject.FindGameObjectsWithTag(TagConstants.POLE);
        for (int i = 0; i < poles.Length; ++i)
        {
            if (poles[i].transform.parent == null)
            {
                Destroy(poles[i].gameObject);
            }
        }
    }
}
