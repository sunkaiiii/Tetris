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
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();
        }
        private Config config = new Config();

        private void lblMode_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            gp.Clear(Color.Black);
            Pen p = new Pen(Color.White);
            //SolidBrush s = new SolidBrush(Color.White);
            for (int i = 30; i < 155; i = i + 31)//画横白线
                gp.DrawLine(p, 1, i, 155, i);
            for (int i = 30; i < 155; i = i + 31) //画竖白线
                gp.DrawLine(p, i, 1, i, 155);
            SolidBrush s = new SolidBrush(blockColor);
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    if(struArr[x,y])
                        gp.FillRectangle(s, 31 * x, 31 * y, 30, 30);
                }
            }
        }
        private bool[,] struArr = new bool[5, 5];
        private Color blockColor = Color.Red;
        private void lblMode_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int xPos, yPos;
            xPos = e.X / 31;//数组的第一个下标和第二个下标
            yPos = e.Y / 31;
            struArr[xPos, yPos] = !struArr[xPos, yPos];
            bool b = struArr[xPos, yPos];
            Graphics gp = lblMode.CreateGraphics();
            SolidBrush s = new SolidBrush(b ? blockColor : Color.Black);
            gp.FillRectangle(s, 31 * xPos , 31 * yPos, 30, 30);
            gp.Dispose();
        }

        private void lblColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            blockColor = colorDialog1.Color;
            lblColor.BackColor = colorDialog1.Color;
            lblMode.Invalidate();//使得lblMode重画， 执行了她的Paint事件
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool isEmpty = false;//首先查找图案是否为空
            foreach(bool i in struArr)
            {
                if(i)
                {
                    isEmpty = true;
                    break;
                }
            }
            if(!isEmpty)
            {
                MessageBox.Show("图案为空，请先用鼠标点击左边窗口绘制图案！","提示窗口",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            StringBuilder sb = new StringBuilder(25);
            foreach(bool i in struArr)
            {
                sb.Append(i ? "1" : "0");
            }
            string blockString = sb.ToString();
            //在检查是否重复图案
            foreach(ListViewItem item in lsvBlockSet.Items)
            {
                if(item.SubItems[0].Text==blockString)
                {
                    MessageBox.Show("该图案已经存在！", "提示窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            ListViewItem myItem = new ListViewItem();
            myItem = lsvBlockSet.Items.Add(blockString);
            myItem.SubItems.Add(Convert.ToString(blockColor.ToArgb()));
        }

        private void lsvBlockSet_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(e.IsSelected)
            {
                blockColor = Color.FromArgb(int.Parse(e.Item.SubItems[1].Text));
                lblColor.BackColor = blockColor;
                string s = e.Item.SubItems[0].Text;
                for(int i=0;i<s.Length;i++)
                {
                    struArr[i / 5, i % 5] = (s[i] == '1') ? true:false;
                }
                lblMode.Invalidate();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if(lsvBlockSet.SelectedItems.Count==0)
            {
                MessageBox.Show("请在右边窗口选择一个条目进行删除！", "提示窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            lsvBlockSet.Items.Remove(lsvBlockSet.SelectedItems[0]);
            btnClear.PerformClick();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for(int x=0;x<5;x++)//把struArr内所有的元素置为false，实现清空
            {
                for(int y=0;y<5;y++)
                {
                    struArr[x, y] = false;
                }
            }
            lblMode.Invalidate();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lsvBlockSet.SelectedItems.Count==0)//判断有没有被选中
            {
                MessageBox.Show("请在右边窗口选择一个条目进行修改！", "提示窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            bool isEmpty = false;//判断图案是否为空
            foreach(bool i in struArr)
            {
                if(i)
                {
                    isEmpty = true;
                    break;
                }
            }
            if(!isEmpty)
            {
                MessageBox.Show("图案为空，请先用鼠标点击左边窗口绘制窗口图案在进行修改", "提示窗口", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            StringBuilder sb = new StringBuilder(25);
            foreach (bool i in struArr)
            {
                sb.Append(i ? "1" : "0");
            }
            lsvBlockSet.SelectedItems[0].SubItems[0].Text = sb.ToString();
            lsvBlockSet.SelectedItems[0].SubItems[1].Text = Convert.ToString(blockColor.ToArgb());
        }

        private void txtContra_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.KeyValue>=33&&e.KeyValue<=36)||(e.KeyValue>=45&&e.KeyValue<=46)||(e.KeyValue>=48&&e.KeyValue<=57)||(e.KeyValue>=65&&e.KeyValue<=90)||(e.KeyValue>=96&&e.KeyValue<=107)||(e.KeyValue>=109&&e.KeyValue<=111)||(e.KeyValue>=219&&e.KeyValue<=222))
            {
                //检查是否有冲突的快捷键
                foreach(Control c in gbKeySet.Controls)
                {
                    Control tempC = c as TextBox;
                    if(tempC!= null&&((TextBox)tempC).Text!="")
                    {
                        if(((int)((TextBox)tempC).Tag)==e.KeyValue)
                        {
                            ((TextBox)tempC).Text = "";
                            ((TextBox)tempC).Tag = Keys.None;
                        }
                    }
                }
                ((TextBox)sender).Text = e.KeyCode.ToString();
                ((TextBox)sender).Tag = (Keys)e.KeyValue;
            }
        }

        private void lblBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            lblBackColor.BackColor = colorDialog1.Color;
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            config.LoadFromXmlFile();
            InfoArr info = config.Info;
            //读取砖块样式
            ListViewItem myItem = new ListViewItem();
            for(int i=0;i<info.Length;i++)
            {
                myItem = lsvBlockSet.Items.Add(info[i].GetIdStr());
                myItem.SubItems.Add(info[i].GetColorStr());
            }
            //读快捷键
            txtDown.Text = config.DownKey.ToString();
            txtDown.Tag = config.DownKey;
            txtDrop.Text = config.DropKey.ToString();
            txtDrop.Tag = config.DropKey;
            txtLeft.Text = config.MoveLeftKey.ToString();
            txtLeft.Tag = config.MoveLeftKey;
            txtRight.Text = config.MoveRightKey.ToString();
            txtRight.Tag = config.MoveRightKey;
            txtDeasil.Text = config.DeasilRotateKey.ToString();
            txtDeasil.Tag = config.DeasilRotateKey;
            txtContra.Text = config.ContraRotateKey.ToString();
            txtContra.Tag = config.ContraRotateKey;
            //读取环境参数设置
            txtCoorWidth.Text = config.CoorWidth.ToString();
            txtCoorHeight.Text = config.CoorHeight.ToString();
            txtRectPix.Text = config.RectPix.ToString();
            lblBackColor.BackColor = config.BackColor;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                InfoArr info = new InfoArr();
                foreach (ListViewItem item in lsvBlockSet.Items)//从lsvBlockSet内读取砖块信息，并存入info内
                {
                    info.Add(item.SubItems[0].Text, item.SubItems[1].Text);
                }
                config.Info = info;//把info赋给config对象的ifno属性
                config.DownKey = (Keys)txtDown.Tag;
                config.DropKey = (Keys)txtDrop.Tag;
                config.MoveLeftKey = (Keys)txtLeft.Tag;
                config.MoveRightKey = (Keys)txtRight.Tag;
                config.DeasilRotateKey = (Keys)txtDeasil.Tag;
                config.ContraRotateKey = (Keys)txtContra.Tag;
                config.CoorWidth = int.Parse(txtCoorWidth.Text);
                config.CoorHeight = int.Parse(txtCoorHeight.Text);
                config.RectPix = int.Parse(txtRectPix.Text);
                config.BackColor = lblBackColor.BackColor;
                config.SaveToXmlFile();//保存成xml文件
                MessageBox.Show("保存成功");
            }
            catch
            {
                MessageBox.Show("保存失败");
            }
          
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("小凯哥是猪");
        }
    }
}
