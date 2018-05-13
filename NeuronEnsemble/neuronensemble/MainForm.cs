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

using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Graphviz;
using QuickGraph.Collections;
using QuickGraph.Concepts;
using QuickGraph.Data;
using QuickGraph.Exceptions;
using QuickGraph.Graphviz;
using QuickGraph.Predicates;
using QuickGraph.Representations;
using QuickGraph.Serialization;
using QuickGraph.Web;
//using QuickGraph.Graphviz.Dot;

using QuickGraph.Graphviz;
using System.Diagnostics;
using examples.Support;

namespace NeuronEnsemble
{
    public partial class MainForm : Form
    {
        public DataSet1 ds;
        private DataGridViewCellStyle tt;
        NeuralNet nn;
        private NeuronEnsemble.forImages fI;

        public MainForm(DataSet1 _ds)
        {
            InitializeComponent();
            //tabPage1.Parent = null;
            ds = _ds;
            ToolStripControlHost host = new ToolStripControlHost(checkBox1);
            toolStrip1.Items.Insert(3, host);
            ToolStripControlHost host2 = new ToolStripControlHost(numericUpDown1);
            host2.Width = 100;
            toolStrip1.Items.Insert(10, host2);

            dataGridView2.Columns.Add("ПОРОГИ", "ПОРОГИ");
            int i = dataGridView2.Columns.Count;
            dataGridView2.Columns[i - 1].Width = 250;
            dataGridView3.Columns.Add("АКТИВНОСТЬ", "АКТИВНОСТЬ");
            i = dataGridView3.Columns.Count;
            dataGridView3.Columns[i - 1].Width = 250;
            nn = new NeuralNet();
            nn.isChenged = false;
            DownloadData();

            //dataGridView2.Rows.RemoveAt(dataGridView2.Rows.Count - 1);
        }

        // загрузка данных из файла
        public void DownloadData()
        {

            if (ds != null)
            {
                // загружаем нейроны
                foreach(DataSet1.NeuronRow dn in ds.Neuron.Rows)
                {
                    AddNeuron(dn);
                }
                // загружаем связи
                foreach (DataSet1.WeightRow dw in ds.Weight.Rows)
                {
                    int iRow = 0;
                    int iCol = 0;
                    foreach (DataGridViewColumn dc in dataGridView1.Columns)
                        if (int.Parse(dc.HeaderText) != dw.idNeuronIn)  iCol++; 
                        else break;
                    foreach (DataGridViewRow dr in dataGridView1.Rows)
                        if (int.Parse(dr.HeaderCell.Value.ToString()) != dw.idNeuronFrom) iRow++;
                        else break;
                    if (Math.Round(dw.weight, 0) - dw.weight != 0)
                        dataGridView1.Rows[iRow].Cells[iCol].Value = dw.weight;
                    else
                        dataGridView1.Rows[iRow].Cells[iCol].Value = ((int) Math.Round(dw.weight, 0)).ToString();
                    //j = int.Parse(dataGridView1.Columns[i - 1].HeaderText);
                }
                colourGrid();  
            }
        }

        // преобразование символьной строки в действительное число
        public double ConvertSTRToFloat(string s)
        {
            s = s.Replace(',', '.');
            if (s.Trim().Length > 0)
            {
                double result = 0;
                bool minus = false;
                const char C1 = '.';
                const char C2 = 'e';
                if (s[0] == '-')
                {
                    minus = true;
                }
                //			const char C3 = '-';
                //			const char C3 = '
                char[] delimiters = new char[] { C1, C2 };
                //			string output = "";
                int ctr = 1;
                // выделяем подстроки на основе разделителей
                // и сохраняем результат
                foreach (string substring in s.Split(delimiters))
                {
                    if (ctr == 1)
                    {
                        result = ConvertSTRToInt(substring);
                        ctr = ctr + 1;
                    }
                    else if (ctr == 2)
                    {
                        if (result < 0)
                        {
                            result = result - ConvertFraction(substring);
                        }
                        else
                        {
                            result = result + ConvertFraction(substring);
                        }
                        ctr += 1;
                    }
                    else if (ctr == 3)
                    {
                        result = result * Math.Exp(ConvertSTRToInt(substring) * Math.Log(10));
                    }
                }
                if ((minus) && (result > 0))
                {
                    return -result;
                }
                else
                {
                    return result;
                };
            }
            else return 0;
        }

