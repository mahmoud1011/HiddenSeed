using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class GridController : MonoBehaviour
{
    [SerializeField] GridLayoutGroup gridLayout;
    [SerializeField, Range(0f, 0.2f)] float outerPaddingPercent = 0.05f;
    [SerializeField, Range(0f, 2f)] float spacingRatio = 0.05f;

    public void SetupGrid(int cols, int rows)
    {
        var rect = (transform as RectTransform).rect;

        // Calculate available space
        float outerPadX = rect.width * outerPaddingPercent * 2f;
        float outerPadY = rect.height * outerPaddingPercent * 2f;
        float internalPadX = gridLayout.padding.left + gridLayout.padding.right;
        float internalPadY = gridLayout.padding.top + gridLayout.padding.bottom;

        float availableWidth = Mathf.Max(0f, rect.width - outerPadX - internalPadX);
        float availableHeight = Mathf.Max(0f, rect.height - outerPadY - internalPadY);

        // Calculate cell size
        float cellFromWidth = availableWidth / (rows + (rows - 1) * spacingRatio);
        float cellFromHeight = availableHeight / (cols + (cols - 1) * spacingRatio);
        float cellSize = Mathf.Floor(Mathf.Min(cellFromWidth, cellFromHeight));
        float spacing = spacingRatio * cellSize;

        // Apply layout
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = rows;
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
        gridLayout.spacing = new Vector2(spacing, spacing);

        // Center grid
        float usedWidth = cellSize * rows + spacing * (rows - 1);
        float usedHeight = cellSize * cols + spacing * (cols - 1);

        var padding = gridLayout.padding;
        padding.left += Mathf.RoundToInt(Mathf.Max(0f, rect.width - internalPadX - usedWidth) * 0.5f);
        padding.right = padding.left;
        padding.top += Mathf.RoundToInt(Mathf.Max(0f, rect.height - internalPadY - usedHeight) * 0.5f);
        padding.bottom = padding.top;
        gridLayout.padding = padding;

        ClearGrid();
    }

    public void Populate(List<CardModel> cards)
    {
        foreach (var card in cards)
            card.transform.SetParent(transform, false);
    }

    private void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}