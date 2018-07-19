using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    class MultiCmd: ICmd
    {
        private List<ICmd> list = new List<ICmd>();
        private string message;

        public MultiCmd(List<ICmd> newList)
        {
            list = newList;
        }

        public ICmd Exec()
        {
            List<ICmd> invList = new List<ICmd>(); //list to store returned inverses
            for(int i = 0; i < list.Count; i++) //loop through entire list
            {
                invList.Add(list[i].Exec()); //call execute function for each element in the list and add returned inverse to inverse list
            }
            var inverse = new MultiCmd(invList); //create new inverse element
            return inverse; 
        }

        public void addColorToList(Cell cell, uint BGColor) //add restore background color object to list
        {
            list.Add(new RestoreBackgroundColor(cell,BGColor));
        }

        public Cell getCell()
        {
            return null;
        }

        public string getMessage()
        {
            return message;
        }

        public void setMessage(string newMessage)
        {
            message = newMessage;
        }

        public int getListSize()
        {
            return list.Count;
        }

        public List<ICmd> getList()
        {
            return list;
        }
    }
}
