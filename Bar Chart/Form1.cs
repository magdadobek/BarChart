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
using System.Diagnostics;

namespace Bar_Chart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static class Global //Class with variables used globally
        {
            public static int N; //the biggest numeric value found in a text file; decides the histogram range  
            public static int[] Data = new int[N]; //table with filtered numeric data from a text file
            public static string path; //path to a text file
        }

        public int Find_biggest_value(string text) 
        {
            int j, b, N1;
            double analyzed_value;
            N1 = 0;
            for (int i = 0; i < text.Length;) 
            {
                b = 0;//the amount of checked characters in one for
                j = i;//position
                while (j < text.Length && Char.IsDigit(text, j) == true)
                {
                    if (Char.IsDigit(text, j) == true)
                    {
                        b++;
                        j++;
                        if (j + 1 < text.Length)
                        {
                            if (Char.IsDigit(text, (j + 1)) == true)
                            {
                                if (text.Substring(j, 1) == "," || text.Substring(j, 1) == ".")
                                {
                                    b++;
                                    j++;
                                }
                            }
                        }
                    }
                }
                if (b != 0)
                {
                    analyzed_value = double.Parse(text.Substring(i, b));
                    i += b;
                    if (analyzed_value > N1) //checking, if newly filtered value is greater than currently saved one
                    {
                        N1 = (int)Math.Ceiling(analyzed_value);
                    }
                }
                else i++;
            }
            return N1;
        }

        public int[] Add_data(int N1, string text) //function that adds data to a table Global.Data using the same algorithm as in Find_biggest_value
        {
            int[] table = new int[N1];
            int  j, b, value;
            double analyzed_value;
            for (int i = 0; i < text.Length;)
            {
                b = 0;
                j = i; 
                while (j < text.Length && Char.IsDigit(text, j) == true) 
                {
                    if (Char.IsDigit(text, j) == true) 
                    {
                        b++;
                        j++;
                        if (j + 1 < text.Length) {
                            if (Char.IsDigit(text, (j + 1)) == true)
                            {
                                if (text.Substring(j, 1) == "," || text.Substring(j, 1) == ".")
                                {
                                    b++;
                                    j++;
                                }
                            }
                        }
                    }
                }
                if (b != 0)
                {
                    analyzed_value = double.Parse(text.Substring(i, b));
                    i += b;
                    value = (int)Math.Floor(analyzed_value);
                    if (value == N1) value--;
                    table[value]++;
                }
                else i++;
            }
            return table;
        }

        public static void Add_checkboxes(CheckedListBox clb)
        {
            clb.Items.Clear();
            for (int i = 0; i < Global.N; i++)
            {
                clb.Items.Add($"{i}-{(i + 1)}");
                clb.SetItemChecked(i, true);
            }
        }

        public void Fill_richtextbox(int[] table, RichTextBox rtb, CheckedListBox clb)
        {
            int quantity = 0; 
            rtb.Text = "Filtered data:";
            for (int i = 0; i < table.Length; i++)
            {
                if (clb.GetItemChecked(i))
                {
                    rtb.AppendText("\r\n(" + i + "-" + (i + 1) + ")" + ": " + table[i]);
                    quantity += table[i];
                }
            }
            rtb.AppendText("\r\nThe amount of filtered values: " + quantity);
        }

        public void Export_to_textfile(RichTextBox rtb)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                InitialDirectory = @"c:\\",
                Title = "Choose a location",
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                RestoreDirectory = true,
                AddExtension = true,
                OverwritePrompt=true,
        };
            if (SFD.ShowDialog() == DialogResult.OK&& SFD.FileName != "")
            {
                if (SFD.CheckPathExists)
                {
                    try
                    {
                        StreamWriter writer = new StreamWriter(SFD.FileName);
                        writer.Write(rtb.Text);
                        writer.Close();
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("An error ocurred. Please try again or choose another path.");
                    }
                }
                else MessageBox.Show("Path does not exist");
            }
            Process.Start("notepad.exe", SFD.FileName);
        }


        private void Browse_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog
            {
                InitialDirectory = @"c:\\", 
                Title = "Choose a text file", 
                CheckFileExists = true, 
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox1.Text = OFD.FileName;
                    Export_Button.Enabled = true;
                    Chart_Button.Enabled = true;
                    Apply_Changes_Button.Enabled = true;

                    Global.path = textBox1.Text;
                    Process.Start("notepad.exe", Global.path);
                    StreamReader s = new StreamReader(Global.path);
                    String text = s.ReadToEnd();
                    s.Close();
                    Global.N = Find_biggest_value(text);
                    Global.Data = Add_data(Global.N, text);
                    Add_checkboxes(checkedListBox1);
                    Fill_richtextbox(Global.Data,richTextBox1,checkedListBox1);
                    MessageBox.Show("Operation succeed.");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("No access.");
                }
                catch (IOException)
                {
                    MessageBox.Show("An error ocurred. Please try again or choose another file.");
                }
            }
        }

        private void Export_button_Click(object sender, EventArgs e)
        {
            if (Global.N == 0)
            {
                MessageBox.Show("No data to save.");
            }
            else
            {
                try
                {
                    Export_to_textfile(richTextBox1);
                    MessageBox.Show("Operation succeed."); 
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("No access.");
                }
                catch (IOException)
                {
                    MessageBox.Show("An error ocurred. Please try again or choose another file.");
                }
            }
        }

        private void Chart_button_Click(object sender, EventArgs e) 
        {
            if (Global.N == 0)
            {
                MessageBox.Show("No data to create a chart");
            }
            else
            {
                Form2 f2 = new Form2();
                f2.Show();
            }
        }

        private void Apply_changes_button_Click(object sender, EventArgs e)
        {
            Fill_richtextbox(Global.Data,richTextBox1,checkedListBox1);
        }
    }
}
