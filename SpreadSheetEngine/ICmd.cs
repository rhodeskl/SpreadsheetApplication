//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    interface ICmd
    {
        ICmd Exec();
        Cell getCell();
        string getMessage();
        void setMessage(string newMessage);
        List<ICmd> getList();
    }
}
