using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{

    public GameObject theCanvas;

    private GameStateManager.GameState currentlyActiveUI;
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
    }

    // Use this for initialization
    void Start()
    {
        // Try to find canvas by tag first
        theCanvas = GameObject.FindGameObjectWithTag(CANVAS);

        // If not found, find canvas by name
        if (theCanvas == null)
            theCanvas = GameObject.Find(CANVAS);

        // Add all game states to internal dictionary keeping track of coroutines per UI panel
        foreach (GameStateManager.GameStateNameEntry e in GameStateManager.instance.gameStateNames)
        {
            coroutineDict.Add(e.name, null);
        }

        currentlyActiveUI = GameStateManager.instance.CurrentGameState.Value;
        GameStateManager.instance.CurrentGameState.Changed += SwitchUI;

        InitUI();
    }

    private void OnDestroy()
    {
        GameStateManager.instance.CurrentGameState.Changed -= SwitchUI;
    }

    private void SwitchUI(GameStateManager.GameState newState)
    {
        SwitchUI(currentlyActiveUI, newState);
        currentlyActiveUI = newState;
    }

    private void SwitchUI(GameStateManager.GameState currentState, GameStateManager.GameState newState)
    {
        foreach (Transform trans in theCanvas.transform)
        {
            if (trans.name == GameStateManager.instance.GetGameStateName(newState))
            {
                // Fade in all children
                if (coroutineDict.ContainsKey(trans.name) && coroutineDict[trans.name] != null)
                    StopCoroutine(coroutineDict[trans.name]);
                var coroutine = StartCoroutine(FadeAlphaTransition(true, trans));
                coroutineDict[trans.name] = coroutine;
            }
            else
            {
                if (coroutineDict.ContainsKey(trans.name) && coroutineDict[trans.name] != null)
                    StopCoroutine(coroutineDict[trans.name]);
                var coroutine = StartCoroutine(FadeAlphaTransition(false, trans));
                coroutineDict[trans.name] = coroutine;
            }
        }
    }

    private void InitUI()
    {
        foreach (Transform trans in theCanvas.transform)
        {
            if (trans.name == GameStateManager.instance.GetGameStateName(GameStateManager.instance.CurrentGameState.Value))
            {
                StartCoroutine(FadeAlphaTransition(true, trans));
            }
            else
            {
                trans.TraverseAndExecute(MakeBtnNonInteractable);
                trans.TraverseAndExecute(MakeImagesNotRaycastTarget);
                trans.TraverseAndExecute(SetHiddenPos);
                trans.TraverseAndExecute(InstantDecreaseAlpha);
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

    IEnumerator FadeAlphaTransition(bool fadeIn, Transform trans)
    {
        var transitionComplete = false;

        if (!fadeIn)
        {
            trans.TraverseAndExecute(MakeBtnNonInteractable);
            trans.TraverseAndExecute(MakeImagesNotRaycastTarget);
        }
        else
        {
            trans.TraverseAndExecute(SetInitPos);
        }

        while (!transitionComplete)
        {
            if (fadeIn)
                transitionComplete = trans.TraverseExecuteAndCheck(IncreaseAlpha);
            else
                transitionComplete = trans.TraverseExecuteAndCheck(DecreaseAlpha);

            yield return null;
        }

        if (fadeIn)
        {
            trans.TraverseAndExecute(MakeBtnInteractable);
            trans.TraverseAndExecute(MakeImagesRaycastTarget);
        }
        else
        {
            trans.TraverseAndExecute(SetHiddenPos);
        }
    }

    // Publicly available varibles, properties and methods
    public static UIManager instance = null; // Static ref

    public void InstantlyHideUI(GameStateManager.GameState gameState)
    {
        foreach (Transform trans in theCanvas.transform)
        {
            if (trans.name == GameStateManager.instance.GetGameStateName(gameState))
            {
                trans.TraverseAndExecute(MakeBtnNonInteractable);
                trans.TraverseAndExecute(MakeImagesNotRaycastTarget);
                trans.TraverseAndExecute(InstantDecreaseAlpha);
                trans.TraverseAndExecute(SetHiddenPos);

                break;
            }
        }
    }
}
