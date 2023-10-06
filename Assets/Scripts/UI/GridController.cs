using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public FlexibleGridLayout FlexibleGridLayout;
    public CellController TemplateCell;

    public void DrawCell(Component sender, object data)
    {
        var size = (int)data;
        for(int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var cell = Instantiate(TemplateCell, transform);
                cell.SetRowColumn(i, j);
                cell.gameObject.SetActive(true);
            }
        }

        var spacingWidth = 24 - (0.25 * size * size);
        FlexibleGridLayout.spacing = new Vector2((float)spacingWidth, (float)spacingWidth);
    }
}
