using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DVAH.Lib{
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

        float cellWidth = (parentWidth -  (spacing.x * (columns - 1)) - padding.left - padding.right) / (float)columns ;
        float cellHeight = (parentHeight - (spacing.y * (rows -1) - padding.top - padding.bottom) ) / (float)rows ;

        cellSize.x = FitX ? cellWidth : cellSize.x;
        cellSize.y = FitY ? cellHeight : cellSize.y;

        int columnCount = 0, rowCount = 0;
        bool isCenter = (int)childAlignment == 1 || (int)childAlignment == 4 || (int)childAlignment == 7;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;
            float center = 0;
            int leftOver = rectChildren.Count % columns;
           
            if(isCenter && (rowCount == rows - 1 && leftOver != 0)){
                center = (parentWidth - cellSize.x * leftOver - spacing.x * (columns - 1) - padding.left - padding.right) / 2.0f; 
            }

            var item = rectChildren[i];
            var xPos = center + cellSize.x * columnCount + spacing.x * columnCount + padding.left;
            
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
}
