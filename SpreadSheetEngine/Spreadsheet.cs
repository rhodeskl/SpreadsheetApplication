//Kayla Rhodes, WSU ID: 11373485

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace SpreadSheetEngine
{
    public class Spreadsheet: Cell
    {
        private Cell [,] spreadsheet = new Cell[1,1];
        private int rowCount;
        private int columnCount;
        private int rowChanged; //used for events
        private int columnChanged; //used for events
        private ExpTree expTree = new ExpTree("0");
        public Dictionary<string, List<string>> dependentCells = new Dictionary<string, List<string>>();
        private Stack<ICmd> undo = new Stack<ICmd>(); //stack to store undo commands
        private Stack<ICmd> redo = new Stack<ICmd>(); //stack to store redo commands
        private string redoMessage;
        private HashSet<Cell> visited = new HashSet<Cell>(); //hashset to keep track of cells visted while evaluating a command - prevents circular references

        public Spreadsheet(int newRowIndex, int newColumnIndex, int placeholder) : base(newRowIndex, newColumnIndex)
        {
        }

        public Spreadsheet(int numRows, int numColumns):base(0,0) //spreadsheet constructor that takes number of rows and columns as arguments
        {
            spreadsheet = new Cell[numRows, numColumns]; //create new 2D array of cells with specified number of rows and columns

            rowCount = numRows; //set row count

            columnCount = numColumns; //set column count

            for(int i = 0; i < numColumns; i++) //loop through all columns
            {
                for(int j = 0; j < numRows; j++) //loop through all rows
                {
                    //spreadsheet[j, i] = new Cell(j,i);
                    spreadsheet[j, i] = new Spreadsheet(j,i,0);
                    //spreadsheet[j, i].setCell(j, i); //set row and column index
                    spreadsheet[j,i].PropertyChanged += CellPropertyChanged; //subscribe to cell property changed event
                }
            }
        }

        public string getUndoMessage()
        {
            if (undo.Count > 0)
            {
                return undo.Peek().getMessage();
            }
            return null;
        }

        public string getRedoMessage()
        {
            if(redo.Count > 0)
            {
                return redoMessage;
            }
            return null;
        }

        public int getUndoStackSize()
        {
            return undo.Count; //return number of items in undo stack
        }

        public int getRedoStackSize()
        {
            return redo.Count; //return number of items in redo stack
        }

        public void executeUndo() //execute the command on the top of the undo stack
        {
            redoMessage = getUndoMessage(); //get the redo message
            ICmd cmd = undo.Pop(); //pop the command to undo from the undo stack
            redo.Push(cmd.Exec()); //execute the undo command and add it to the redo stack
        }

        public void executeRedo() //execute the command on the top of the redo stack
        {
            ICmd cmd = redo.Pop(); //pop the command to redo from the redo stack
            undo.Push(cmd.Exec()); //execute the redo command and add it to the undo stack
            
        }

        public void pushToUndoBGColor(Cell cell) //push color change to undo stack
        {
            undo.Push(new RestoreBackgroundColor(cell,cell.getBGColor()));
        }

        public void pushToUndoText(Cell cell, string content) //push content change to undo stack
        {
            undo.Push(new RestoreText(cell, content));
        }

        public void pushToUndoBGColorGroup(List<Cell> list) //push group of color changes to undo stack
        {
            List<ICmd> cmds = new List<ICmd>();
            for (int i = 0; i < list.Count; i++) //loop through list of cells to create list of restore background color objects
            {
                RestoreBackgroundColor temp = new RestoreBackgroundColor(list[i], list[i].getBGColor());
                cmds.Add(temp);
            }
            MultiCmd mcmd = new MultiCmd(cmds); //create new multi command object
            mcmd.setMessage(" change in background color of cells");
            undo.Push(mcmd); //add multi command object to list
        }

        public Cell getCell(int row, int column)
        {
            if((row >= 0)&&(row < rowCount)&&(column >= 0)&&(column < columnCount)) //make sure row and column are within bounds of the table
            {
                return spreadsheet[row, column]; //return corresponding cell
            }
            else //requested cell doesn't exist
            {
                return null;
            }
        }

        public int getRowCount()
        {
            return rowCount;
        }

        public int getColumnCount()
        {
            return columnCount;
        }

        public int getRowChanged()
        {
            return rowChanged;
        }

        public int getColumnChanged()
        {
            return columnChanged;
        }

        private string getCellName(int rowIndex, int columnIndex) //get cell name
        {
            string c = ((char)(columnIndex + 65)).ToString(); //convert column index to capital letter using ascii code
            string num = (rowIndex + 1).ToString(); //convert row index to row number
            string name = c + num; //combine capital letter and row number into cell name
            return name;
        }

        private int getRowIndexByName(string name) //get row index of cell by name
        {
            int rowIndex = Convert.ToInt32(name.Substring(1)) - 1;
            return rowIndex;
        }

        private int getColumnIndexByName(string name) //get column index of cell by name
        {
            int columnIndex = (int)name[0] - 65;
            return columnIndex;
        }

        private Cell getCellByName(string name) //get cell by name in spreadsheet instead of column and row number
        {
            int length = name.Length;
            char c; 
            string r;
            c = name[0]; //get column letter
            r = name.Substring(1); //get row number
            int columnIndex = (int)c - 65; //convert column letter to ascii value and subtract 64 to get column number
            int rowIndex = Convert.ToInt32(r)-1; //convert row number to integer

            return getCell(rowIndex, columnIndex); //return cell corresponding to name
        }

        private bool checkCircularReference(string name) //check if a circular reference exists in expression
        {
            if (dependentCells.ContainsKey(name))
            {
                List<string> list = dependentCells[name]; //get list of dependent cells

                if (list.Count > 0) //the current cell has dependent cells
                {
                    foreach (string dependent in list) //loop through entire list
                    {
                        Cell c = getCellByName(dependent); //get dependent cell
                        if (visited.Contains(c)) //check if cell has already been visited
                        {
                            //visited.Clear(); //clear the visited hash set so it is ready for the next expression
                            return false; //cell has already been visited so there is a circular reference
                        }
                        else //cell has not been visited yet
                        {
                            visited.Add(c); //add the current dependent cell to the hash set
                            return checkCircularReference(dependent); //make recursive call
                        }
                    }
                }
            }

            //visited.Clear(); //clear the visited hash set so it is ready for the next expression
            return true; //there were no circular references in the expression
        }

        private void setPreValues(Cell current)
       {
            foreach(Cell c in visited)
            {
                if(c != current)
                {
                    c.setValue(c.getPreValue(), true);
                }
            }
       }

        private bool updateDependencies(string name) //update dependent cells of the cell that was just changed, returns false if there is a circular reference
        {
            if (dependentCells.ContainsKey(name))
            {
                List<string> list = dependentCells[name]; //get list of dependent cells

                if (list != null && list.Count > 0) //the current cell has dependent cells
                {
                    for(int i = 0; i < list.Count; i++)
                    {
                        string var = list[i]; //get cell name
                        Cell c = getCellByName(var); //get dependent cell using its name
                        //if (visited.Contains(c)) //check if the cell has already been visited
                        //{
                        //    return false;
                        //}
                        //else
                        //{
                        //    visited.Add(c); //add dependent cell to visited hash set
                            return updateCell(c, var); //update cell
                        //}
                    }
                }
            }

            return true; //no circular references are present
        }

        private void updateCellValue(Cell c, string name)
        {
            string content = c.getContent(); //get content of cell
            string preValue = c.getValue(); //get current value of cell
            c.setPreValue(value); //set previous value of cell to current value
            int rowIndex = getRowIndexByName(name); //get row index of cell
            int columnIndex = getColumnIndexByName(name); //get column index of cell
            List<string> list = new List<string>();
            if (content != null && content != "")
            {
                expTree = new ExpTree(content.Substring(1));
                list = expTree.parseVariables(content.Substring(1)); //get list of variables
                string n = getCellName(rowIndex, columnIndex); //get name of current cell
                //addDependencies(n, list); //add to dependencies dictionary
                string val;
                foreach (string var in list)
                {
                    val = getCellByName(var).getValue(); //get numerical value of cell
                    expTree.addToDictionary(var, val); //add variable name and value to expression tree dictionary
                }
                if (expTree.getDictionarySize() > 0) //variables exist in the tree
                {
                    expTree.resetTree(content.Substring(1)); //put variable values in tree
                }
                string value = expTree.Eval().ToString(); //get value of expression and convert it to string
                spreadsheet[rowIndex, columnIndex].setValue(value, true); //set value of cell to new value
            }
            else
            {
                spreadsheet[rowIndex, columnIndex].setValue("", true);
            }
            rowChanged = rowIndex; //save the row number that was changed
            columnChanged = columnIndex; //save the column number that was changed
            OnPropertyChanged("content"); //fire property changed event to update UI
        }

        private bool updateCell(Cell c, string name) //update dependent cell 
        {
            value = c.getValue(); //get current value of the cell
            if (value != "!(self reference)" && value != "!(circular reference)" && value != "!(bad reference)") //prevent crashing -> only update cell if it doesn't contain an error
            {
                updateCellValue(c, name);
                return updateDependencies(name); //update all of the current cell's dependencies
            }
            return true;
        }

        public void save(StreamWriter file) //save current spreadsheet to xml file
        {
            XmlDocument document = new XmlDocument(); //create new xml document object
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null); //put declaration at top of xml file
            XmlElement root = document.DocumentElement; 
            document.InsertBefore(xmlDeclaration, root);
            XmlElement element1 = document.CreateElement(string.Empty, "Spreadsheet", string.Empty); //add spreadsheet as root element
            document.AppendChild(element1);
            //loop through entire spreadsheet and save only the cells that the user has changed 
            for(int i = 0; i < 50; i++)
            {
                for(int j = 0; j < 26; j++)
                {
                    Cell temp = getCell(i, j); //get current cell
                    if (temp.getContent() != "" || temp.getBGColor() != 4294967295) //cell is not in its default state, need to save it to xml file
                    {
                        XmlElement element2 = document.CreateElement(string.Empty, "Cell", string.Empty); //create new cell tag
                        element1.AppendChild(element2);
                        XmlElement element3 = document.CreateElement(string.Empty, "RowIndex", string.Empty); //add row index tag
                        XmlText text1 = document.CreateTextNode(i.ToString());
                        element3.AppendChild(text1);
                        element2.AppendChild(element3);
                        XmlElement element4 = document.CreateElement(string.Empty, "ColumnIndex", string.Empty); //add column index tag
                        XmlText text2 = document.CreateTextNode(j.ToString());
                        element4.AppendChild(text2);
                        element2.AppendChild(element4);
                        XmlElement element5 = document.CreateElement(string.Empty, "Content", string.Empty); //add content tag
                        XmlText text3 = document.CreateTextNode(temp.getContent());
                        element5.AppendChild(text3);
                        element2.AppendChild(element5);
                        XmlElement element6 = document.CreateElement(string.Empty, "BGColor", string.Empty); //add bgcolor tag
                        XmlText text4 = document.CreateTextNode(temp.getBGColor().ToString());
                        element6.AppendChild(text4);
                        element2.AppendChild(element6); 
                    }
                }
            }

            document.Save(file); //save to StreamWriter object
        }

        public void load(StreamReader file) //load a xml file into spreadsheet
        {
            //reset cells to default values
            for(int i = 0; i < 50; i++)
            {
                for(int j = 0; j < 26; j++)
                {
                    Cell temp = getCell(i, j);
                    temp.setContent("");
                    temp.setBGColor(4294967295);
                }
            }
            XmlDocument document = new XmlDocument();
            document.Load(file); //load the file into the xml document object
            XmlNodeList nodes = document.DocumentElement.SelectNodes("/Spreadsheet/Cell"); //get the list of cells from the xml file
            int rowIndex, columnIndex;
            uint BGColor;
            string content;
            foreach(XmlNode node in nodes)
            {
                rowIndex = Convert.ToInt32(node.SelectSingleNode("RowIndex").InnerText); //get row index of cell
                columnIndex = Convert.ToInt32(node.SelectSingleNode("ColumnIndex").InnerText); //get column index of cell
                content = node.SelectSingleNode("Content").InnerText; //get content of cell
                BGColor = Convert.ToUInt32(node.SelectSingleNode("BGColor").InnerText); //get BGColor of cell
                Cell temp = getCell(rowIndex, columnIndex); //get cell corresponding to row and column index
                temp.setContent(content); //set the content of the cell to the content read from xml file
                temp.setBGColor(BGColor); //set the bgcolor of the cell to the bgcolor read from xml file
            }
            undo.Clear(); //clear undo stack
            redo.Clear(); //clear redo stack
        }

        private void addDependencies(string name, List<string> dependencies)
        {
            if (dependencies.Count > 0) //there are dependencies to add to the dictionary
            {
                foreach (string dep in dependencies)
                {
                    if (dependentCells.ContainsKey(dep)) //dependency is already in dictionary
                    {
                        if (dependentCells[dep].Contains(name) == false) //name isn't already in the list
                        {
                            dependentCells[dep].Add(name); //add dependency to list
                        }
                    }
                    else //dependency is not in dictionary
                    {
                        List<string> list = new List<string>();
                        list.Add(name); //add name to list
                        dependentCells.Add(dep,list); //add dependency and corresponding list to dictionary
                    }
                }
            }
        }

        private void removeDependencies(string name) //remove old dependencies when a cell's content changes
        {
            List<string> temp = new List<string>();
            foreach(KeyValuePair<string,List<string>> entry in dependentCells) //loop through entire dictionary
            {
                if(entry.Key != name) //only do something if current item is not the entry for the current cell
                {
                    temp = entry.Value; //get list of cells that reference current cell
                    if (temp != null && temp.Count > 0) //error prevention
                    {
                        temp.Remove(name); //remove current cell from list if it is in the list
                    }
                    /*if (temp.Count == 0) //if list is now empty
                    {
                        dependentCells.Remove(entry.Key); //remove key if corresponding dictionary is empty
                    }*/
                }
            }
        }

        private bool checkVariablesExist(List<string> list) //check if all variables exist in the spreadsheet
        {

            foreach(string variable in list) //loop through list to find variables that don't exist in spreadsheet
            {
                if (variable.Length > 1) //variable name must be at least two characters long
                {
                    char first = variable[0]; //get first letter of variable, must be capital letter in alphabet
                    if (first < 'A' || first > 'Z') //letter is not valid
                    {
                        return false; //variable does not exist in spreadsheet
                    }
                    char second = variable[1]; //get second character in variable, must be either 1,2,3,4, or 5
                    if (second < '1' || second > '5') //second character is not valid
                    {
                        return false; //variable does not exist in spreadsheet
                    }
                    int num = Convert.ToInt32(variable.Substring(1)); //get the number following the letter
                    if (num < 1 || num > 50) //num must be between 1 and 50 for it to be valid in the spreadsheet
                    {
                        return false; //variable does not exist in spreadsheet
                    }
                }
                else //variable name has only one character
                {
                    return false; //variable name must be more than one character long
                }
            }

            return true; //returns true if all variables exist in spreadsheet
        }

        private bool checkSelfReference(List<string> list, int rowIndex, int columnIndex) //check to see if cell references itself
        {
            string name = getCellName(rowIndex, columnIndex); //get name of current cell

            foreach(string variable in list) //check every variable in list
            {
                if(variable == name) //check to see if variable is same as the name of the cell that was changed
                {
                    return false; //self reference so return false
                }
            }

            return true; //return true if none of the variables in the list are a self reference
        }

        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e) //cell property changed event
        {
            int rowIndex = 0, columnIndex = 0;
            bool found = false;

            //determine which cell threw the event
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (sender.Equals(spreadsheet[i, j])) //check if current cell is the one that sent the event
                    {
                        found = true;
                        rowIndex = i; //save row index of sender
                        columnIndex = j; //save column index of sender
                        break; //break out of loop
                    }
                }
            }

            if ("content" == e.PropertyName)
            {
                if(found == true) //only proceed if sender was identified
                {
                    bool error = false;
                    string content = spreadsheet[rowIndex, columnIndex].getContent(); //get content string
                    if (content != null && content != "")
                    {
                        if (content.StartsWith("=")) //value needs to be updated
                        {
                            List<string> list = new List<string>();
                            expTree = new ExpTree(content.Substring(1));
                            list = expTree.parseVariables(content.Substring(1)); //get list of variables
                            if (checkVariablesExist(list)) //only execute following code if all variables exist in the spreadsheet
                            {
                                if (checkSelfReference(list, rowIndex, columnIndex)) //only execute following code if there aren't any self references in the formula
                                {
                                    visited.Add(getCell(rowIndex, columnIndex)); //add starting cell to visited hash set
                                    string n = getCellName(rowIndex, columnIndex); //get name of current cell
                                    removeDependencies(n); //remove old dependencies before adding new ones
                                    addDependencies(n, list); //add to dependencies dictionary
                                    if (checkCircularReference(getCellName(rowIndex, columnIndex))) //only execute following code if there aren't any circular references in the formula
                                    {
                                        string val;
                                        foreach (string var in list)
                                        {
                                            val = getCellByName(var).getValue(); //get numerical value of cell
                                            expTree.addToDictionary(var, val); //add variable name and value to expression tree dictionary
                                        }
                                        if (expTree.getDictionarySize() > 0) //variables exist in the tree
                                        {
                                            expTree.resetTree(content.Substring(1)); //put variable values in tree
                                        }
                                        string value = expTree.Eval().ToString(); //get value of expression and convert it to string
                                        spreadsheet[rowIndex, columnIndex].setValue(value, false); //set value of cell to new value
                                    }
                                    else //expression in cell contained a circular reference
                                    {
                                        spreadsheet[rowIndex, columnIndex].setValue("!(circular reference)", false); //set value of cell to error message
                                        error = true;
                                        //visited.Clear(); //clear visited hash set for the next expression
                                    }
                                }
                                else //expression in cell contained a self reference
                                {
                                    spreadsheet[rowIndex, columnIndex].setValue("!(self reference)", false); //set value of cell to error message
                                    error = true;
                                }
                            }
                            else //expression in cell contained an invalid cell name
                            {
                                spreadsheet[rowIndex, columnIndex].setValue("!(bad reference)", false); //set value of cell to error message
                                error = true;
                            }
                        }
                        else //value is the same as content
                        {
                            string n = getCellName(rowIndex, columnIndex); //get cell name
                            removeDependencies(n); //remove dependencies since cell is now just a constant value and doesn't reference any cells
                            spreadsheet[rowIndex, columnIndex].setValue(content, false); //set value of cell to content of cell
                        }
                    }
                    else
                    {
                        spreadsheet[rowIndex, columnIndex].setValue("", false);
                    }

                    bool success = false;
                    string name = getCellName(rowIndex, columnIndex); //get name of current cell
                    if (error == false) //only update dependencies if there is not an error
                    {
                        //visited.Add(getCellByName(name)); //add starting cell to visited hash set
                        success = updateDependencies(name); //update dependencies of current cell, returns false if there is a circular dependency
                        //if(success == false) //there was a circular dependency in the expression
                        //{
                        //    spreadsheet[rowIndex, columnIndex].setValue("!(circular reference)", false); //set value of cell to error message
                        //    setPreValues(getCellByName(name)); //make sure values of dependent cells don't change
                        //}
                    }
                    visited.Clear(); //clear hash set for next expression
                    rowChanged = rowIndex; //save the row number that was changed
                    columnChanged = columnIndex; //save the column number that was changed

                    OnPropertyChanged("content");//fire PropertyChanged event
                }
            }
            else if("value" == e.PropertyName)
            {
                rowChanged = rowIndex;
                columnChanged = columnIndex;
                OnPropertyChanged("content"); //fire PropertyChanged event
            }

            else if("BGColor" == e.PropertyName)
            {
                rowChanged = rowIndex;
                columnChanged = columnIndex;
                OnPropertyChanged("BGColor"); //fire PropertyChanged event
            }
        }
    }
}