        // преобразование символьной строки в целое число
        public int ConvertSTRToInt(string s)
        {
            if (s.Trim().Length > 0)
            {
                int result = 0;
                bool minus = false;
                string st;

                if (s[0] == '-')
                {
                    minus = true;
                    st = s.Substring(1, s.Length - 1);
                }
                else
                {
                    st = s;
                }

                for (int k = 0; k < st.Length; k++)
                {
                    result = result * 10 + (st[k] - '0');
                }
                if (minus)
                {
                    return -result;
                }
                else
                {
                    return result;
                }
            }
            else return 0;
        }

        // преобразование символьной строки в целое число
        public double ConvertFraction(string s)
        {
            double result = 0;
            for (int k = 0; k < s.Length; k++)
            {
                result = result + (s[k] - '0') * Math.Pow(10, (-k - 1)); //*Math.Exp((-k-1)*Math.Log(10));
            }
            return result;
        }

        private void tabPage1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*Image image = Image.FromFile("1.png");

            this.pictureBox1.Image = image;



            CalculateDistance g = new CalculateDistance();
            g.SetUpEdgesAndCosts(table, ens, checkedListBox1);
            g.ShortesDistanceTest();
            g.Visualize();
            Image image2 = Image.FromFile("C:\\temp\\distance.png");
            this.pictureBox1.Image = image2;*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddNeuron();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            nn.isChenged = true;
            AddNeuron();
            SaveNetwork();
            //this.dataGridView1.Columns.wid
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void AddNeuron()
        {
            int i = dataGridView1.Columns.Count;
            int j = 0;
            if (i > 0) j = int.Parse(dataGridView1.Columns[i - 1].HeaderText);

            dataGridView1.Columns.Add((j + 1).ToString(), (j + 1).ToString());
            i = dataGridView1.Columns.Count;
            dataGridView1.Columns[i - 1].Width = 40;
            dataGridView1.Rows.Add();
            dataGridView1.Rows[i - 1].HeaderCell.Value = (j + 1).ToString();
            //for(int k=0; k<i+1; k++)
            //{
            //    dataGridView1.Rows[i - 1].Cells[k].Value = 0;
            //}
            
            dataGridView2.Rows.Add();
            dataGridView2.Rows[i - 1].HeaderCell.Value = (j + 1).ToString();
            dataGridView2.Rows[i - 1].SetValues(0);

            dataGridView3.Rows.Add();
            dataGridView3.Rows[i - 1].HeaderCell.Value = (j + 1).ToString();
            dataGridView3.Rows[i - 1].SetValues(toolStripTextBox1.Text);
        }

        private void AddNeuron(DataSet1.NeuronRow dn)
        {
            int numN = dn.Number;
            int i = dataGridView1.Columns.Count;
            /*int j = 0;
            if (i > 0) j = int.Parse(dataGridView1.Columns[i - 1].HeaderText);*/

            dataGridView1.Columns.Add((numN).ToString(), (numN).ToString());
            i = dataGridView1.Columns.Count;
            dataGridView1.Columns[i - 1].Width = 40;
            dataGridView1.Rows.Add();
            dataGridView1.Rows[i - 1].HeaderCell.Value = (numN).ToString();
            //for(int k=0; k<i+1; k++)
            //{
            //    dataGridView1.Rows[i - 1].Cells[k].Value = 0;
            //}

            dataGridView2.Rows.Add();
            dataGridView2.Rows[i - 1].HeaderCell.Value = (numN).ToString();
            dataGridView2.Rows[i - 1].SetValues(dn.treshhold);

            dataGridView3.Rows.Add();
            dataGridView3.Rows[i - 1].HeaderCell.Value = (numN).ToString();
            dataGridView3.Rows[i - 1].SetValues(dn.outValue);
        }

        private void DeleteNeuron()
        {
            int numDelNeuron = this.dataGridView1.CurrentRow.Index;
            dataGridView1.Rows.RemoveAt(numDelNeuron);
            dataGridView1.Columns.RemoveAt(numDelNeuron);

            dataGridView2.Rows.RemoveAt(numDelNeuron);
            dataGridView3.Rows.RemoveAt(numDelNeuron);

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            colourGrid();     
        }

