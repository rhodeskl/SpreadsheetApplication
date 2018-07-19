//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    class RestoreBackgroundColor: ICmd //class to undo and redo cell background color changes
    {
        private Cell cell;
        private uint BGColor;
        private string message;
        public RestoreBackgroundColor(Cell newCell, uint newBGColor)
        {
            cell = newCell;
            BGColor = newBGColor;
            message = " cell background color change";
        }
        public ICmd Exec()
        {
            var inverse = new RestoreBackgroundColor(cell, cell.getBGColor()); //create inverse object
            cell.setBGColor(BGColor); //restore the cell's previous background color
            return inverse; //return inverse object to add to the undo or redo stack
        }

        public string getMessage() //message getter
        {
            return message;
        }

        public Cell getCell()
        {
            return cell;
        }

        public void setMessage(string newMessage)
        {
            message = newMessage;
        }

        public List<ICmd> getList()
        {
            return null;
        }
    }
}
