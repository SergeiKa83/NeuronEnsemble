using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuickGraph;
using QuickGraph.Algorithms.Graphviz;

namespace NeuronEnsemble
{
    public partial class Form1 : Form
    {
        private NeuronEnsemble.MainForm fMain;
        public DataSet1 fData = new DataSet1();

        public Form1()
        {
            InitializeComponent();
            
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void закрытьПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void печатьВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void закрытьПроектToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void сведенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void ShowNetwork()
        {
            ActivateMainForm();
        }

        private void ActivateMainForm()
        {
            fMain = (NeuronEnsemble.MainForm)GetChildWindow(Type.GetType("NeuronEnsemble.MainForm"));
            if (fMain == null)
            {
                fMain = new MainForm(fData);
                fMain.MdiParent = this;
                fMain.Show();
            }
            else fMain.Activate();
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void редактированиеСетиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNetwork();
        }

        private void SaveProject(string fileName)
        {
            try
            {
                if (fData.Project.Count > 0)
                {
                    foreach (DataSet1.ProjectRow dr in fData.Project.Rows)
                    {
                        dr.DateOfModified = DateTime.Now;
                        dr.Name = fileName;
                    }
                }
                else 
                {
                    DataSet1.ProjectRow pr = fData.Project.NewProjectRow();
                    pr.Name = fileName;
                    pr.DateOfCreation = DateTime.Now;
                    pr.DateOfModified = DateTime.Now;
                    fData.Project.AddProjectRow(pr);
                }
                
                fData.WriteXml(fileName, XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {

            }
        }

        private void SaveProjectAs()
        {
            string ESFileName = "";
            using (SaveFileDialog fSaveDlg = new SaveFileDialog())
            {
                fSaveDlg.AddExtension = true;
                fSaveDlg.Title = "Сохранить данные";
                fSaveDlg.Filter = "XML-файл(*.xml)|*.xml";
                fSaveDlg.FilterIndex = 1;
                switch (fSaveDlg.ShowDialog())
                {
                    case DialogResult.OK:
                        ESFileName = fSaveDlg.FileName;
                        break;                        
                }
            }
            if (ESFileName != "") SaveProject(ESFileName);            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProjectAs();                
            }
            catch (Exception ex)
            {                
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (fData.Project.Rows.Count == 0) // в первый раз
            {
                SaveProjectAs();
            }
            else // не в первый раз
            {                
                 object[] drP;
                foreach (DataSet1.ProjectRow dr in fData.Project.Rows)
                {
                    dr.DateOfModified = DateTime.Now;
                    drP = dr.ItemArray;
                    fData.WriteXml(drP[0].ToString(), XmlWriteMode.IgnoreSchema);
                }                
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            fData = new DataSet1();
            toolStrip1.Items[2].Enabled = true;
            toolStrip1.Items[3].Enabled = true;
            toolStrip1.Items[4].Enabled = true;
            //menuStrip1.Items[0].
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (OpenProject() == DialogResult.OK)
            {
                toolStrip1.Items[2].Enabled = true;
                toolStrip1.Items[3].Enabled = true;
                toolStrip1.Items[4].Enabled = true;
            }
        }

        /// <summary>
        /// Вызывает диалог для открытия 
        /// </summary>
        public DialogResult OpenProject()
        {

            using (OpenFileDialog fOpenDlg = new OpenFileDialog())
            {
                fOpenDlg.CheckFileExists = true;
                fOpenDlg.CheckPathExists = true;
                fOpenDlg.AddExtension = true;
                fOpenDlg.Title = "Открыть проект";
                fOpenDlg.Filter = "XML-файл(*.xml)|*.xml";
                fOpenDlg.FilterIndex = 1;
                if (fOpenDlg.ShowDialog() == DialogResult.OK)
                {
                    string ESFileName = fOpenDlg.FileName;
                    // Грузим
                    OpenProject(ESFileName);
                    return DialogResult.OK;
                }
            }
            return DialogResult.Cancel;
        }

        /// <summary>
        /// Загрузить из файла
        /// </summary>
        /// <param name="fileName">Имя файла, из которого загружаем</param>
        private void OpenProject(string fileName)
        {
            try
            {
                fData.Clear();
                // Грузим  в DataSet
                fData.ReadXml(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в формате загружаемого файла.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ShowNetwork();
        }
    }
}
