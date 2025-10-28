using UnityEngine;

public static class RectTransformEx
{
    /// <summary>
    /// Stretch a RectTransform to its parent
    /// </summary>
    /// <param name="transform"></param>
    public static void StretchToParent(this Transform transform)
    {
        var rectTransform = transform as RectTransform;
        if (rectTransform == null) return;

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        transform.localScale = Vector3.one;
    }
}