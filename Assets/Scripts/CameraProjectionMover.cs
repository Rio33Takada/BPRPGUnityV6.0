using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraProjectionMover : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform perspectiveTransform;
    [SerializeField] private Transform orthographicTransform;

    [Header("FOV Settings")]
    [SerializeField] private float toPerspective = 60f;
    [SerializeField] private float toOrthographic = 5f;

    [SerializeField] private float orthographicSize = 5.0f;

    [Header("Animation")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField]
    private AnimationCurve transFormEaseCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField]
    private AnimationCurve fOVEaseCurve = 
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Camera cam;
    private bool isTransitioning;

    [SerializeField] MeshRenderer boardFade;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = toPerspective;
    }

    public void Toggle()
    {
        if (isTransitioning) return;

        if (cam.orthographic)
            StartCoroutine(ToPerspective());
        else
            StartCoroutine(ToOrthographic());
    }

    // Perspective → Orthographic
    private IEnumerator ToOrthographic()
    {
        isTransitioning = true;

        // 移動 + FOV補間
        yield return MoveWithFOV(
            orthographicTransform,
            toPerspective,
            toOrthographic
        );

        // projection切替
        cam.orthographic = true;
        cam.orthographicSize = orthographicSize;

        boardFade.enabled = true;

        isTransitioning = false;
    }

    // Orthographic → Perspective
    private IEnumerator ToPerspective()
    {
        isTransitioning = true;

        // projection切替
        cam.orthographic = false;

        boardFade.enabled = false;

        // 移動 + FOV補間
        yield return MoveWithFOV(
            perspectiveTransform,
            toOrthographic,
            toPerspective
        );

        isTransitioning = false;
    }

    private IEnumerator MoveWithFOV(
        Transform target,
        float startFov,
        float endFov)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float tt = transFormEaseCurve.Evaluate(time / moveDuration);
            float ft = fOVEaseCurve.Evaluate(time / moveDuration);

            // 位置・回転補間
            transform.position = Vector3.Lerp(startPos, endPos, tt);
            transform.rotation = Quaternion.Slerp(startRot, endRot, tt);

            // FOV補間（指定通り）
            cam.fieldOfView = Mathf.Lerp(startFov, endFov, ft);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;
        cam.fieldOfView = endFov;
    }
}
