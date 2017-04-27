using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace IMEI_Check
{
    public partial class Form1 : Form
    {
        public static int last;

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private static string GetLuhnCheckDigit(string number)
        {
            var sum = 0;
            var alt = true;
            var digits = number.ToCharArray();
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                var curDigit = (digits[i] - 48);
                if (alt)
                {
                    curDigit *= 2;
                    if (curDigit > 9)
                        curDigit -= 9;
                }
                sum += curDigit;
                alt = !alt;
            }
            if ((sum % 10) == 0)
            {
                last = 0;
                return "0";
            }
            int temp = sum % 10;
            temp = temp - 10;
            temp = temp * -1;
            last = temp;
            return (10 - (sum % 10)).ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            GetLuhnCheckDigit(textBox1.Text);
            if (textBox1.Text.Length == 14)
            {
                Clipboard.SetText(textBox1.Text + last.ToString());
            }
            textBoxResult.Text = textBox1.Text + last.ToString();
        }

        private void importCSVbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Filter = "CSV Files | *.csv";
            Dialog.Multiselect = false;
            string path = Dialog.FileName;

            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Original");
            dt1.Columns.Add("New");


            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var fs = File.OpenRead(Dialog.FileName))
                    {
                        using (var reader = new StreamReader(fs))
                        {

                            List<string> lista = new List<string>();
                            List<string> listb = new List<string>();
                            string temp;

                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                var values = line.Split(',');
                                lista.Add(values[0]);
                                GetLuhnCheckDigit(values[0]);
                                listb.Add(last.ToString());
                                temp = values[0] + last.ToString();
                                dt1.Rows.Add(values[0], temp);
                            }
                            dataGridView1.DataSource = dt1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
