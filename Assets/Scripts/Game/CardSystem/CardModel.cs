using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardModel : MonoBehaviour
{
    public int InstanceId { get; private set; }
    public int CardId { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFaceUp { get; private set; }

    [SerializeField] private GameObject frontBG;
    [SerializeField] private Image frontImage, backImage;

    private Button button;
    private Transform transformCache;

    private void Awake()
    {
        transformCache = transform;

        InstanceId = GetInstanceID();
        button = GetComponent<Button>();
    }
}