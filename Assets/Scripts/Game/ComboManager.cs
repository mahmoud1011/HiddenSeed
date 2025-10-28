using UnityEngine;
using System;

public class ComboManager : Singleton<ComboManager>
{
    [SerializeField] private float comboTimeout = 3f; // seconds before combo expires

    public int CurrentCombo { get; private set; }

    public event Action<int> OnComboUpdated;
    public event Action OnComboReset;

    float lastMatchTime;
    float currentTime;
    bool hasStartedCombo;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (hasStartedCombo && currentTime - lastMatchTime > comboTimeout)
            ResetCombo();
    }

    public void RegisterMatch()
    {
        if (!hasStartedCombo)
        {
            // First successful match — start combo tracking
            hasStartedCombo = true;
            lastMatchTime = currentTime;
            return;
        }

        // From the second match onward, increase combo
        CurrentCombo++;
        lastMatchTime = currentTime;

        OnComboUpdated?.Invoke(CurrentCombo);
    }

    public void ResetCombo()
    {
        if (!hasStartedCombo) return;

        hasStartedCombo = false;
        CurrentCombo = 0;
        OnComboReset?.Invoke();
    }
}