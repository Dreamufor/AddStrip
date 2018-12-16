using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AddStrip
{
    /// <summary>
    ///  Main user form for the AddStrip application
    /// </summary>
    public partial class AddStripForm : Form
    {
        private Calculation calculation;

        //keep track of a file to know saved or not
        private bool isSaved = false;

        private string fileName;
        /// <summary>
        /// Constructor
        /// </summary>
        public AddStripForm()
        {
            InitializeComponent();
            calculation = new Calculation(lstOutoput);
        }
        /// <summary>
        /// Exit the application
        /// if the file haven't saved yet ,ask user.
        /// if saved ,then exit.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(isSaved == true && MessageBox.Show("Dou you want exit now?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Close();
            }
            else if (isSaved == true && MessageBox.Show("Dou you want exit now?", "Exit", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            else
            {
                if(isSaved == false && lstOutoput.Items.Count != 0 && MessageBox.Show("Dou you want save the current data?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    saveAsToolStripMenuItem_Click(sender,e);
                    Close();
                }
                else
                {
                    Close();
                }
            }
        }
        /// <summary>
        /// Exit the application
        /// if the file haven't saved yet ,ask user.
        /// if saved ,then exit.
        /// </summary>
        private void AddStripForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           if(isSaved == true && MessageBox.Show("Dou you want exit now?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else if (isSaved == true && MessageBox.Show("Dou you want exit now?", "Exit", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                if (isSaved == false && lstOutoput.Items.Count != 0 && MessageBox.Show("Dou you want save the current data?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    saveAsToolStripMenuItem_Click(sender, e);
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = false;
                }
            }
        }
        /// <summary>
        /// diaplay a selected item in text box
        /// if contains "#" of "=", only display the operator
        /// </summary>
        private void lstOutoput_SelectedIndexChanged(object sender, EventArgs e)
        {
            string select = this.lstOutoput.SelectedItem.ToString();
            if (this.lstOutoput.SelectedItem != null)
            {
                if (this.lstOutoput.SelectedItem.ToString().Contains("#")|| this.lstOutoput.SelectedItem.ToString().Contains("="))
                {                   
                    txtSelect.Text = select.Substring(0, 1);
                }
                else
                {
                    txtSelect.Text = select.Replace(" ", "");
                }
            }
        }
        /// <summary>
        /// delete a selected item in listbox, give warning before deleting.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int selected = lstOutoput.SelectedIndex;
            if (txtSelect.Text != null)
            {
                CalcLine calcline = new CalcLine(this.txtSelect.Text);
                if (MessageBox.Show("Do you want to delete this item?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    calculation.Delete(selected);
                    txtSelect.Text = "";
                }
                else return;
            }
        }
        /// <summary>
        /// Updata a selected item in listbox
        /// check the reasonablility of the input data,if rules are violated, warn the user.
        /// </summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int selected = lstOutoput.SelectedIndex;
            if (txtSelect.Text.Length > 0)
            {
                if (txtSelect.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a valid data", "Error");
                }
                else if (txtSelect.Text.Length == 1)
                {
                    if(txtSelect.Text.Contains("#")|| txtSelect.Text.Contains("="))
                    {
                        CalcLine calcline = new CalcLine(this.txtSelect.Text);
                        calculation.Replace(calcline, selected);
                        txtSelect.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Incorrect data format", "Invalid Data Format");
                    }
                }
                else
                {
                    char ch = txtSelect.Text[0];//first character in the text box 
                    char ch1 = txtSelect.Text[1];//second character,numeric
                    char ch2 = txtSelect.Text[txtSelect.Text.Length - 1];//last charac, +-*/=#
                    if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                    {
                        if(((ch1 >= (char)48) && (ch1 <= (char)57)) && ((ch2 >= (char)48) && (ch2 <= (char)57)))
                        {
                            CalcLine calcline = new CalcLine(this.txtSelect.Text);
                            calculation.Replace(calcline, selected);
                            txtSelect.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect data format", "Invalid Data Format");
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Please enter valid value.");
            }

        }
        /// <summary>
        /// Insert a selected calculation line.
        /// Update according to the calculation rules
        /// If rules are violated, warn the user.
        /// </summary>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            int selected = lstOutoput.SelectedIndex;
            if(selected == 0)//if the insert line will be the first line
            {
                if(txtSelect.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a valid data","Error");
                }
               else if (txtSelect.Text.Length == 1)
                {
                    if (txtSelect.Text.Contains("#") || txtSelect.Text.Contains("="))
                    {
                        CalcLine calcline = new CalcLine(this.txtSelect.Text);
                        calculation.Insert(calcline, selected);
                        txtSelect.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Incorrect data format", "Invalid Data Format");
                    }
                }
                else
                {
                    char ch = txtSelect.Text[0];
                    char ch1 = txtSelect.Text[1];
                    char ch2 = txtSelect.Text[txtSelect.Text.Length - 1];
                    if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                    {
                        if (((ch1 >= (char)48) && (ch1 <= (char)57)) && ((ch2 >= (char)48) && (ch2 <= (char)57)))
                        {
                            CalcLine calcline = new CalcLine(this.txtSelect.Text);
                            calculation.Insert(calcline, selected);
                            txtSelect.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect data format", "Invalid Data Format");
                    }
                }
            }
            else
            {
                if (lstOutoput.Items[selected - 1].ToString().Contains("="))//decide if the item before selected item is total,then obry the rule of first charac.
                {
                    if (txtSelect.Text.Length == 0)
                    {
                        MessageBox.Show("Please enter a valid data", "Error");
                    }
                    else if (txtSelect.Text.Length == 1)
                    {
                        if (txtSelect.Text[0] != '#' || txtSelect.Text[0] != '=')
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                        }
                        else
                        {
                            CalcLine calcline = new CalcLine(this.txtSelect.Text);
                            calculation.Insert(calcline, selected);
                            txtSelect.Text = "";
                        }
                    }
                    else
                    {
                        if (!(txtSelect.Text[0] == '+' || txtSelect.Text[0] == '-'))
                        {
                            MessageBox.Show("First data entry must be either \'+\' or \'-\' as the beginning operator", "Invalid First Data Entry");
                            txtEnter.Text = "";
                        }
                        else
                        {
                            char ch1 = txtSelect.Text[1];
                            char ch2 = txtSelect.Text[txtSelect.Text.Length - 1];
                            if (((ch1 >= (char)48) && (ch1 <= (char)57)) && ((ch2 >= (char)48) && (ch2 <= (char)57)))
                            {
                                CalcLine calcline = new CalcLine(this.txtSelect.Text);
                                calculation.Insert(calcline, selected);
                                txtSelect.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Incorrect data format", "Invalid Data Format");
                            }

                        }
                    }
                }
                else//decide if input resonable
                {
                    if (txtSelect.Text.Length == 0)
                    {
                        MessageBox.Show("Please enter a valid data", "Error");
                    }
                    else if(txtSelect.Text.Length == 1)
                    {
                        if(txtSelect.Text[0] == '#' || txtSelect.Text[0] == '+')
                        {
                            CalcLine calcline = new CalcLine(this.txtSelect.Text);
                            calculation.Insert(calcline, selected);
                            txtSelect.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                            txtSelect.Text = "";
                        }
                    }                                
                    else if (txtSelect.Text[0] == '+' || txtSelect.Text[0] == '-' || txtSelect.Text[0] == '*' || txtSelect.Text[0] == '/')
                    {
                        char ch = txtSelect.Text[0];
                        char ch1 = txtSelect.Text[1];
                        char ch2 = txtSelect.Text[txtSelect.Text.Length - 1];
                        if (((ch1 >= (char)48) && (ch1 <= (char)57)) && ((ch2 >= (char)48) && (ch2 <= (char)57)))
                        {
                            CalcLine calcline = new CalcLine(this.txtSelect.Text);
                            calculation.Insert(calcline, selected);
                            txtSelect.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect data format", "Invalid Data Format");
                    }
                }
            }


        }
        /// <summary>
        /// Request to save or not before create a new file
        /// Clear all the object in the form
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isSaved == false && lstOutoput.Items.Count != 0 && MessageBox.Show("Dou you want save the current data?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveAsToolStripMenuItem_Click(sender, e);//back to savefiledialog for user to save the file 
                calculation.Clear();
                lstOutoput.Items.Clear();
                txtEnter.Clear();
                txtSelect.Clear();
                isSaved = false;
            }
            else
            {
                calculation.Clear();
                lstOutoput.Items.Clear();
                txtEnter.Clear();
                txtSelect.Clear();
                isSaved = false;
            }         
        }
        /// <summary>
        /// Open a cal file and load the calculations.
        /// Request to save the current data first
        /// Give message to user if cannot load the file
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isSaved == false && lstOutoput.Items.Count > 0 && MessageBox.Show("Dou you want save the current data?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveAsToolStripMenuItem_Click(sender, e);//save the current data before opening a new file
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open File";
            ofd.Filter = "Cal Files(*.cal)|*.cal";
            ofd.InitialDirectory = @"C:\Temp\";
            if (ofd.ShowDialog() == DialogResult.OK)
            {                   
                    calculation.LoadFromFile(ofd.FileName);
                    isSaved = true;
            }
            
        }
        /// <summary>
        /// if no data in the listbox, give message to user
        /// if has saved before,update the file
        /// if hasn't saved,ave as a new file
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstOutoput.Items.Count == 0)
            {
                MessageBox.Show("Please have valid data in the list box before saving","Error");
            }
            else if(isSaved == true)//update a saved file
            {
                calculation.SaveToFile(fileName);
                MessageBox.Show("Your changes have been saved!");
            }
            else if(isSaved == false)//save a new file.
            {
                saveAsToolStripMenuItem_Click(sender, e);
                isSaved = true;
            }
        }
        /// <summary>
        /// save as a new file wth valid filename
        /// </summary>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save File";
            sfd.Filter = "Cal Files(*.cal)|*.cal";
            sfd.FileName = "Calculation1";
            sfd.InitialDirectory = @"C:\Temp\";     
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileName;
                calculation.SaveToFile(fileName);
                isSaved = true;
            }
            else
            {
                isSaved = false;
                return;
            }
        }
        /// <summary>
        /// If enter is pressed and the add calculation textbox is empty,
        /// send notice to user to enter a calculation.
        /// </summary>
        private void txtEnter_KeyUp(object sender, KeyEventArgs e)
        {
            string txt = txtEnter.Text;
            if (lstOutoput.Items.Count == 0)
            {
                if (txt.Length == 0)//enter key is not allowed if no calculation have begun
                {
                    if(e.KeyCode == Keys.Enter)
                    {
                        MessageBox.Show("No calculations have begun,Enter key is not allowed", "Invalid Request");
                    }
                    else
                    {
                        return;
                    }
                    
                }
                else if (txt.Length == 1)
                {
                    if (txt[0].ToString() == "#" || txt[0].ToString() == "=")//no total or subtotal for the first line.
                    {
                        MessageBox.Show("No calculation available. \n So no subtotal or total can be displayed", "Invalid Request");
                        txtEnter.Text = "";
                    }
                    else if (!(txt[0].ToString() == "+" || txt[0].ToString() == "-"))//first character must be "+" or "-" for the first line
                    {
                        MessageBox.Show("First data entry must be either \'+\' or \'-\' as the beginning operator", "Invalid First Data Entry");
                        txtEnter.Text = "";
                    }
                }
                else
                { 
                    if(txt[1]>= (char)48 && txt[1] <= (char)57)//numeric after an operator
                    {
                        //to decide if last one is a valid operator ,then add the front data to listbox 
                        char ch = txt[txt.Length - 1]; 
                        if (ch == '#' || ch == '=' || e.KeyCode == Keys.Enter)//# = enter
                        {
                            if (e.KeyCode == Keys.Enter)
                            {
                                calculation.Add(new CalcLine(txt.Substring(0, txt.Length)));
                            }
                            else
                            {
                                calculation.Add(new CalcLine(txt.Substring(0, txt.Length - 1)));
                            }
                            if (ch == '=' || e.KeyCode == Keys.Enter)
                            {
                                calculation.Add(new CalcLine("="));
                            }
                            else if (ch == '#')
                            {
                                calculation.Add(new CalcLine("#"));
                            }
                            txtEnter.Text = "";
                        }
                        else if ((ch == '+') || (ch == '-') || (ch == '*') || (ch == '/'))//add the front data to listbox 
                        {
                            CalcLine calcline = new CalcLine(txt.Substring(0, txt.Length - 1));
                            calculation.Add(calcline);
                            txtEnter.Text = txt[txt.Length - 1].ToString();
                            txtEnter.Focus();
                            txtEnter.SelectionStart = txtEnter.Text.Length;
                        }
                        else if (!(ch >= (char)48) && (ch <= (char)57))//numeric
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                            txtEnter.Text = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect data format", "Invalid Data Format");
                        txtEnter.Text = "";
                    }
                   
                }
            }
            //lstOutoput.Items.Count > 0
            else
            {
                //if last item in the list box is total,then consider as listbox is empty
                if (lstOutoput.Items[lstOutoput.Items.Count - 1].ToString().Contains("="))
                {
                    if (txt.Length == 0)
                    {
                        return;
                    }
                    else if (txt.Length == 1)
                    {
                        if (txt[0].ToString() == "#" || txt[0].ToString() == "=")
                        {
                            MessageBox.Show("No calculation available. \n So no subtotal or total can be displayed", "Invalid Request");
                            txtEnter.Text = "";
                        }
                        else if (!(txt[0].ToString() == "+" || txt[0].ToString() == "-"))
                        {
                            MessageBox.Show("First data entry must be either \'+\' or \'-\' as the beginning operator", "Invalid First Data Entry");
                            txtEnter.Text = "";
                        }

                    }
                    else
                    {
                        char ch = txt[txt.Length - 1];
                        if(txt[1] >= (char)48 && txt[1] <= (char)57)
                        {
                            if (ch == '#' || ch == '=' || e.KeyCode == Keys.Enter)//# =
                            {
                                if (e.KeyCode == Keys.Enter)
                                {
                                    calculation.Add(new CalcLine(txt.Substring(0, txt.Length)));
                                }
                                else
                                {
                                    calculation.Add(new CalcLine(txt.Substring(0, txt.Length - 1)));
                                }
                                if (ch == '=' || e.KeyCode == Keys.Enter)
                                {
                                    calculation.Add(new CalcLine("="));
                                }
                                else if (ch == '#')
                                {
                                    calculation.Add(new CalcLine("#"));
                                }
                                txtEnter.Text = "";
                            }
                            else if ((ch == '+') || (ch == '-') || (ch == '*') || (ch == '/'))
                            {
                                CalcLine calcline = new CalcLine(txt.Substring(0, txt.Length - 1));
                                calculation.Add(calcline);
                                txtEnter.Text = txt[txt.Length - 1].ToString();
                                txtEnter.Focus();
                                txtEnter.SelectionStart = txtEnter.Text.Length;
                            }
                            else if (!(ch >= (char)48) && (ch <= (char)57))
                            {
                                MessageBox.Show("Incorrect data format", "Invalid Data Format");
                                txtEnter.Text = "";
                            }
                        }
                        else
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                            txtEnter.Text = "";
                        }
                   
                    }
                }
                else
                {
                    if (txt.Length == 0)
                    {
                        return;
                    }
                    else if (txt.Length == 1)
                    {
                        if (!(txt[0].ToString() == "#" || txt[0].ToString() == "=" || txt[0].ToString() == "+" || txt[0].ToString() == "-" || txt[0].ToString() == "*" || txt[0].ToString() == "/" || e.KeyCode == Keys.Enter))
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                            txtEnter.Text = "";
                        }
                    }
                    else
                    {
                        if(txt[1] >= (char)48 && txt[1] <= (char)57)
                        {
                            char ch = txt[txt.Length - 1];
                            if ((ch == '#') || (ch == '=') || e.KeyCode == Keys.Enter)//# =
                            {
                                if (e.KeyCode == Keys.Enter)
                                {
                                    calculation.Add(new CalcLine(txt.Substring(0, txt.Length)));
                                }
                                else
                                {
                                    calculation.Add(new CalcLine(txt.Substring(0, txt.Length - 1)));
                                }
                                if (ch == '=' || e.KeyCode == Keys.Enter)
                                {
                                    calculation.Add(new CalcLine("="));
                                }
                                else if (ch == '#')
                                {
                                    calculation.Add(new CalcLine("#"));
                                }
                                txtEnter.Text = "";
                            }
                            else if ((ch == '+') || (ch == '-') || (ch == '*') || (ch == '/'))
                            {
                                CalcLine calcline = new CalcLine(txt.Substring(0, txt.Length - 1));
                                calculation.Add(calcline);
                                txtEnter.Text = txt[txt.Length - 1].ToString();
                                txtEnter.Focus();
                                txtEnter.SelectionStart = txtEnter.Text.Length;
                            }
                            else if (!(ch >= (char)48) && (ch <= (char)57))
                            {
                                MessageBox.Show("Incorrect data format", "Invalid Data Format");
                                txtEnter.Text = "";
                            }
                        }
                        else
                        {
                            MessageBox.Show("Incorrect data format", "Invalid Data Format");
                            txtEnter.Text = "";
                        }
     
                    }
                }
            }
        }
        /// <summary>
        /// show print preview dialog
        /// </summary>
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(lstOutoput.Items.Count == 0)
            {
                MessageBox.Show("You must have valid data in your list box before printing", "Error");
            }
            else
            {
                prvPrint.Show();
            }
        }
        /// <summary>
        /// load the data from the listbox and display in the print preview dialog
        /// </summary>
        private void printString_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string text = "";
            foreach(var item in lstOutoput.Items)
            {
                text += item.ToString() + "\n";
            }
            e.Graphics.DrawString(text, new Font("Arial", 30, FontStyle.Bold),
                Brushes.Black, 30, 60);
        }


    }
}
