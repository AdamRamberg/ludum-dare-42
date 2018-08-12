using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using ScriptableObjectVariables;

public class UIManager : MonoBehaviour
{

    public GameObject canvas;

    public StringVariable uiState;
    public string initialState;

    private List<UIPanel> uiPanels = new List<UIPanel>();
    private List<Func<Transform, bool>> uiPanelUntilChecks = new List<Func<Transform, bool>>();
    private Dictionary<string, Coroutine> coroutineDict = new Dictionary<string, Coroutine>();

    private const string CANVAS = "Canvas";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        uiPanels = FindObjectsOfType<UIPanel>().ToList();

        for (int i = 0; i < uiPanels.Count; ++i)
        {
            uiPanelUntilChecks.Add((t) => IsUIPanel(t) && t.GetInstanceID() != uiPanels[i].transform.GetInstanceID());
            coroutineDict.Add(uiPanels[i].panelName, null);
        }
    }

    // Use this for initialization
    void Start()
    {
        // Try to find canvas by tag first
        canvas = GameObject.FindGameObjectWithTag(CANVAS);

        // If not found, find canvas by name
        if (canvas == null)
            canvas = GameObject.Find(CANVAS);

        uiState.Value = initialState;
        uiState.Changed += SwitchUI;

        InitUI();
    }

    private void OnDestroy()
    {
        uiState.Changed -= SwitchUI;
    }

    private void SwitchUI(string newState)
    {
        SwitchUI(uiState.PreviousValue, newState);
    }

    private void SwitchUI(string currentState, string newState)
    {
        for (int i = 0; i < uiPanels.Count; ++i)
        {
            if (coroutineDict.ContainsKey(uiPanels[i].panelName) && coroutineDict[uiPanels[i].panelName] != null)
                StopCoroutine(coroutineDict[uiPanels[i].panelName]);
            coroutineDict[uiPanels[i].panelName] = StartCoroutine(FadeAlphaTransition(uiPanels[i].activeOnState.Contains(uiState.Value), uiPanels[i].transform, uiPanelUntilChecks[i]));
        }
    }

    private bool IsUIPanel(Transform trans)
    {
        return trans.GetComponent<UIComponent>() != null;
    }

    private void InitUI()
    {
        for (int i = 0; i < uiPanels.Count; ++i)
        {
            if (uiPanels[i].activeOnState.Contains(uiState.Value))
            {
                StartCoroutine(FadeAlphaTransition(true, uiPanels[i].transform, uiPanelUntilChecks[i]));
            }
            else
            {
                uiPanels[i].transform.TraverseAndExecute(MakeBtnNonInteractable, uiPanelUntilChecks[i]);
                uiPanels[i].transform.TraverseAndExecute(MakeImagesNotRaycastTarget, uiPanelUntilChecks[i]);
                uiPanels[i].transform.TraverseAndExecute(SetHiddenPos, uiPanelUntilChecks[i]);
                uiPanels[i].transform.TraverseAndExecute(InstantDecreaseAlpha, uiPanelUntilChecks[i]);
            }
        }
    }

    private void InstantIncreaseAlpha(Transform trans)
    {
        ChangeAlpha(trans, true, 1f);
    }

    private void InstantDecreaseAlpha(Transform trans)
    {
        ChangeAlpha(trans, false, 1f);
    }

    private bool DecreaseAlpha(Transform trans)
    {
        return ChangeAlpha(trans, false);
    }

    private bool IncreaseAlpha(Transform trans)
    {
        return ChangeAlpha(trans, true);
    }

    private bool ChangeAlpha(Transform trans, bool increaseAlpha, float changeAmount = 0.05f)
    {

        // Must have a UIComponent attached
        var component = trans.GetComponent<UIComponent>();
        if (component == null)
        {
            return true;
        }
        else if (!component.useInTransition)
        {
            return true;
        }

        // Text
        var text = trans.GetComponent<Text>();
        if (text != null)
        {
            Color c = text.color;
            c.a = increaseAlpha ? c.a + changeAmount : c.a - changeAmount;

            if (increaseAlpha && c.a > component.maxAlpha)
                c.a = component.maxAlpha;

            text.color = c;

            return (increaseAlpha && text.color.a >= component.maxAlpha) || (!increaseAlpha && text.color.a <= 0f);
        }

        // Image
        var img = trans.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = increaseAlpha ? c.a + changeAmount : c.a - changeAmount;

            if (increaseAlpha && c.a > component.maxAlpha)
                c.a = component.maxAlpha;

            img.color = c;

            return (increaseAlpha && img.color.a >= component.maxAlpha) || (!increaseAlpha && img.color.a <= 0f);
        }

        // Nothing to change
        return true;
    }

    private void SetBtnInteractable(Transform trans, bool interactable)
    {
        var component = trans.GetComponent<UIComponent>();
        if (component != null && !component.useInTransition)
        {
            return;
        }

        var btn = trans.GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = interactable;
        }
    }

    private void MakeBtnNonInteractable(Transform trans)
    {
        SetBtnInteractable(trans, false);
    }

    private void MakeBtnInteractable(Transform trans)
    {
        SetBtnInteractable(trans, true);
    }

    private void SetRaycastTarget(Transform trans, bool raycastTarget)
    {
        var component = trans.GetComponent<UIComponent>();
        if (component != null && !component.useInTransition)
        {
            return;
        }

        var image = trans.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = !raycastTarget ? false : (component != null ? component.IsRaycastTarget : raycastTarget);
        }
    }

    private void MakeImagesNotRaycastTarget(Transform trans)
    {
        SetRaycastTarget(trans, false);
    }

    private void MakeImagesRaycastTarget(Transform trans)
    {
        var comp = trans.GetComponent<UIComponent>();
        if (comp != null && comp.useInTransition)
        {
            SetRaycastTarget(trans, true);
        }
    }

    // Set UI in correct position
    private void SetInitPos(Transform trans)
    {
        var component = trans.GetComponent<UIComponent>();
        if (component != null && component.useInTransition)
        {
            trans.GetComponent<RectTransform>().anchoredPosition = component.InitAnchoredPos;
        }
    }

    // Move UI far away so that it doesn't affect current UI.
    private void SetHiddenPos(Transform trans)
    {
        var component = trans.GetComponent<UIComponent>();
        if (component != null && component.useInTransition)
        {
            trans.GetComponent<RectTransform>().anchoredPosition = new Vector3(-10000f, -10000f, -10000f);
        }
    }

    IEnumerator FadeAlphaTransition(bool fadeIn, Transform transform, Func<Transform, bool> executeUntil)
    {
        var transitionComplete = false;

        if (!fadeIn)
        {
            transform.TraverseAndExecute(MakeBtnNonInteractable, executeUntil);
            transform.TraverseAndExecute(MakeImagesNotRaycastTarget, executeUntil);
        }
        else
        {
            transform.TraverseAndExecute(SetInitPos, executeUntil);
        }

        while (!transitionComplete)
        {
            if (fadeIn)
                transitionComplete = transform.TraverseExecuteAndCheck(IncreaseAlpha, executeUntil);
            else
                transitionComplete = transform.TraverseExecuteAndCheck(DecreaseAlpha, executeUntil);

            yield return null;
        }

        if (fadeIn)
        {
            transform.TraverseAndExecute(MakeBtnInteractable, executeUntil);
            transform.TraverseAndExecute(MakeImagesRaycastTarget, executeUntil);
        }
        else
        {
            transform.TraverseAndExecute(SetHiddenPos, executeUntil);
        }
    }

    // Publicly available varibles, properties and methods
    public static UIManager instance = null; // Static ref
}
