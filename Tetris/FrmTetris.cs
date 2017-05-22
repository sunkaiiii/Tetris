using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class FrmTetris : Form
    {
        public FrmTetris()
        {
            InitializeComponent();
        }
        private Pelette p;
        private Keys downKey;
        private Keys dropKey;
        private Keys moveLeftKey;
        private Keys moveRightKey;
        private Keys deasilRotateKey;
        private Keys contraROtateKey;
        private int pletterWidth;
        private int paletterHeight;
        private Color platteColor;
        private int rectPix;

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (p != null)
                p.Close();
            p = new Pelette(pletterWidth, paletterHeight, rectPix, platteColor, Graphics.FromHwnd(pbRun.Handle), Graphics.FromHwnd(lblReady.Handle));
            p.Start();
        }


        private void btnSetting_Click(object sender, EventArgs e)
        {
            if(btnPause.Text=="暂停")
            {
                btnPause.PerformClick();
            }
            using (FrmConfig frmConfig = new FrmConfig())
            {
                frmConfig.ShowDialog();
            }
        }

        private void pbRun_Paint(object sender, PaintEventArgs e)
        {
            if (p != null)
            {
                p.PaintPalette(e.Graphics);
            }
        }

        private void lblReady_Paint(object sender, PaintEventArgs e)
        {
            if(p!=null)
            {
                p.PaintReady(e.Graphics);
            }
        }

        private void FrmTetris_Load(object sender, EventArgs e)
        {
            //读取xml文件中的参数配置信息
            Config config = new Config();
            config.LoadFromXmlFile();
            downKey = config.DownKey;
            dropKey = config.DropKey;
            moveLeftKey = config.MoveLeftKey;
            moveRightKey = config.MoveRightKey;
            deasilRotateKey = config.DeasilRotateKey;
            contraROtateKey = config.ContraRotateKey;
            paletterHeight = config.CoorHeight;
            pletterWidth = config.CoorWidth;
            platteColor = config.BackColor;
            rectPix = config.RectPix;
            this.Width = pletterWidth * rectPix + 200;
            this.Height = paletterHeight * rectPix + 58;
            pbRun.Width = pletterWidth * rectPix;
            pbRun.Height = paletterHeight * rectPix;
        }

        private void FrmTetris_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 32)//屏蔽回车键
                e.Handled = true;
            if (e.KeyCode == downKey)
                p.Down();
            else if (e.KeyCode == dropKey)
                p.Drop();
            else if (e.KeyCode == moveLeftKey)
                p.MoveLeft();
            else if (e.KeyCode == moveRightKey)
                p.MoveRight();
            else if (e.KeyCode == Keys.NumPad3)
                p.DeasilRotate();
            else if (e.KeyCode == Keys.NumPad1)
                p.ContraRotate();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if(p==null)
            {
                return;
            }
            if(btnPause.Text=="暂停")
            {
                p.Pause();
                btnPause.Text = "继续";
            }
            else
            {
                p.EndPause();
                btnPause.Text = "暂停";
            }
        }

        private void FrmTetris_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (p != null)
                p.Close();
        }
    }
}