using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[System.Serializable]
public class GameStateDto
{
    public int score;
    public int rows, cols;

    public static int Rows { get; private set; }
    public static int Columns { get; private set; }

    public static void SetGridSize(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
    }

    public static Vector2Int GetGridSize() => new(Rows, Columns);
}

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    [SerializeField] private GridController gridController;
    [SerializeField] private CardFactory cardFactory;

    [Header("Config")]
    [Tooltip("Seconds to show all cards at start.")]
    [SerializeField] float previewTime = 2f;

    readonly ISaveService saveService = new PlayerPrefsSaveService();

    readonly List<CardModel> flippedCards = new();
    readonly Dictionary<int, CardModel> allCards = new();

    void Start() => StartNewGame(GameStateDto.Rows, GameStateDto.Columns);

    public void ReturnToMenu() => SceneManager.LoadScene(0);
    
    public void StartNewGame(int r, int c)
    {
        StopAllCoroutines();
        gridController.SetupGrid(c, r);

        int totalCards = r * c;
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
        }

        StartCoroutine(PreviewCards(cardObjects));

        Debug.Log($"New Game Started: {r}x{c}, Total Cards: {cardObjects.Count}, Pairs: {pairCount}");
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
        flippedCards.RemoveRange(0, 2);

        var a = processing[0];
        var b = processing[1];

        if (a.CardId == b.CardId)
        {
            a.SetMatched();
            b.SetMatched();

            AudioManager.Instance?.PlayMatch();
            ComboManager.Instance.RegisterMatch();

            ScoreManager.Instance.IncrementScore();
        }
        else
        {
            a.FlipToBack(0.6f);
            b.FlipToBack(0.6f);

            ComboManager.Instance.ResetCombo();
            AudioManager.Instance?.PlayMismatch();
        }

        // check for level complete (all cards matched)
        if (allCards.Values.All(x => x.IsMatched))
        {
            IncrementDifficulty();
        }
    }

    private void IncrementDifficulty()
    {
        var currentDifficulty = MenuManager.SelectedDifficulty; 
        var difficulties = System.Enum.GetValues(typeof(MenuManager.Difficulty));
        int nextIndex = ((int)currentDifficulty + 1) % difficulties.Length;
        var nextDifficulty = (MenuManager.Difficulty)difficulties.GetValue(nextIndex);

        MenuManager.SelectedDifficulty = nextDifficulty;
        SaveGame();

        // Return to menu if game over
        if (nextDifficulty == MenuManager.Difficulty.Easy)
        { 
            AudioManager.Instance?.PlayGameOver();
            SceneManager.LoadScene(0);
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SaveGame()
    {
        var dto = new GameStateDto
        {
            score = ScoreManager.Instance.Score,
            rows = GameStateDto.Rows,
            cols = GameStateDto.Columns
        };
        
        saveService.Save(dto);
    }
}
