using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace Bar_Chart
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            chart1.Series.Add("Values");
            for (int i = 0; i < Form1.Global.N; i++) 
            {
                comboBox1.Items.Add($"{i}-{(i + 1)}");
                Form1.Add_checkboxes(checkedListBox1);
                chart1.Series["Values"].Points.AddXY((i + "-" + (i + 1)), Form1.Global.Data[i]);
            }
        }
        private void All_checkbox_Check(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            chart1.Series.Add("Values");
            for (int i = 0; i < Form1.Global.N; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    chart1.Series["Values"].Points.AddXY((i + "-" + (i + 1)), Form1.Global.Data[i]);
                }
            }
        }

        private void Legend_button_Click(object sender, EventArgs e)
        {
            if (chart1.Series["Values"].IsVisibleInLegend == true) chart1.Series["Values"].IsVisibleInLegend = false;
            else chart1.Series["Values"].IsVisibleInLegend = true;
        }
        private void Colour_change_Click(object sender, EventArgs e)
        {
            ColorDialog CD = new ColorDialog
            {
                AllowFullOpen = true
            };
            if (CD.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (checkBox1.Checked)
                    {
                        for (int i = 0; i < Form1.Global.N; i++)
                        {
                                chart1.Series["Values"].Points[i].Color = CD.Color;
                        }
                    }
                    else
                    {
                        chart1.Series["Values"].Points[int.Parse(comboBox1.Text.Substring(0, 1))].Color = CD.Color;
                    }
                }
                catch (ArgumentException) { };
            }
        }
        private void Change_Charttype_Click(object sender, EventArgs e)
        {
            if (chart1.Series["Values"].ChartType == SeriesChartType.Column)
            {
                chart1.Series["Values"].ChartType = SeriesChartType.Pie;
            }
            else
            {
                chart1.Series["Values"].ChartType = SeriesChartType.Column;
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                InitialDirectory = @"c:\\",
                Title = "Choose a location",
                DefaultExt = "png",
                Filter = "PNG Image|*.png|JPeg Image|*.jpg",
                RestoreDirectory = true,
                AddExtension = true,
                OverwritePrompt = true,
            };
            if (SFD.ShowDialog() == DialogResult.OK && SFD.FileName != "")
            {
                if (SFD.CheckPathExists)
                {
                    try
                    {
                        if (SFD.FilterIndex == 1)
                        {
                            chart1.SaveImage(SFD.FileName, ChartImageFormat.Jpeg);
                        }
                        if (SFD.FilterIndex == 2)
                        {
                            chart1.SaveImage(SFD.FileName, ChartImageFormat.Png);
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("An error ocurred. Please try again or choose another path.");
                    }
                }
                else MessageBox.Show("Path does not exist");
            }
        }

    }
}
