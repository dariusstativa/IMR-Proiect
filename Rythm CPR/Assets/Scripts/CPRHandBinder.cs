using UnityEngine;
using UnityEngine.InputSystem;

public class CPRHandBinder : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftHandVisual;
    public Transform leftHandSnapPointOnRight;

    public InputActionReference cprHoldAction;
    public float maxSnapDistance = 0.15f;

    public Animator leftHandAnimator;
    public string cprBoolName = "IsCPR";

    private bool isLocked = false;
    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    private void Awake()
    {
        originalParent = leftHandVisual.parent;
        originalLocalPos = leftHandVisual.localPosition;
        originalLocalRot = leftHandVisual.localRotation;
    }

    private void OnEnable()
    {
        cprHoldAction.action.started += OnHoldStarted;
        cprHoldAction.action.canceled += OnHoldCanceled;
        cprHoldAction.action.Enable();
    }

    private void OnDisable()
    {
        cprHoldAction.action.started -= OnHoldStarted;
        cprHoldAction.action.canceled -= OnHoldCanceled;
        cprHoldAction.action.Disable();
    }

    private void OnHoldStarted(InputAction.CallbackContext ctx)
    {
        TryLock();
    }

    private void OnHoldCanceled(InputAction.CallbackContext ctx)
    {
        Unlock();
    }

    private void TryLock()
    {
        if (isLocked) return;

        float dist = Vector3.Distance(leftHand.position, rightHand.position);
        if (dist > maxSnapDistance) return;

        leftHandVisual.SetParent(leftHandSnapPointOnRight, false);
        leftHandVisual.localPosition = Vector3.zero;
        leftHandVisual.localRotation = Quaternion.identity;

        leftHandAnimator?.SetBool(cprBoolName, true);
        isLocked = true;
    }

    private void Unlock()
    {
        if (!isLocked) return;

        leftHandVisual.SetParent(originalParent, false);
        leftHandVisual.localPosition = originalLocalPos;
        leftHandVisual.localRotation = originalLocalRot;

        leftHandAnimator?.SetBool(cprBoolName, false);
        isLocked = false;
    }
}
