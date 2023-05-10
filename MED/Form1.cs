using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MED
{
    public partial class Form1 : Form
    {
        public class wordDistance
        {
            public string word;
            public int distance;

        }
        List<wordDistance> words = new List<wordDistance>();
        List<wordDistance> wordsFive = new List<wordDistance>();
        List<string> steps = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            string getWord = textBox1.Text;
            if (!String.IsNullOrEmpty(getWord))
            {
                string[] file1 = File.ReadAllLines(@"sozluk.txt", Encoding.GetEncoding("iso-8859-9"));
                for (int i = 0; i < file1.Length; i++)
                {
                    string datas = file1[i].ToLower(new CultureInfo("tr-TR", false));
                    int distance_ = ComputeDistance(getWord, datas);
                    words.Add(new wordDistance { word = datas, distance = distance_ });
                }
            }
            sortDistances();
            watch.Stop();
            label4.Text = watch.ElapsedMilliseconds.ToString() + " " + "ms";
        }
        private void sortDistances()
        {
            List<int> distances = new List<int>();
            for (int i = 0; i < words.Count; i++)
            {
                if (!distances.Contains(words[i].distance))
                {
                    distances.Add(words[i].distance);
                }
            }
            int[] dists = distances.ToArray();
            Array.Sort(dists);

            findFive(dists);
        }
        private void findFive(int[] dists)
        {
            int index = 0;
            for (int i = 0; i < dists.Length; i++)
            {
                for (int j = 0; j < words.Count; j++)
                {
                    if (index < 5)
                    {
                        if (words[j].distance == dists[i])
                        {
                            wordsFive.Add(new wordDistance { word = words[j].word, distance = words[j].distance });
                            index++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //textBox2.Clear();
            foreach (var item in wordsFive)
            {
                
                textBox2.Text += item.word + " " + item.distance + Environment.NewLine;
            }
        }

        public int ComputeDistance(string first, string second)
        {
            if (first.Length == 0)
            {
                return second.Length;
            }

            if (second.Length == 0)
            {
                return first.Length;
            }

            var current = 1;
            var previous = 0;
            var r = new int[2, second.Length + 1];
            for (var i = 0; i <= second.Length; i++)
            {
                r[previous, i] = i;
            }

            for (var i = 0; i < first.Length; i++)
            {
                r[current, 0] = i + 1;

                for (var j = 1; j <= second.Length; j++)
                {
                    var cost = (second[j - 1] == first[i]) ? 0 : 1;
                    r[current, j] = Min(
                        r[previous, j] + 1,
                        r[current, j - 1] + 1,
                        r[previous, j - 1] + cost);
                    //Delete, Insert, Replace
                }
                previous = (previous + 1) % 2;
                current = (current + 1) % 2;
            }
            return r[previous, second.Length];
        }
        public int ComputeDistanceSteps(string first, string second)
        {
       
            if (first.Length == 0)
            {
                return second.Length;
            }

            if (second.Length == 0)
            {
                return first.Length;
            }
            dataGridView1.ColumnCount = second.Length + 2;
            dataGridView1.RowCount = first.Length + 1;
            dataGridView1.Columns[1].Name = "#";
            dataGridView1.Rows[0].Cells[0].Value = "#";
            for (int i = 0; i < second.Length; i++)
            {
                dataGridView1.Columns[i + 2].Name = second[i].ToString().ToUpper();
            }
            for (int i = 0; i < first.Length; i++)
            {
                dataGridView1.Rows[i + 1].Cells[0].Value = first[i].ToString().ToUpper();
            }
            int row_table = 0;
            int col_table = 1;
            var current = 1;
            var previous = 0;
            var r = new int[2, second.Length + 1];
            for (var i = 0; i <= second.Length; i++)
            {
                r[previous, i] = i;
                dataGridView1.Rows[row_table].Cells[col_table].Value = i.ToString();
                col_table++;
            }
            row_table++;
            for (var i = 0; i < first.Length; i++)
            {
                r[current, 0] = i + 1;
                dataGridView1.Rows[row_table].Cells[1].Value = (i + 1).ToString();
                for (var j = 1; j <= second.Length; j++)
                {
                    var cost = (second[j - 1] == first[i]) ? 0 : 1;
                    r[current, j] = Min(
                        r[previous, j] + 1,
                        r[current, j - 1] + 1,
                        r[previous, j - 1] + cost);
                    if (cost == 1)
                    {
                        if (r[current, j] == r[previous, j] + 1)
                        {
                            steps.Add("Delete");
                        }
                        else if (r[current, j] == r[current, j - 1] + 1)
                        {
                            steps.Add("Insert");
                        }
                        else if (r[current, j] == r[previous, j - 1])
                        {
                            steps.Add("Replace");
                        }
                    }
                    else if (cost == 0)
                    {
                        steps.Add("Replace");
                    }
                    dataGridView1.Rows[row_table].Cells[j + 1].Value = (r[current, j]).ToString();
                    //Delete, Insert, Replace
                }
                row_table++;
                previous = (previous + 1) % 2;
                current = (current + 1) % 2;
            }
            foreach (var item in steps)
            {
                textBox6.Text += item + Environment.NewLine;
            }
            return r[previous, second.Length];
        }
        private static int Min(int e1, int e2, int e3) =>
            Math.Min(Math.Min(e1, e2), e3);

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            string word_1 = textBox3.Text;
            string word_2 = textBox4.Text;

            int dist = ComputeDistanceSteps(word_1, word_2);

            string result = dist.ToString();
            textBox5.Text = result;
            watch.Stop();
            label5.Text = watch.ElapsedMilliseconds.ToString() + " " + "ms";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
