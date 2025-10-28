using UnityEngine;
using System.Collections;
using System;

public static class TweenEx
{
    public static WaitForEndOfFrame waitForEndOfFrame = new();

    public static IEnumerator RotateY(Transform t, float from, float to, float duration, Action callback = null)
    {
        float elapsed = 0f;

        Vector3 startRot = t.localEulerAngles;
        startRot.y = from;

        Vector3 endRot = t.localEulerAngles;
        endRot.y = to;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            float yRotation = Mathf.Lerp(from, to, progress);
            t.localEulerAngles = new Vector3(startRot.x, yRotation, startRot.z);

            yield return waitForEndOfFrame;
        }

        t.localEulerAngles = endRot;

        callback?.Invoke();
    }
}