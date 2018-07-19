//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    //node that stores constant in expression tree
    class ConstNode: Node
    {
        private double value;

        public ConstNode(string newContent)
        {
            content = newContent;
            value = Convert.ToDouble(content); //set value to numerical value of content 
        }

        public double getValue()
        {
            return value;
        }

        public void setValue(double newValue)
        {
            value = newValue;
        }

        public override double Eval()
        {
            return value;
        }
    }
}