        private void colourGrid()
        {
            if (checkBox1.Checked)
            {
                dataGridView1.ClearSelection();
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        double ww = ConvertSTRToFloat(cell.Value.ToString());
                        if (ww > 0) cell.Style.BackColor = Color.LightPink;
                        else if (ww < 0) cell.Style.BackColor = Color.LightSeaGreen;
                        else cell.Style.BackColor = Color.White;
                    }
                }
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    tt = this.dataGridView1.DefaultCellStyle;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!(row.Index == e.RowIndex))
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.ColumnIndex == e.ColumnIndex)
                                {
                                    cell.Style.BackColor = Color.PowderBlue;
                                }
                            }
                        }
                        else
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.ColumnIndex == e.ColumnIndex)
                                {
                                    cell.Style.BackColor = Color.Pink;
                                }
                                else
                                {
                                    cell.Style.BackColor = Color.PowderBlue;
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    tt = this.dataGridView1.DefaultCellStyle;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!(row.Index == e.RowIndex))
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.ColumnIndex == e.ColumnIndex)
                                {
                                    cell.Style.BackColor = Color.White;
                                }
                            }
                        }
                        else
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.ColumnIndex == e.ColumnIndex)
                                {
                                    cell.Style.BackColor = Color.White;
                                }
                                else
                                {
                                    cell.Style.BackColor = Color.White;
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            nn.isChenged = true;
            DeleteNeuron();
            SaveNetwork();
            //toolStripButton3.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SaveNetwork();
            SaveNetworkToDS();
        }

        private void SaveNetwork()
        {
            if (nn.isChenged == true && dataGridView1.Rows.Count>1)
            {
                double[,] val = new double[dataGridView1.Rows.Count, dataGridView1.ColumnCount]; // потому что есть последняя лишняя строка
                double[] treshholds = new double[dataGridView2.Rows.Count];
                int[] ids = new int[dataGridView1.ColumnCount];
                int[] startValues = new int[dataGridView1.ColumnCount];

                int nNeurons = dataGridView1.Rows.Count;
                int count = 1;

                // подготовка матрицы порогов
                foreach (DataGridViewRow dr in dataGridView2.Rows)
                {
                    if ((dr.Cells[0].Value != null) && (dr.Cells[0].Value.ToString() != ""))
                    {
                        treshholds[dr.Index] = ConvertSTRToFloat(dr.Cells[0].Value.ToString());
                    }
                }
                // подготовка матрицы весов
                foreach (DataGridViewRow dr in dataGridView1.Rows)
                {
                    //if (count <= nNeurons)
                    //{
                    for (int i = 0; i < nNeurons; i++)
                    {
                        if ((dr.Cells[i].Value != null) && (dr.Cells[i].Value.ToString() != "")) val[dr.Index, i] = ConvertSTRToFloat(dr.Cells[i].Value.ToString());
                        else val[dr.Index, i] = 0;

                    }
                    count++;
                    //}

                }
                // подготовка матрицы idшников нейронов
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    ids[i] = int.Parse(dataGridView1.Columns[i].HeaderText);
                }
                count = 0;
                foreach (DataGridViewRow dr in dataGridView3.Rows)
                {
                    if ((dr.Cells[0].Value != null) && (dr.Cells[0].Value.ToString() != ""))
                    {
                        startValues[count] = ConvertSTRToInt(dr.Cells[0].Value.ToString());
                        count++;
                    }
                }

                nn = new NeuralNet(val, treshholds, dataGridView1.Rows.Count, ids, startValues, (int)numericUpDown2.Value);

                //SaveNetworkToDS();
                nn.isChenged = false;
            }
        }


        private void SaveNetworkToDS()
        {
                ds.Neuron.Clear();
                ds.Weight.Clear();
                Neuron[] ns = nn.getNet();
                // заполнение таблицы Neuron
                foreach (Neuron n in ns)
                {
                    DataSet1.NeuronRow nr = ds.Neuron.NewNeuronRow();
                    nr.Number = n.getId();
                    nr.treshhold = n.getTreshhold();
                    nr.outValue = n.getOutValue();
                    ds.Neuron.AddNeuronRow(nr);
                }
                // заполнение таблицы Weight
                foreach (Neuron n in ns)
                {
                    DataSet1.NeuronRow[] nr = (DataSet1.NeuronRow[])ds.Neuron.Select("Number=" + n.getId().ToString());
                    int IdIn = nr[0].Number;
                    List<Weight> weights = n.getInConnects();
                    foreach (Weight wn in weights)
                    {
                        Neuron tekN = wn.getNeuron();
                        nr = (DataSet1.NeuronRow[])ds.Neuron.Select("Number=" + tekN.getId().ToString());
                        int FromId = nr[0].Number;
                        DataSet1.WeightRow wr = ds.Weight.NewWeightRow();
                        wr.idNeuronFrom = FromId;
                        wr.weight = wn.getWeight();
                        wr.idNeuronIn = IdIn;
                        ds.Weight.AddWeightRow(wr);
                    }
                }

                // заполнение таблицы Ensemble 
                string str = "";
                List<List<Neuron>> listEs = nn.getEnsembles();
                foreach (List<Neuron> ln in listEs)
                {
                    foreach (Neuron ln2 in ln)
                    {
                        str = str + "-" + ln2.getId().ToString();
                    }
                    str = str.Substring(1, str.Length - 1);
                    foreach (Neuron ln2 in ln)
                    {
                        DataSet1.EnsembleRow er = ds.Ensemble.NewEnsembleRow();
                        er.Name = str;
                        er.idNeuron = ln2.getId();
                        ds.Ensemble.AddEnsembleRow(er);
                    }
                    str = "";
                }            
        }

        private void DrawNetwork()
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            SaveNetwork();
            SaveNetworkToDS();
            dataGridView4.Rows.Clear();
            dataGridView4.Columns.Clear();

            int nNeurons = dataGridView1.Rows.Count;
            int[] res;
            
            int times = ConvertSTRToInt(numericUpDown1.Value.ToString());
            for (int t = 0; t < times+1; t++)
            {
                dataGridView4.Columns.Add("t=" + t.ToString(), "t=" + t.ToString());
                int j = dataGridView4.Columns.Count;
                dataGridView4.Columns[j - 1].Width = 50;

                if (t == 0)
                {
                    for (int i = 0; i < nNeurons; i++)
                    {
                        dataGridView4.Rows.Add();
                        dataGridView4.Rows[i].HeaderCell.Value = "Нейрон "+(i + 1).ToString();
                    }
                }

                res = nn.getValues();
                int count = 0;
                foreach (DataGridViewRow dr in dataGridView4.Rows)
                {
                    if (count < nNeurons)
                    {
                        dr.Cells[t].Value = res[count];
                        count++;
                    }
                }
                nn.Step();
            }

            foreach (DataGridViewRow row in dataGridView4.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        double ww = ConvertSTRToFloat(cell.Value.ToString());
                        if (ww > 0) cell.Style.BackColor = Color.LightPink;
                        else if (ww < 0) cell.Style.BackColor = Color.LightSeaGreen;
                        else cell.Style.BackColor = Color.White;
                        cell.Value = "";
                    }
                }
            }

            // cell.Style.BackColor = Color.PowderBlue;

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            SearchNeuronEnsembles();
        }

        public void SearchNeuronEnsembles()
        {
            //if ()
            SaveNetwork();
            SaveNetworkToDS();
            treeView1.Nodes.Clear();            
            string str = "";
            List<List<Neuron>> listEs = nn.getEnsembles();

            nn.getTreeView(treeView1.Nodes);
            /*
            int myId = 1;
            foreach(List<Neuron> ln in listEs)
            {
                foreach (Neuron ln2 in ln)
                {
                    str = str + "-" + ln2.getId().ToString();
                }
                str = str.Substring(1, str.Length - 1);
                treeView1.Nodes.Add(new TreeNode(str));
                //treeView1.Nodes.Add(myId.ToString(), str, imageList1.Images[0].);
                foreach(Neuron ln2 in ln)
                {
                    treeView1.Nodes[listEs.IndexOf(ln)].Nodes.Add(new TreeNode(ln2.getId().ToString()));
                }
                myId = myId+1;
                str = "";
            }
            //treeView1*/
        }

        private void toolStripTextBox1_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (DataGridViewRow dr in dataGridView2.Rows)
            {
                dr.Cells[0].Value = toolStripTextBox1.Text;
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            nn.isChenged = true;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            nn.isChenged = true;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            nn.isChenged = true;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            nn.isChenged = true;
            //SaveNetwork();
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            nn.isChenged = true;
            //SaveNetwork();
        }

        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            nn.isChenged = true;
            //SaveNetwork();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }

        private void processingNode()
        {
            label1.Text = treeView1.SelectedNode.Name;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
          
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            label1.Text = "Ансамбль: "+treeView1.SelectedNode.Text;
            checkedListBox1.Items.Clear();
            EnsembleGraphNode ens = (EnsembleGraphNode)treeView1.SelectedNode.Tag;//.ensemble;
            foreach (Neuron neur in ens.getInputNeurons())
            {
                checkedListBox1.Items.Add(neur.getId());                
            }
        }
        private void getAutonataTable()
        {
            dataGridView5.Rows.Clear();
            dataGridView5.Columns.Clear();

            EnsembleGraphNode ens = (EnsembleGraphNode)treeView1.SelectedNode.Tag;
            
            string[,] table = ens.getAutomataTable(checkedListBox1.CheckedItems);

            /*dataGridView1.Columns.Add((j + 1).ToString(), (j + 1).ToString());
            i = dataGridView1.Columns.Count;
            dataGridView1.Columns[i - 1].Width = 40;
            dataGridView1.Rows.Add();
            dataGridView1.Rows[i - 1].HeaderCell.Value = (j + 1).ToString();*/

            for (int i = 1; i <= ens.getAutomataWidth() - 1; i++)
            {
                dataGridView5.Columns.Add(table[0, i].ToString(), table[0, i].ToString());
                dataGridView5.Columns[i - 1].Width = 40;
            }
            
            
            for (int i = 1; i <= ens.getAutomataHeigh(checkedListBox1.CheckedItems)-1; i++)
            {
                dataGridView5.Rows.Add(); 
                dataGridView5.Rows[i - 1].HeaderCell.Value = table[i, 0].ToString();                
            }
            
            for (int i = 1; i < ens.getAutomataHeigh(checkedListBox1.CheckedItems); i++)
                for (int j = 1; j < ens.getAutomataWidth(); j++)
                {
                    dataGridView5.Rows[i-1].Cells[j-1].Value = table[i, j].ToString();
                }
           
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // получить таблицу автомата
            getAutonataTable();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            // рисовать граф автомата
            EnsembleGraphNode ens = (EnsembleGraphNode)treeView1.SelectedNode.Tag;
/*
            if (File.Exists(Directory.GetCurrentDirectory()+"\\automata.png"))
            {                
                File.Delete(Directory.GetCurrentDirectory() + "\\automata.png");
            }
 */

            // automata table
            string[,] table = ens.getAutomataTable(checkedListBox1.CheckedItems);

            CalculateDistance g = new CalculateDistance();
            g.SetUpEdgesAndCosts(table, ens, checkedListBox1);
            //g.ShortesDistanceTest();
            g.Visualize();

            ActivateforImages();
            //Image image2 = Image.FromFile("C:\\temp\\distance.png");
            //this.pictureBox1.Image = image2;
        }

        /// <summary>
        /// Проверяет, есть ли уже такая открытая форма
        /// </summary>
        /// <param name="formToActivate"> тип формы </param>
        /// <returns> Возвращает форму, если она найдена и null, если нет </returns>
        private Form GetChildWindow(Type formToActivate)
        {
            Form foundForm = null;
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType().ToString() == formToActivate.ToString())
                {
                    foundForm = f;
                    break;
                }
            }
            return foundForm;
        }
        
        private void ActivateforImages()
        {
            fI = null/*(NeuronEnsemble.forImages)GetChildWindow(Type.GetType("NeuronEnsemble.forImages"))*/;
            if (fI == null)
            {
                fI = new forImages();
                //fI.MdiParent = this;
                fI.Show();
            }
            else fI.Activate();
        }
    }

    

    public class FileDotEngine //: IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            string output = outputFileName;
            File.WriteAllText(output, dot);

            // assumes dot.exe is on the path:
            var args = string.Format(@"{0} -Tjpg -O", output);
            System.Diagnostics.Process.Start("dot.exe", args);
            return output;

        }
    }

    class CalculateDistance
    {
        private AdjacencyGraph<string, Edge<string>> _graph;
        private Dictionary<Edge<string>, string> _costs;

        public void SetUpEdgesAndCosts(string[,] table, EnsembleGraphNode ens, CheckedListBox checkedListBox1)  
        {
            _graph = new AdjacencyGraph<string, Edge<string>>();
            _costs = new Dictionary<Edge<string>, string>();

            //for(int i=0; i<table.Length)
            for (int j = 1; j <= ens.getAutomataWidth() - 1; j++) // по столбцам
            {
                for (int i = 1; i <= ens.getAutomataHeigh(checkedListBox1.CheckedItems) - 1; i++) // по строкам
                {
                    // AddEdgeWithCosts(table[0, j].ToString(), table[i, j].ToString(), 1);
                    AddEdgeWithCosts(table[0, j].ToString(), table[i, j].ToString(), table[i, 0].ToString());
                }
            }

/*            AddEdgeWithCosts("6", "7", 2.0);
            AddEdgeWithCosts("6", "1", 1.0);
            AddEdgeWithCosts("6", "8", 5.0);
            AddEdgeWithCosts("7", "6", 3.0);
            AddEdgeWithCosts("7", "2", 3.0);
            AddEdgeWithCosts("7", "8", 3.0);
            AddEdgeWithCosts("8", "6", 3.0);
            AddEdgeWithCosts("8", "2", 2.0);
            AddEdgeWithCosts("1", "6", 2.0);
            AddEdgeWithCosts("1", "3", 2.0);
            AddEdgeWithCosts("2", "6", 2.0);
            AddEdgeWithCosts("2", "1", 4.0);
            AddEdgeWithCosts("2", "3", 2.0);
            AddEdgeWithCosts("3", "1", 1.0);
            AddEdgeWithCosts("3", "5", 5.0);
            AddEdgeWithCosts("3", "4", 2.0);
            AddEdgeWithCosts("4", "3", 3.0);
            AddEdgeWithCosts("4", "2", 3.0);
            AddEdgeWithCosts("5", "2", 2.0);
            AddEdgeWithCosts("5", "3", 3.0);
            AddEdgeWithCosts("5", "4", 3.0);*/
        }

        private void AddEdgeWithCosts(string source, string target, string cost) //double cost)
        {
            var edge = new Edge<string>(source, target);
            _graph.AddVerticesAndEdge(edge);
            _costs.Add(edge, cost);
        }

        void CostEdgeFormatter(object sender, FormatEdgeEventArgs<string, Edge<string>> e)
        {
            e.EdgeFormatter.Label.Value = _costs[e.Edge].ToString(); //.ToString("0"); //0.0
        }

        public void Visualize()
        {
            Console.WriteLine(_graph.ToDotNotation());
            _graph.Visualize("automata", CostEdgeFormatter, Directory.GetCurrentDirectory());
            //Task.Delay(4000);
            Stopwatch sw = Stopwatch.StartNew();
            var delay = Task.Delay(4000).ContinueWith(_ =>
            {
                sw.Stop();
                return sw.ElapsedMilliseconds;
            });
        }

        
        public void ShortesDistanceTest()
        {
            //PrintShortestPath("4", "1");
        }
        /*
        private void PrintShortestPath(string @from, string to)
        {
            var edgeCost = AlgorithmExtensions.GetIndexer(_costs);
            var tryGetPath = _graph.ShortestPathsDijkstra(edgeCost, @from);

            IEnumerable<Edge<string>> path;
            if (tryGetPath(to, out path))
            {
                PrintPath(@from, to, path);
            }
            else
            {
                Console.WriteLine("No path found from {0} to {1}.");
            }
        }
        */
        private static void PrintPath(string @from, string to, IEnumerable<Edge<string>> path)
        {
            Console.Write("Path found from {0} to {1}: {0}", @from, to);
            foreach (var e in path)
                Console.Write(" > {0}", e.Target);
            Console.WriteLine();
        }
    }
}


