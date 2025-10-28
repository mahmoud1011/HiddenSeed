using TMPro;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] TextMeshProUGUI scoreText, turnsText;

    public int Score { get; private set; }
    public int Turns { get; private set; }

    #region Public 

    public void IncrementScore()
    {
        Score += 1;
        UpdateScoreUI();
    }

    public void IncrementTurn()
    {
        Turns += 1;
        UpdateTurnsUI();
    }

    public void Clear()
    {
        Score = 0;
        Turns = 0;
        UpdateUI();
    }

    #endregion

    #region Private 

    private void UpdateScoreUI() => scoreText.text = $"Matches {Score}";
    private void UpdateTurnsUI() => turnsText.text = $"Turns {Turns}";
    private void UpdateUI()
    {
        UpdateScoreUI();
        UpdateTurnsUI();
    }

    #endregion 

}