﻿using System;
using SSGui;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form, ISpreadSheet
    {
        
        
        public string Title
        {
            set
            {
                Text = value; 
            }

            get
            {
                return Text;
            }
        }

        
        public string Message
        {
            set
            {
                MessageBox.Show(value);
            }
        }

        public string MessageYesNo
        {
            set
            {
                if (MessageBox.Show("Do you want to save the current file?", value, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DialogResult result = saveFileDialog1.ShowDialog();
                    if (result == DialogResult.Yes || result == DialogResult.OK)
                    {
                        if (SaveEvent != null)
                        {
                            SaveEvent(saveFileDialog1.FileName);
                        }
                    }
                }
            }
            
        }

        public string cellNameMainBox
        {
            set
            {
                cellNameBox.Text = value;
            }
        }

        public string cellValueMainBox
        {
            set
            {
                CellValueBox.Text = value;
            }
        }

        public string cellContentsMainBox
        {
            set
            {
                cellContentsBox.Text = value;
            }

            get
            {
                return cellContentsBox.Text;
            }
        }

        public Form1()
        {
            InitializeComponent();
            spreadsheetPanel1.SelectionChanged += displaySelection;

        }

        

        public event Action<string> FileChosenEvent;
        public event Action CloseEvent;
        public event Action NewEvent;
        public event Action<string> SaveEvent;
        public event Action<string, int, int> ChangeContents;
        public event Action<int, int> ChangeSelection;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (ChangeSelection != null)
            {
                ChangeSelection(0, 0);
            }
        }

        public void DoClose()
        {
            Close();
        }


        public void OpenNew()
        {
            SpreadSheetApplicationContext.GetContext().RunNew();
        }

        private void openItem_Click_1(object sender, EventArgs e)
        {
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (FileChosenEvent != null)
                {
                    FileChosenEvent(fileDialog.FileName);
                }
            }
            if (ChangeSelection != null)
            {
                ChangeSelection(0, 0);
            }
        }

        private void newItem_Click_1(object sender, EventArgs e)
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        private void closeItem_Click_1(object sender, EventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (SaveEvent != null)
                {
                    SaveEvent(saveFileDialog1.FileName);
                }
            }
        }


        private void cellContentsBox_TextChanged(object sender, KeyEventArgs e)
        {
            int column;
            int row;


            if (e.KeyCode == Keys.Enter)
            {
                if (ChangeContents != null)
                {
                    spreadsheetPanel1.GetSelection(out column, out row);
                    ChangeContents(cellContentsBox.Text, column, row);
                }
            }
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            ss.GetSelection(out col, out row);
            

            if(ChangeSelection != null)
            {
                ChangeSelection(col, row);
            }
        }

        public void updateTable(string obj, int col, int row)
        {
            spreadsheetPanel1.SetValue(col, row, obj);
        }
    }
}
