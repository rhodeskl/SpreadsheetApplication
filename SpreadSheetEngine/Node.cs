//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    //Node base class; each of the three types of nodes inherit from this class
    public abstract class Node
    {
        protected internal string content;

        public string getContent()
        {
            return content;
        }

        public void setContent(string newContent)
        {
            content = newContent;
        }

        public abstract double Eval(); //Eval function that needs to be overriden by all classes that inherit from this class
    }
}
