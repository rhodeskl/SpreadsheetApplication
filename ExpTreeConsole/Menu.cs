//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadSheetEngine;

namespace ExpTreeConsole
{
    class Menu
    {
        private string option, expression = "A1 + B1 + C1";
        private ExpTree expTree = new ExpTree("A1 + B1 + C1");

        public Menu() //constructor
        {
            ExpTree expTree = new ExpTree("A1 + B1 + C1"); //set default expression tree in case use sets variables first, then tree
        }

        public void printMenu() //menu
        {
            bool done = false, accepted = true;
            string temp;
            double value;

            while (done == false)
            {
                do
                {
                    Console.WriteLine("Choose an option:");
                    Console.WriteLine("1. Enter an expression");
                    Console.WriteLine("2. Set a variable value");
                    Console.WriteLine("3. Evaluate the expression to a numerical value");
                    Console.WriteLine("4. Quit");
                    option = Console.ReadLine(); //get the user's choice 
                } while ((option != "1") && (option != "2") && (option != "3") && (option != "4"));

                if (option == "1") //enter an expression
                {
                    expTree.clearVar(); //clear old variables before starting a new expression
                    Console.WriteLine("Enter a expression with at most one kind of operator:");
                    expression = Console.ReadLine(); //read in the user's expression
                    expTree = new ExpTree(expression); //create a new expression tree with user entered expression
                }
                else if (option == "2") //set a variable value
                {
                    do
                    {
                        accepted = true;
                        Console.WriteLine("Enter a variable name:");
                        temp = Console.ReadLine(); //read in the variable name
                        if ((temp[0] < 'A') || (temp[0] > 'z'))
                        {
                            Console.WriteLine("The variable name must start with a letter");
                            accepted = false;
                        }
                    } while (accepted == false);
                    Console.WriteLine("Enter the variable value:");
                    value = Convert.ToDouble(Console.ReadLine()); //read in variable value as double
                    expTree.SetVar(temp, value);  //call set var function to add the name and value to variable dictionary
                }
                else if (option == "3") //evaluate an expression
                {
                    if(expTree.getDictionarySize() > 0) //there are variable values that need to be added to the tree
                    {
                        expTree.resetTree(expression);
                    }
                    value = expTree.Eval(); //call eval function
                    Console.WriteLine("The result of the evaluation was: ");
                    Console.WriteLine(value.ToString()); //print out result of eval function
                }
                else if (option == "4") //quit
                {
                    done = true;
                }
            }
        }
    }
}
