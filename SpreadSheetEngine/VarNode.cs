//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    //node that stores variable in expression tree
    class VarNode: Node
    {
        private double value;

        public VarNode(string newContent, double newValue)
        {
            content = newContent;
            value = newValue;
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
