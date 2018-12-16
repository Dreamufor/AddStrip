using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AddStrip
{
    /// <summary>
    /// This class is to handle the requirements for the calculation as a whole
    /// The class contains required methods 
    /// </summary>
    class Calculation
    {
        //The arraylist is to store the calcline Objects
        private ArrayList theCalcs = new ArrayList();

        //The listbox is to display calculationn objects
        private ListBox lstDisplay;
        /// <summary> method Calculation
        /// Constructor initializes the reference to the listbox and starts a new ArrayList.
        /// </summary>
        public Calculation(ListBox lb) {
            lstDisplay = lb;
        }
        /// <summary> method Add
        /// Add a CalcLine object to the ArrayList then redisplay the calculations.
        /// </summary>
        public void Add(CalcLine cl) {
            this.theCalcs.Add(cl);
            Redisplay();
        }
        /// <summary> method Clear`
        /// Clear the ArrayList and the listbox.
        /// </summary>
        public void Clear() {
            this.theCalcs.Clear();
            lstDisplay.Items.Clear();
            Redisplay();
        }
        /// <summary> method Redisplay
        /// Clear the listbox and then for each line in the calculation if the line is an ordinary calculation add the text version
        /// of the CalcLine to the listbox and calculate the result of the calculation so far.If the line is for a total or subtotal
        ///add the text for the total or subtotal to the listbox.If the line is for a total, the result of the calculation so far is
        ///reset to zero.
        /// </summary>
        public void Redisplay()
        {
            lstDisplay.Items.Clear();
            double resultSoFar = 0.0;
            foreach (CalcLine c in this.theCalcs)
            {
                resultSoFar = c.NextResult(resultSoFar);
                if (c.Op == Operator.subtotal)
                {
                    lstDisplay.Items.Add(c.ToString() + " " + resultSoFar.ToString() + " < subtotal");
                }
                else if (c.Op == Operator.total)
                {
                    lstDisplay.Items.Add(c.ToString() + " " + resultSoFar.ToString() + " << total");
                    resultSoFar = 0;
                }
                else
                {
                    lstDisplay.Items.Add(c.ToString());
                }
            }
        }
        /// <summary> method Find
        /// Return a reference to the nth CalcLine object in the ArrayList.
        /// </summary>
        public CalcLine Find(int n) {
            return this.theCalcs[n] as CalcLine;
        }
        /// <summary> method Replace
        /// Replace the nth CalcLine object in the ArrayList with the newCalc object, and then redisplay the calculations.
        /// </summary>
        public void Replace(CalcLine newCalc, int n) {
            this.theCalcs[n] = newCalc;
            Redisplay();
            lstDisplay.SelectedIndex = n;
        }
        /// <summary> method Insert
        /// Insert the newCalc CalcLine object in the ArrayList immediately before the nth object, and then redisplay the
        /// calculations.
        /// </summary>
        public void Insert(CalcLine newCalc, int n) {
            this.theCalcs.Insert(n, newCalc);
            Redisplay();
            lstDisplay.SelectedIndex = n;
        }
        /// <summary> method Delete
        /// Delete the nth CalcLine object in the ArrayList, and then redisplay the calculations.
        /// </summary>
        public void Delete(int n) {
            this.theCalcs.RemoveAt(n);
            Redisplay();
        }
        /// <summary> method SaveToFile
        /// Save all the CalcLine objects in the ArrayList as lines of text in the given file.
        /// </summary>
        public void SaveToFile(string filename) {     
            double resultSoFar = 0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
            foreach (CalcLine c in this.theCalcs)
            {
                String strText = "";
                resultSoFar = c.NextResult(resultSoFar);
                if (c.Op == Operator.subtotal)
                {
                    strText = "#";
                }
                else if (c.Op == Operator.total)
                {
                    strText = "=";
                }
                else
                {
                    strText = c.ToString();
                }
                    file.WriteLine(strText);
            }
        }
        /// <summary> method LoadFormFile
        /// Clear the ArrayList and then open the given file and convert the lines of the file to a set of CalcLine objects held
        /// in the ArrayList.Then redisplay the calculations.
        /// </summary>
        public void LoadFromFile(string filename) {
            Clear();
            string line;
            ArrayList lines = new ArrayList();//store all the data in the listbox
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while((line = file.ReadLine()) != null)
            {
                lstDisplay.Items.Add(line);
                lines.Add(line);
            }
            foreach(String newLine in lines)
            {
                Operator op;
                Double num = 0;
                char charOp = newLine[0];
                
                if (charOp != '=' && charOp != '#')
                {
                    if (newLine.Length > 2)
                    {
                        num = Convert.ToDouble(newLine.Substring(2));
                    }
                    else
                    {
                        throw new Exception("line `" + newLine + "` is wrong.");
                    }
                }
                switch (charOp)
                {
                    case '+':
                        op = Operator.plus;
                        break;
                    case '-':
                        op = Operator.minus;
                        break;
                    case '*':
                        op = Operator.times;
                        break;
                    case '/':
                        op = Operator.divide;
                        break;
                    case '=':
                        op = Operator.total;
                        num = 0;
                        break;
                    case '#':
                        op = Operator.subtotal;
                        num = 0;
                        break;                                                        
                    default:
                        op = Operator.error;
                        num = 0;
                        break;
                }
                Add(new CalcLine(op, num));
            }
            Redisplay();
            file.Close();
        }
    }
}
