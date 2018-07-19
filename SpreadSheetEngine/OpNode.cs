//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    //node that stores operator in expression tree
    class OpNode: Node
    {
        private Node left;
        private Node right;

        public OpNode(char newContent, Node newLeft, Node newRight)
        {
            content = newContent.ToString();
            left = newLeft;
            right = newRight;
        }

        public Node getLeft()
        {
            return left;
        }

        public Node getRight()
        {
            return right;
        }

        public void setLeft(Node newLeft)
        {
            left = newLeft;
        }

        public void setRight(Node newRight)
        {
            right = newRight;
        }

        public override double Eval()
        {
            if(content == "+") //operand is +
            {
                return left.Eval() + right.Eval(); //add left and right nodes together
            }
            else if(content == "-") //operand is -
            {
                return left.Eval() - right.Eval(); //subtract right node from left node
            }
            else if(content == "*") //operand is *
            {
                return left.Eval() * right.Eval(); //multiply left and right nodes together
            }
            else if(content == "/") //operand is /
            {
                return left.Eval() / right.Eval(); //divide left node by right node
            }

            return 0;
        }
    }
}
