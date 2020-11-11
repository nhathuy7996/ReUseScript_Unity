using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FelexiableLayoutGroup : LayoutGroup
{
    public FitType fitType;
    public int rows, columns;
    public Vector2 cellSize, spacing;
    public bool FitX, FitY;

    public override void CalculateLayoutInputVertical()
    {
      
        if(fitType == FitType.Width || fitType == FitType.Heigh || fitType == FitType.Uniform)
        {
            FitX = true;
            FitY = true;
            float sqrRt = Mathf.Sqrt(this.transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);            
        }

        if (fitType == FitType.Width || fitType == FitType.FixedColum)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }

        if (fitType == FitType.Heigh || fitType == FitType.FixedRow)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }



        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / (float)columns  - ((spacing.x / (float)columns) * 2) -  (padding.left / (float) columns) - (padding.right / (float) columns);
        float cellHeight = parentHeight / (float)rows - ((spacing.y / (float)rows) * 2) - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize.x = FitX ? cellWidth : cellSize.x;
        cellSize.y = FitY ? cellHeight : cellSize.y;

        int columnCount = 0, rowCount = 0;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];
            var xPos = cellSize.x * columnCount + spacing.x * columnCount + padding.left;
            var yPos = cellSize.y * rowCount + spacing.y * rowCount + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void SetLayoutHorizontal()
    {


        
    }

    public override void SetLayoutVertical()
    {

    }

    public enum FitType
    {
        Uniform,
        Width,
        Heigh,
        FixedRow,
        FixedColum
    }

}
