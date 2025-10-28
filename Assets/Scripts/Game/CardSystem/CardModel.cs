using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class CardModel : MonoBehaviour
{
    public int InstanceId { get; private set; }
    public int CardId { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFaceUp { get; private set; }

    [SerializeField] GameObject frontBG;
    [SerializeField] Image frontImage, backImage;
    [SerializeField] float flipDuration = 0.25f;

    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        InstanceId = GetInstanceID();
    }

    void OnEnable()
    {
        ResizeCardContent();
        button.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    #region Public

    public void SetData(int cardId, Sprite front)
    {
        CardId = cardId;
        frontImage.sprite = front;

        IsMatched = false;
        IsFaceUp = false;
    }

    public void SetMatched()
    {
        IsMatched = true;
        IsFaceUp = true;

        Invoke(nameof(HideCard), 1f);
    }

    public void HideCard()
    {
        frontBG.SetActive(false);
        backImage.enabled = false;
        button.interactable = false;
    }

    #endregion

    #region Private

    private void OnClick()
    {
        if (IsMatched || IsFaceUp) return;

        StartCoroutine(Flip(true));

        GameManager.Instance.OnCardFlipped(this);
    }

    private void ResizeCardContent()
    {
        // Stretch to full parent
        var frontRect = frontBG.transform as RectTransform;
        frontRect.anchorMin = Vector2.zero;
        frontRect.anchorMax = Vector2.one;
        frontRect.offsetMin = Vector2.zero;
        frontRect.offsetMax = Vector2.zero;
        frontBG.transform.localScale = Vector3.one;
    }

    #endregion

    #region Animation

    public void FlipToFront()
    {
        button.interactable = false;
        StartCoroutine(TweenEx.RotateY(transform, 0f, 180f, flipDuration));
    }

    public void FlipToBack()
    {
        StartCoroutine(TweenEx.RotateY(transform, 180f, 0f, flipDuration, () => button.interactable = true));
    }

    public void FlipToBack(float delay) => StartCoroutine(FlipBackCoroutine(delay));

    IEnumerator FlipBackCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (IsMatched) yield break;

        StartCoroutine(Flip(false));
    }

    IEnumerator Flip(bool faceUp)
    {
        float startY = transform.localEulerAngles.y;
        float endY = faceUp ? 180f : 0f;

        IsFaceUp = faceUp;

        yield return TweenEx.RotateY(transform, startY, endY, flipDuration);

        transform.localEulerAngles = new Vector3(0, endY, 0);
    }

    #endregion

}
