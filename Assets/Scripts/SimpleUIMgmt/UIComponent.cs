using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIComponent : MonoBehaviour
{
    public bool useInTransition = true;
    public float maxAlpha = 1f;
    public bool instantTransition = false;
    private Vector3 initAnchoredPos;
    public Vector3 InitAnchoredPos { get { return initAnchoredPos; } }
    private bool isRaycastTarget;
    public bool IsRaycastTarget { get { return isRaycastTarget; } }


    void Awake()
    {
        initAnchoredPos = GetComponent<RectTransform>().anchoredPosition;

        var image = GetComponent<Image>();
        if (image != null)
        {
            isRaycastTarget = image.raycastTarget;
        }
        else
        {
            isRaycastTarget = false;
        }
    }
}
