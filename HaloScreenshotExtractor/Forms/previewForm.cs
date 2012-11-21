using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HaloScreenshots
{
    public partial class previewForm : Form
    {
        ScreenshotReader reader = new ScreenshotReader();

        public previewForm()
        {
            InitializeComponent();
        }

        public void showPreview(screenshotItem shot)
        {
            this.WindowState = FormWindowState.Normal;
            MemoryStream previewMem = new MemoryStream(reader.readFile(shot));
            Bitmap preview = new Bitmap(previewMem);
            previewMem.Close();
            previewMem.Dispose();

            this.Text = "Previewing \"" + shot.shotTitle + "\"";
            pictureBox1.Image = preview;
            this.ClientSize = new Size(preview.Width / 2, preview.Height / 2);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
