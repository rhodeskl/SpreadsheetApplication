//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadSheetEngine;
using System.IO;
using System.Xml;

namespace Spreadsheet_KRhodes
{
    public partial class Form1 : Form
    {
        Spreadsheet spreadsheet = new Spreadsheet(50, 26); //create spreadsheet object
        public event DataGridViewCellCancelEventHandler CellBeginEdit; //cell begin edit event 
        //public event DataGridViewCellCancelEventHandler CellEndEdit; //cell end edit event

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 26; i++) //create 26 columns
            {
                dataGridView1.Columns.Add(Convert.ToChar(i + 65).ToString(), Convert.ToChar(i + 65).ToString()); //label columns with letters A-Z by adding 65 to index and then converting to character and then to string
            }
            dataGridView1.Rows.Add(50); //create 50 rows
            for(int i = 0; i < 50; i++) //loop through all 50 rows
            {
                var row = dataGridView1.Rows[i]; //get current row
                row.HeaderCell.Value = (i + 1).ToString(); //set row number
            }

            //initialize the value of all cells to their value in the data layer
            for(int i = 0; i < 26; i++)
            {
                for(int j = 0; j < 50; j++)
                {
                    dataGridView1[i, j].Value = spreadsheet.getCell(j, i).getValue(); //set current cell value to the value of the corresponding cell in the data layer
                }
            }

            button3.Enabled = false;
            button5.Enabled = false;

