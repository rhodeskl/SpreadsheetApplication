//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    class RestoreText: ICmd //class to undo and redo cell text changes
    {
        private Cell cell;
        private string text;
        private string message;
        public RestoreText(Cell newCell, string newText)
        {
            cell = newCell;
            text = newText;
            message = " cell content change";
        }

        public ICmd Exec()
        {
            var inverse = new RestoreText(cell, cell.getContent()); //create inverse object
            cell.setContent(text); //restore previous content value to cell
            return inverse; //return inverse to add to the undo or redo stack
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
