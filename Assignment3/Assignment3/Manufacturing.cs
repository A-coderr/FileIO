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

namespace Assignment3
{
    /* FileIO
     * Manufacturing
     * A. Kostyuk Mar 2019
     */
    public partial class Manufacturing : Form
    {
        StreamWriter writer;
        StreamReader reader;

        string filePath = " ";
        string record = "";

        public Manufacturing()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreateOpen_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "";
            if (!Path()) return;
            try
            {
                filePath = txtPath.Text;

                //Add existing path
                if (rbExisting.Checked)
                {
                    int lastSlaash = filePath.LastIndexOf("\\");
                    string path = filePath.Substring(0, lastSlaash);
                    string file = filePath.Substring(lastSlaash);
                    path = filePath.Substring(0, lastSlaash);

                    //If path does not exist create one
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        rbNew.Checked = true;
                        rbExisting.Checked = false;
                        txtDisplay.Text = "New Directory Path Created";
                    }
                    else
                    {
                        txtDisplay.Text = "Path is found";
                    }
                }
                //Create new path
                else if (rbNew.Checked)
                {
                    int lastSlaash = filePath.LastIndexOf("\\");
                    string path = filePath.Substring(0, lastSlaash);
                    string file = filePath.Substring(lastSlaash);
                    path = filePath.Substring(0, lastSlaash);
                    Directory.CreateDirectory(path);
                    txtDisplay.Text = "New Directory Path Created";
                }
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                if (writer != null) writer.Close();
            }

        }

        // Edit the inputs
        private Boolean Path()
        {
            string errors = "";

            if (string.IsNullOrEmpty(txtPath.Text))
            {
                if (errors == "") txtPath.Focus();
                errors += "Path is required\n";
            }

            //Do some edits
            if (errors == "")
                return true;
            else
            {
                txtDisplay.Text = errors;
                return false;
            }
        }
       
        private Boolean Edit()
        {
            string errors = "";
            double dummy = 0;
            int dummyy = 0;

            if (string.IsNullOrEmpty(txtTransact.Text))
            {
                if (errors == "") txtTransact.Focus();
                errors += "Transaction # is required\n";
            }
            if (string.IsNullOrEmpty(txtSerial.Text))
            {
                if (errors == "") txtSerial.Focus();
                errors += "Serial # is required\n";
            }
            if (string.IsNullOrEmpty(txtTool.Text))
            {
                if (errors == "") txtTool.Focus();
                errors += "Tool purchased is required\n";
            }
            if (datDate.Value > DateTime.Now)
            {
                if (errors == "") datDate.Focus();
                errors += "Date cannot be in the future\n";
            }
            if (!double.TryParse(txtPrice.Text, out dummy))
            {
                if (errors == "") txtPrice.Focus();
                errors += "Price must be an integer\n";
            }
            if (!int.TryParse(txtQuantity.Text, out dummyy))
            {
                if (errors == "") txtQuantity.Focus();
                errors += "Quantity must be an integer\n";
            }

            //Do some edits
            if (errors == "")
                return true;
            else
            {
                txtDisplay.Text = errors;
                return false;
            }
        }

        private Boolean Delete()
        {
            string errors = "";

            if (string.IsNullOrEmpty(txtDeleteTransact.Text))
            {
                if (errors == "") txtDeleteTransact.Focus();
                errors += "Transaction # is required to delete a record\n";
            }

            //Do some edits
            if (errors == "")
                return true;
            else
            {
                txtDisplay.Text = errors;
                return false;
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            //Add record to the file
            txtDisplay.Text = "";
            if (!Edit()) return;
            try
            {
                double price = double.Parse(txtPrice.Text);
                double quantity = double.Parse(txtQuantity.Text);
                double result = price * quantity;
                string amount = result.ToString();
                txtAmount.Text = amount;

                writer = new StreamWriter(filePath, true);
                record = txtTransact.Text + "\t";
                record += datDate.Value.ToString("dd-MMM-yyyy") + "\t";
                record += txtSerial.Text + "\t";
                record += txtTool.Text + "\t";
                record += txtPrice.Text + "\t";
                record += txtQuantity.Text + "\t";
                record += txtAmount.Text;

                writer.WriteLine(record);
                lblMessage.Text = "Transaction added";
            }
            catch (Exception ex)
            {
                MessageBox.Show("exception writing new transaction:\n" + ex.Message);
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            //Display Data
            txtDisplay.Text = "#".PadRight(8) +
                        "Purchase-Date".PadRight(18) +
                        "Serial #".PadRight(20) +
                        "Manufacturing Tools".PadRight(35) + 
                        "Price".PadRight(10) + 
                        "Qty".PadRight(6) + 
                        "Amount".PadRight(12) + 
                        "\n";
            
            using (reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    record = reader.ReadLine();
                    string[] fields = record.Split('\t');
                    txtDisplay.Text += fields[0].PadRight(8) +
                        fields[1].PadRight(18) +
                        fields[2].PadRight(20) +
                        fields[3].PadRight(35) + 
                        "$"+fields[4].PadRight(10) + 
                        fields[5].PadRight(6) +
                        "$" + fields[6].PadRight(12) + 
                        "\n";
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //Delete Record
            if (!Delete()) return;
            try
            {
                List<string> data = new List<string>();
                txtDisplay.Text = "";
                using (reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        record = reader.ReadLine();
                        if (!record.StartsWith(txtDeleteTransact.Text + "\t"))
                        {
                            data.Add(record);
                        }
                        else
                        {
                            txtDisplay.Text = "Found it!";
                        }
                    }
                }
                if (txtDisplay.Text == "")
                    txtDisplay.Text = "Record not found";
                else
                {
                    try
                    {
                        writer = new StreamWriter(filePath, append: false);
                        foreach (var item in data)
                        {
                            writer.WriteLine(item);
                        }
                        lblMessage.Text = "Record deleted";
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = $"exception recreating the file: {ex.Message}";
                    }
                    finally
                    {
                        if (writer != null) writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("exception writing new transaction:\n" + ex.Message);
            }
            finally
            {
                if (writer != null) writer.Close();
            }
            
        }

        private void Manufacturing_Load(object sender, EventArgs e)
        {
           
        }
    }
}
