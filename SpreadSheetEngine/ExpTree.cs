//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    public class ExpTree
    {
        private Node root;
        private Dictionary<string, double> variables;

        public ExpTree(string expression) //Implement this constructor to construct the tree from the specific expression
        {
            variables = new Dictionary<string, double>(); //allocate memory for variable list
            root = Compile(expression); //call compile function to build tree
        }

        public void resetTree(string expression)
        {
            root = Compile(expression); //call compile function to build tree
        }

        public void SetVar(string varName, double varValue) //Sets the specified variable within the ExpTree variables dictionary
        {
            variables.Add(varName, varValue);  //add the variable name and value to dictionary
        }

        public double Eval() //Implement this member function with no parameters that evaluates the expression's double value
        {
            if (root != null)
            {
                return root.Eval();
            }
            else
                return double.NaN;
        }

        private Node BuildSimple(string term)
        {
            double num;
            if(double.TryParse(term, out num))
            {
                return new ConstNode(num.ToString()); //term is a number
            }
            num = getVariableValue(term);
            return new VarNode(term, num); //term is a variable name
        }

        public int getDictionarySize()
        {
            return variables.Count;
        }

        public double getVariableValue(string term)
        {
            double num;
            variables.TryGetValue(term, out num); //get value associated with variable name
            return num;
        }

        public void addToDictionary(string name, string value)
        {
            if (value != "!(bad reference)" && value != "!(self reference)" && value != "!(circular reference)" && value != "") //only add to dictionary if value is a valid value
            {
                variables.Add(name, Convert.ToInt32(value));
            }
            else
            {
                if (value != "!(circular reference)")
                {
                    variables.Add(name, 0);
                }
            }
        }

        public List<string> parseVariables(string expression) //look through passed in expression and find all variable names
        {
            List<string> list = new List<string>();
            int i = 0;

            while(i < expression.Length)
            {
                char c = expression[i]; //get current character
                if(c != '(' && c != ')' && c != '+' && c != '-' && c != '*' && c != '/') //the current character is not a parentheses or operator so must be number or character
                {
                    if(c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9') //the current character is a number so need to skip it
                    {
                        i++;
                    }
                    else //the current character is the beginning of a variable name
                    {
                        string temp = ""; //temporary string to store variable name
                        while((c != '(') && (c != ')') && (c != '+') && (c != '-') && (c != '*') && (c != '/') && (i < expression.Length)) //loop until find an operator or end of the string
                        {
                            temp = temp + c; //add current character to temp string
                            i++;
                            if (i < expression.Length)
                            {
                                c = expression[i]; //get current character
                            }
                        }
                        list.Add(temp); //add temp to list of variable names
                    }
                }
                else //the current character is a parentheses or operator so need to skip it
                {
                    i++;
                }
            }

            return list;
        }

        private Node Compile(string exp)
        {
            //Find first operator
            //Build parent operator node
            //parent.left = BuildSimple(before op char)
            //parent.right = Compile(after op char)
            //return parent

            exp = exp.Replace(" ", ""); //remove white space
            //Check for being entirely enclosed in ()
            //If first char is '(' and last char is matching ')', remove parentheses
            if(exp[0] == '(') //check to see if first character is (
            {
                int counter = 1;  //initialize parentheses counter -> will be incremented when '(' is seen and decremented when ')' is seen
                for(int i = 1; i < exp.Length; i++) //loop through expression until find matching ')'
                {
                    if(exp[i] == ')') // ')' was found
                    {
                        counter--; //decrement parenthesis counter
                        if(counter == 0) //matching parenthesis was found
                        {
                            if(i == exp.Length - 1) //matching parenthesis is at the end of the expression
                            {
                                return Compile(exp.Substring(1, exp.Length - 2)); //call compile on expression without outside parentheses
                            }
                            else //matching parenthesis wasn't at the end of the expression
                            {
                                break;
                            }
                        }
                    }
                    if(exp[i] == '(') //found opening parentheses
                    {
                        counter++; //increment parentheses counter
                    }
                }
            }
            int index = GetLowOpIndex(exp); //get the index of the lowest priority operator
            if (index != -1) //operator was found
            {
                return new OpNode(exp[index], Compile(exp.Substring(0, index)), Compile(exp.Substring(index + 1))); //create new opNode with operator that was found
            }

            return BuildSimple(exp); //operator was not found, expression is number or variable name
        }

        private int GetLowOpIndex(string exp) //find the index of the lowest priority operator
        {
            int parenCounter = 0, index = -1;
            for(int i = exp.Length - 1; i >=0; i--) //loop through expression backwards
            {
                switch(exp[i])
                {
                    case ')':
                        parenCounter--;
                        break;
                    case '(':
                        parenCounter++;
                        break;
                    case '+':
                    case '-':
                        if(parenCounter == 0) //only return index of '+' or '-' if its not within a parentheses
                        {
                            return i;
                        }
                        break;
                    case '*':
                    case '/':
                        if((parenCounter == 0)&&(index == -1)) //return the '*' or '/' that is the farthest left in the expression and not within a parentheses
                        {
                            index = i;
                        }
                        break;
                }
            }
            return index; //only executes if expression doesn't have any operators in it -> the expression is a number or variable name
        }

        public void clearVar()
        {
            if (variables != null)
            {
                variables.Clear();
            }
        }
    }
}
