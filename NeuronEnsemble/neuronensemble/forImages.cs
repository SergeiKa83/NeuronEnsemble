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

namespace NeuronEnsemble
{
    public partial class forImages : Form
    {
        public forImages()
        {
            InitializeComponent();
            //Image image = Image.FromFile("automata.png");
            //pictureBox1.Image = image;
            //pictureBox1.Refresh();
        }

        private void forImages_Load(object sender, EventArgs e)
        {
            Image image = Image.FromFile("automata-2.png"); //automata-2
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void forImages_FormClosed(object sender, FormClosedEventArgs e)
        {
            //pictureBox1.Image = null;
            pictureBox1.Image.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Directory.GetCurrentDirectory() + "\\automata.png");
/*                Bitmap image1 = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory()+"\\automata.png"
, true);

                TextureBrush texture = new TextureBrush(image1);
                texture.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;
                Graphics formGraphics = this.CreateGraphics();
                formGraphics.FillEllipse(texture,
                    new RectangleF(90.0F, 110.0F, 100, 100));
                formGraphics.Dispose();*/
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("There was an error opening the png." +
                    "Please check the path.");
            }
        }
    }
}
