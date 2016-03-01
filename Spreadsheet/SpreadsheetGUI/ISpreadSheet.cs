﻿using System;

namespace SpreadsheetGUI
{
    public interface ISpreadSheet
    {
        event Action<string> FileChosenEvent;

        event Action CloseEvent;

        event Action NewEvent;

        event Action<string> SaveEvent;

        event Action<string, int, int> ChangeContents;

        event Action<int, int> ChangeSelection;

        string Title { set; get; }

        void DoClose();

        void OpenNew();

        string Message { set; }

        string MessageYesNo { set; }

        string cellNameMainBox { set; }

        string cellValueMainBox { set; }

        string cellContentsMainBox { set; }

        void updateTable(string obj, int row, int col);


    }
}