            spreadsheet.PropertyChanged += SpreadsheetPropertyChanged; //subscribe to spreadsheet property changed event
            //subscribe to UI change events
            //dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
        }

        //private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)//event for when cell is being edited
        //{
        //    spreadsheet.setValue(spreadsheet.getCell(e.RowIndex, e.ColumnIndex).getContent(), true);
        //}

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e) //event for when the cell is finished being edited
        {
            dataGridView1.EndEdit();
            spreadsheet.pushToUndoText(spreadsheet.getCell(e.RowIndex, e.ColumnIndex), spreadsheet.getCell(e.RowIndex, e.ColumnIndex).getContent());
            button3.Enabled = true;
            spreadsheet.getCell(e.RowIndex, e.ColumnIndex).setContent(dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString());
        }

        private void SpreadsheetPropertyChanged(object sender, PropertyChangedEventArgs e) //spreadsheet property changed event, needs to update UI
        {
            //if(spreadsheet.getUndoStackSize() == 0)
            //    {
            //    button3.Visible = false;
            //}
            //    else
            //    {
            //    button3.Visible = true;
            //}
            //if (spreadsheet.getRedoStackSize() == 0)
            //{
            //    button5.Visible = false;
            //}
            //else
            //{
            //    button5.Visible = true;
            //}
            if ("content" == e.PropertyName) //the value of the cell was changed
            {
                int rowIndex = spreadsheet.getRowChanged(); //get the row index of the cell that was changed
                int columnIndex = spreadsheet.getColumnChanged(); //get the column index of the cell that was changed
                string value = spreadsheet.getCell(rowIndex, columnIndex).getValue(); //get value of cell that was changed

                button3.Text = "Undo" + spreadsheet.getUndoMessage(); //update undo button text
                //button5.Text = "Redo" + spreadsheet.getRedoMessage(); //update redo button text 

                dataGridView1[columnIndex, rowIndex].Value = value; //set new value of cell corresponding to the one that was changed in the spreadsheet
            }
            else if("BGColor" == e.PropertyName) //the background color of the cell was changed
            {
                int rowIndex = spreadsheet.getRowChanged(); //get the row index of the cell that was changed
                int columnIndex = spreadsheet.getColumnChanged();
                uint BGColor = spreadsheet.getCell(rowIndex, columnIndex).getBGColor(); //get the background color of the cell that was changed

                button3.Text = "Undo" + spreadsheet.getUndoMessage(); //update undo button text
                //button5.Text = "Redo" + spreadsheet.getRedoMessage(); //update redo button text
                var newCellStyle = new DataGridViewCellStyle(); //create new cell style object
                Color newColor = convertUintToColor(BGColor); //get color corresponding to uint
                newCellStyle.BackColor = newColor; //set background color of new style object to new color
                newCellStyle.SelectionBackColor = newColor; //set selection background color of new style object to new color
                dataGridView1[columnIndex, rowIndex].Style = newCellStyle; //set style of current cell to new style object
            }
        }

        private void button1_Click(object sender, EventArgs e) //demo button was clicked
        {
            for(int count = 1; count <= 50; count++) //set value in 50 random cells to Hello World!
            {
                bool success = false;
                do
                {
                    Random rnd = new Random();
                    int rowIndex = rnd.Next(0, 50); // 0 <= rowIndex < 50
                    int columnIndex = rnd.Next(0, 26);  //0 <= columnIndex < 26

                    //dataGridView1[columnIndex, rowIndex].Value = "Hello World!"; //set random cell to Hello World!
                    if (spreadsheet.getCell(rowIndex, columnIndex).getContent() != "Hello World!")
                    {
                        spreadsheet.getCell(rowIndex, columnIndex).setContent("Hello World!"); //set random cell to Hello World!
                        success = true;
                    }
                } while (success == false);
            }

            for(int row = 0; row < 50; row++) //loop through all B cells
            {
                //dataGridView1[1, row].Value = string.Concat("This is cell B", (row+1).ToString()); //set all cells in B column to say this is cell B#, # = row number
                spreadsheet.getCell(row, 1).setContent(string.Concat("This is cell B", (row + 1).ToString())); //set all cells in B column to say this is cell B#, # = row number
            }

            for(int row = 0; row < 50; row++) //loop through all A cells
            {
                //dataGridView1[0, row].Value = string.Concat("=B", (row+1).ToString()); //set all cells in A column to say =B#, # = row number
                spreadsheet.getCell(row, 0).setContent(string.Concat("=B", (row + 1).ToString())); //set all cells in A column to say =B#, # = row number
            }
        }

        private uint convertColorToUint(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }

        private Color convertUintToColor(uint BGColor)
        {
            byte a = (byte)(BGColor >> 24); //get alpha value
            byte r = (byte)(BGColor >> 16); //get red value
            byte g = (byte)(BGColor >> 8); //get green value
            byte b = (byte)(BGColor >> 0); //get blue value
            return Color.FromArgb(a, r, g, b); //return color corresponding to the four byte values
        }

        private void button2_Click(object sender, EventArgs e) //change background color of selected cells button was clicked
        {
            ColorDialog MyDialog = new ColorDialog(); //instantiate new color dialog object
            MyDialog.AllowFullOpen = false; //allows the user get help
            MyDialog.ShowHelp = true; //sets the initial color select to the current text color
            //update the cell color if the user clicks OK
            if(MyDialog.ShowDialog() == DialogResult.OK)
            {
                uint BGColor = convertColorToUint(MyDialog.Color); //get the uint equivalent of the color the user selected
                int selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected); //get the number of cells current selected
                if(selectedCellCount > 0) //there is at least one cell selected
                {
                    if (selectedCellCount == 1)
                    {
                        int rowIndex = dataGridView1.SelectedCells[0].RowIndex; //get row index of selected cell
                        int columnIndex = dataGridView1.SelectedCells[0].ColumnIndex; //get column index of selected cell
                        spreadsheet.pushToUndoBGColor(spreadsheet.getCell(rowIndex, columnIndex)); //push cell change to undo stack
                        spreadsheet.getCell(rowIndex, columnIndex).setBGColor(BGColor); //set color of selected cell

                    }
                    else
                    {
                        List<Cell> list = new List<Cell>();
                        for (int i = 0; i < selectedCellCount; i++) //loop through selected cells and add to list of cells
                        {
                            int rowIndex = dataGridView1.SelectedCells[i].RowIndex; //get row index of selected cell
                            int columnIndex = dataGridView1.SelectedCells[i].ColumnIndex; //get column index of selected cell
                            list.Add(spreadsheet.getCell(rowIndex, columnIndex)); //add selected cell to list
                        }
                        spreadsheet.pushToUndoBGColorGroup(list); //add cells with current background color before changing to undo stack
                        for(int i = 0; i < list.Count; i++) //loop through list of cells and change their background color to the new background color
                        {
                            list[i].setBGColor(BGColor); //set the background color of the current cell
                        }
                    }
                }

            }
        }

        private void button5_Click(object sender, EventArgs e) //redo button was clicked
        {
            if(spreadsheet.getRedoStackSize() > 0) //only execute redo if there is at least one item on the redo stack
            {
                button3.Enabled = true; //enable undo button
                spreadsheet.executeRedo(); //execute the redo command on top of the redo stack
                if(spreadsheet.getRedoStackSize() == 0) //disable redo button if there isn't anything on the redo stack
                {
                    button5.Enabled = false;
                }
                button3.Text = "Undo" + spreadsheet.getUndoMessage(); //update undo button text
                button5.Text = "Redo" + spreadsheet.getRedoMessage(); //update redo button text
            }
        }

        private void button3_Click(object sender, EventArgs e) //undo button was clicked
        {
            if(spreadsheet.getUndoStackSize() > 0) //only execute undo if there is at least one item on the undo stack
            {
                button5.Enabled = true; //enable the redo button
                spreadsheet.executeUndo(); //execute the undo command on top of the undo stack
                if(spreadsheet.getUndoStackSize() == 0)//disable the undo button if there isn't anything on the undo stack
                {
                    button3.Enabled = false;
                }
                button3.Text = "Undo" + spreadsheet.getUndoMessage(); //update undo button text
                button5.Text = "Redo" + spreadsheet.getRedoMessage(); //update redo button text
                
            }
        }

        private void button4_Click(object sender, EventArgs e) //save button was clicked
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK) //show dialog to save a file, only proceed if user clicks the save button
            {
                if(saveFileDialog1.FileName != "") //check to make sure that file name is not empty string
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile())) //create new stream writer with user selected file
                    {
                        spreadsheet.save(sw); //call spreadsheet save function
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e) //load button was clicked
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK) //show dialog to open file, only proceed if user clicks the open button
            {
                if(openFileDialog1.FileName != "") //check to make sure that file name is not empty string
                {
                    using (StreamReader sw = new StreamReader(openFileDialog1.OpenFile())) //create new stream reader with user selected file
                    {
                        spreadsheet.load(sw); //call spreadsheet load function
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) //change value of cell to the corresponding cell content in the data layer when the cell is clicked on
        {
            string content = spreadsheet.getCell(e.RowIndex, e.ColumnIndex).getContent();
            if (content != null)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = content;
            }
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e) //change value of cell back to the corresponding cell value in the data layer after the cell is deselected
        {
            string value = spreadsheet.getCell(e.RowIndex, e.ColumnIndex).getValue();
            if (value != null)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
            }
        }
    }
}
