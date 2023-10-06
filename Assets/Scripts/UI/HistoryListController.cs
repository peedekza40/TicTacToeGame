using System.Collections;
using System.Collections.Generic;
using Infrastructure.Entities;
using UnityEngine;

public class HistoryListController : MonoBehaviour
{
    public HistoryRowController TemplateRow;
    public Transform Container;

    public void DrawRow()
    {
        //clear all row
        foreach (Transform child in Container.transform) {
            Destroy(child.gameObject);
        }

        var connection = new DbContextBuilder().Connection;
        var histories = connection.Table<GameHistory>().OrderByDescending(x => x.ID).ToList();
        foreach(var history in histories)
        {
            var row = Instantiate(TemplateRow, Container);
            row.SetRow(history);
        }
    }
}
