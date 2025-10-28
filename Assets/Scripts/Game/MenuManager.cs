using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : Singleton<MenuManager>
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private ToggleGroup difficultyToggleGroup;
    [SerializeField] private Toggle[] difficultyToggles;
    [SerializeField] private string gameSceneName = "Game";

    [Header("Settings")]
    [SerializeField] private Difficulty selectedDifficulty = Difficulty.Easy;
    public enum Difficulty { Easy, Medium, Hard, Expert, Master }

    readonly Vector2Int[] difficultyGridSizes = new Vector2Int[]
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

        UpdateSelectedGridSize();
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
        Vector2Int previousGrid = GameStateDto.GetGridSize();

        // Check if the saved grid matches any of our difficulty presets
        for (int i = 0; i < difficultyGridSizes.Length; i++)
        {
            if (difficultyGridSizes[i] == previousGrid)
            {
                selectedDifficulty = (Difficulty)i;
                difficultyToggles[i].isOn = true;
                return;
            }
        }

        // If no match found, keep default selection
        difficultyToggles[(int)selectedDifficulty].isOn = true;
    }

    private void OnDifficultyToggled(int difficultyIndex, bool isOn)
    {
        if (isOn)
        {
            selectedDifficulty = (Difficulty)difficultyIndex;
            UpdateSelectedGridSize();
        }
    }

    private void UpdateSelectedGridSize()
    {
        Vector2Int grid = difficultyGridSizes[(int)selectedDifficulty];
        GameStateDto.SetGridSize(grid.x, grid.y);
    }

    private void OnPlayClicked()
    {
        // TODO:: Play button click
        SceneManager.LoadScene(gameSceneName);
    }
}