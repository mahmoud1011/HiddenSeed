using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : Singleton<MenuManager>
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Toggle[] difficultyToggles;
    [SerializeField] private string gameSceneName = "Game";

    public enum Difficulty { Easy, Medium, Hard, Expert, Master }

    public static Difficulty SelectedDifficulty
    {
        get => GetDifficultyFromGrid(GameStateDto.GetGridSize());
        set => GameStateDto.SetGridSize(difficultyGridSizes[(int)value].x, difficultyGridSizes[(int)value].y);
    }

    static readonly Vector2Int[] difficultyGridSizes = new Vector2Int[]
    {
        new(3, 3),   // Easy 
        new(4, 4),   // Medium 
        new(4, 5),   // Hard 
        new(5, 6),   // Expert 
        new(6, 6)    // Master 
    };

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);

        // Assign toggle listeners
        for (int i = 0; i < difficultyToggles.Length; i++)
        {
            int index = i;
            difficultyToggles[i].onValueChanged.AddListener((isOn) => OnDifficultyToggled(index, isOn));
        }

        LoadSelection();
    }

    protected override void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayClicked);

        for (int i = 0; i < difficultyToggles.Length; i++)
        {
            int index = i;
            difficultyToggles[i].onValueChanged.RemoveListener((isOn) => OnDifficultyToggled(index, isOn));
        }

        base.OnDestroy();
    }

    private void LoadSelection()
    {
        var savedGrid = GameStateDto.GetGridSize();
        var savedDifficulty = GetDifficultyFromGrid(savedGrid);

        SelectedDifficulty = savedDifficulty;
        difficultyToggles[(int)savedDifficulty].isOn = true;
    }

    private static Difficulty GetDifficultyFromGrid(Vector2Int grid)
    {
        for (int i = 0; i < difficultyGridSizes.Length; i++)
        {
            if (difficultyGridSizes[i] == grid)
                return (Difficulty)i;
        }
        return Difficulty.Easy;
    }

    private void OnDifficultyToggled(int difficultyIndex, bool isOn)
    {
        if (!isOn) return;

        SelectedDifficulty = (Difficulty)difficultyIndex; 
    }

    private void OnPlayClicked()
    {
        // TODO:: Play button click
        SceneManager.LoadScene(gameSceneName);
    }
}