using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Collections;
namespace Tetris
{
    class BlockInfo
    {
        private BitArray _id;//存储砖块样式信息
        private Color _bColor;//存储颜色信息
        public BlockInfo(BitArray id,Color bColor)//构造函数，给私有成员变量赋值
        {
            _id = id;
            _bColor = bColor;
        }
        public BitArray ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public Color BColor
        {
            get { return _bColor; }
            set { _bColor = value; }
        }
        public string GetIdStr()
        {
            StringBuilder s = new StringBuilder(25);
            foreach (bool b in _id)
            {
                s.Append(b ? "1" : "0");
            }
            return s.ToString();
        }
        public string GetColorStr()
        {
            return Convert.ToString(_bColor.ToArgb());
        }
    }
}
