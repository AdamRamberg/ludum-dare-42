using UnityEngine;
using System;
using System.Collections;

public static class TransformExtensions
{

    /* Finds a child to this transform by name. Searches not only the first level in the 
	 * tree hierarchy of child objects, but all the children, grand children, and so on.  
	 */
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        var result = parent.Find(name);

        if (result != null)
            return result;

        foreach (Transform child in parent)
        {
            result = child.FindDeepChild(name);
            if (result != null)
                return result;
        }

        return null;
    }

    /* Traverse all the children of the transform and executes the action on this transform,
	 * as well as on all the children.
	 */
    public static void TraverseAndExecute(this Transform current, Action<Transform> action, Func<Transform, bool> until = null)
    {
        if (until != null && until(current))
        {
            return;
        }

        action(current);

        foreach (Transform child in current)
        {
            child.TraverseAndExecute(action);
        }
    }

    /* Traverse all the children of the transform and executes the func on this transform,
	 * as well as on all the children. Will return true if all of the funcs returns true.
	 */
    public static bool TraverseExecuteAndCheck(this Transform current, Func<Transform, bool> func, Func<Transform, bool> until = null)
    {
        if (until != null && until(current))
        {
            return true;
        }

        bool ret = func(current);

        foreach (Transform child in current)
        {
            var temp = child.TraverseExecuteAndCheck(func);
            if (!temp)
                ret = false;
        }

        return ret;
    }
}
