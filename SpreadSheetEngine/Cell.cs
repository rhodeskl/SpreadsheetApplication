//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SpreadSheetEngine
{
    public class Cell: INotifyPropertyChanged
    {
        private int rowIndex; //read only row index property
        private int columnIndex; //read only column index property
        protected string content; //text stored in the cell
        protected string value; //evaluated value of the cell
        private uint BGColor; //unsigned int to represent the background color of the cell
        public event PropertyChangedEventHandler PropertyChanged = delegate { }; //property changed event
        private string preValue; //previous evaluated value of the cell

        public Cell(int newRowIndex, int newColumnIndex)
        {
            rowIndex = newRowIndex;
            columnIndex = newColumnIndex;
            content = "";
            BGColor = 4294967295;
        }

        public int getRowIndex() //row index getter
        {
            return rowIndex;
        }

        public int getColumnIndex() //column index getter
        {
            return columnIndex;
        }

        public uint getBGColor() //background color getter
        {
            return BGColor;
        }

        public void setBGColor(uint newBGColor) //background color setter
        {
            BGColor = newBGColor;
            OnPropertyChanged("BGColor"); //fire PropertyChanged event
        }

        protected internal void setCell(int newRowIndex, int newColumnIndex) //setter for use by constructor
        {
            rowIndex = newRowIndex;
            columnIndex = newColumnIndex;
        }

        public string getContent() //content getter
        {
            return content;
        }

        public void setContent(string newContent) //content setter
        {
            //if(newContent != content) //content of cell needs to be changed
            //{
                content = newContent; //change the content of the cell
                OnPropertyChanged("content");//fire PropertyChanged event
            //}
        }

        public string getPreValue() //preValue getter
        {
            return preValue;
        }

        public void setPreValue(string newPreValue) //preValue setter
        {
            preValue = newPreValue;
        }

        public string getValue() //value getter
        {
            return value;
        }

        protected internal/*public*/ void setValue(string newValue, bool isUpdate) //set new value
        {
            preValue = value;
            value = newValue; //change the value of the cell
            if(isUpdate == true)
            {
                OnPropertyChanged("value");
            }
        }

        protected void OnPropertyChanged(string name) //property changed event handler
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
