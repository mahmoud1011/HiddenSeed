using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class GameStateDto
{
    public int score;
    public List<int> matchedCardIds;
    public int rows, cols;
}

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    [SerializeField] private GridController gridController;
    [SerializeField] private CardFactory cardFactory;

    [Header("Config")]
    [Tooltip("Seconds to show all cards at start.")]
    [SerializeField] float previewTime = 2f;

    [SerializeField] private int rows = 4;
    [SerializeField] private int cols = 4;

    readonly List<CardModel> flippedCards = new();
    readonly Dictionary<int, CardModel> allCards = new();

    void Start() => StartNewGame(rows, cols);

    public void StartNewGame(int r, int c)
    {
        rows = r;
        cols = c;
        StopAllCoroutines();

        gridController.SetupGrid(cols, rows);

        int totalCards = rows * cols;
        int pairCount = totalCards / 2;

        var sprites = cardFactory.GetAvailableFronts();
        if (pairCount > sprites.Length)
        {
            Debug.LogWarning($"Not enough unique sprites! Needed {pairCount}, but only have {sprites.Length}. Reusing some.");
        }

        // Choose and duplicate card fronts
        var chosen = new List<Sprite>();
        for (int i = 0; i < pairCount; i++)
            chosen.Add(sprites[i % sprites.Length]);

        // Create paired IDs
        var ids = new List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // Shuffle paired cards
        ids = ids.OrderBy(_ => Random.value).ToList();

        allCards.Clear();

        // Spawn paired cards
        var cardObjects = new List<CardModel>();
        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];
            var card = cardFactory.CreateCard(id, chosen[id % chosen.Count]);
            card.transform.SetParent(gridController.transform);

            allCards.Add(card.InstanceId, card);
            cardObjects.Add(card);
        }

        // Populate the grid
        gridController.Populate(cardObjects);

        ScoreManager.Instance.Clear();

        // Handle odd grid case: create unmatched card
        if (totalCards % 2 != 0)
        {
            var unmatchedCard = cardFactory.CreateCard(-1, sprites[Random.Range(0, sprites.Length)]);

            unmatchedCard.HideCard(); // Hide during gameplay
            unmatchedCard.transform.SetParent(gridController.transform);

            // Insert in middle of hierarchy (so it's visually in the center)
            int middleIndex = gridController.transform.childCount / 2;
            unmatchedCard.transform.SetSiblingIndex(middleIndex);

            Debug.Log("Added extra unmatched card in the middle of the grid (hidden).");
        }

        // Start preview
        StartCoroutine(PreviewCards(cardObjects));

        Debug.Log($"New Game Started: {rows}x{cols}, Total Cards: {cardObjects.Count}, Pairs: {pairCount}");
    }

    private IEnumerator PreviewCards(List<CardModel> cards)
    {
        foreach (var card in cards)
            card.FlipToFront();

        yield return new WaitForSeconds(previewTime);

        foreach (var card in cards)
            card.FlipToBack();
    }

    public void OnCardFlipped(CardModel card)
    {
        if (flippedCards.Contains(card) || card.IsMatched) return;

        flippedCards.Add(card);

        AudioManager.Instance?.PlayFlip();

        // allow continuous flips — but evaluate matches whenever there are 2 or more un-evaluated flips
        if (flippedCards.Count >= 2)
        {
            EvaluateFlipped();
            ScoreManager.Instance.IncrementTurn();
        }
    }

    private void EvaluateFlipped()
    {
        // local copy so further flips can accumulate but we process the current pair set
        var processing = flippedCards.Take(2).ToList();

        // remove them from the main list so player can keep flipping other cards
        flippedCards.RemoveAt(0);
        flippedCards.RemoveAt(0);

        var a = processing[0];
        var b = processing[1];

        if (a.CardId == b.CardId)
        {
            a.SetMatched();
            b.SetMatched();

            AudioManager.Instance?.PlayMatch();

            ScoreManager.Instance.IncrementScore();
        }
        else
        {
            a.FlipToBack(0.6f);
            b.FlipToBack(0.6f);

            AudioManager.Instance?.PlayMismatch();
        }

        // check for game over (all matched)
        if (allCards.Values.All(x => x.IsMatched))
        {
            AudioManager.Instance?.PlayGameOver();

            SaveGame();
        }
    }

    private void SaveGame()
    {
        var dto = new GameStateDto
        {
            score = ScoreManager.Instance.Score,
            matchedCardIds = allCards.Values.Where(x => x.IsMatched).Select(x => x.CardId).ToList(),
            rows = rows,
            cols = cols
        };
        
        // TODO:: Save game
    }
}
