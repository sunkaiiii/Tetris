using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace Tetris
{
    class Pelette
    {
        private int _width = 15;//画板的宽度
        private int _heihgt = 25;//画板高度
        private Color[,] coorArr;//固定砖块数组
        private Color disapperColor;
        private Graphics gpPalette;//砖块活动画版
        private Graphics gpReady;//下一个砖块样式画板
        private Block runBlock;//正在活动的画板
        private Block readyBlock;//下一个砖块
        private int recPix;//单元格像素

        private System.Timers.Timer timerBlock;//定时器
        private int timeSpan = 800;

        public Pelette(int x,int y,int pix,Color dColor,Graphics gp,Graphics gr)
        {
            _width = x;
            _heihgt = y;
            coorArr = new Color[_width, _heihgt];
            disapperColor = dColor;
            gpPalette = gp;
            gpReady = gr;
            recPix = pix;
        }
        public void Start()//游戏开始
        {
            BlockGroup bGroup = new BlockGroup();
            runBlock = bGroup.GetABlock();
            runBlock.XPos = _width / 2;
            int y = 0;
            for(int i=0;i<runBlock.Length;i++)//寻找Y的最大值
            {
                if(runBlock[i].Y>y)
                {
                    y = runBlock[i].Y;
                }
            }
            runBlock.YPos = y;
            gpPalette.Clear(disapperColor);//清空画板
            runBlock.Paint(gpPalette);
            Thread.Sleep(20);
            readyBlock = bGroup.GetABlock();//去另一个砖块赋给readyBlock
            readyBlock.XPos = 4;//5*5的矩阵中心点为2
            readyBlock.YPos = 3;
            gpReady.Clear(disapperColor);
            readyBlock.Paint(gpReady);

            timerBlock = new System.Timers.Timer(timeSpan);
            timerBlock.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timerBlock.AutoReset = true;
            timerBlock.Start();
        }
        private void OnTimedEvent(object source,System.Timers.ElapsedEventArgs e)
        {
            CheckAndOverBlock();
            Down();
        }
        public bool Down()
        {
            int xPos = runBlock.XPos;
            int yPos = runBlock.YPos + 1;
            for(int i=0;i<runBlock.Length;i++)
            {
                if (yPos - runBlock[i].Y > _heihgt - 1)//如果超出下界则 失败
                    return false;
                if (!coorArr[xPos + runBlock[i].X, yPos - runBlock[i].Y].IsEmpty) //如果下边有东西则失败
                    return false;
            }
            runBlock.erase(gpPalette);
            runBlock.YPos++;
            runBlock.Paint(gpPalette);
            return true;
        }
        public void Drop()//丢下砖块
        {
            timerBlock.Stop();
            while (Down()) ;
            timerBlock.Start();
        }
        public void MoveLeft()//向左移动一个单元格
        {
            int xPos = runBlock.XPos - 1;
            int yPos = runBlock.YPos;
            for(int i=0;i<runBlock.Length;i++)
            {
                if(xPos+runBlock[i].X<0)//如果超出边界，则失败
                {
                    return;
                }
                if (!coorArr[xPos + runBlock[i].X, yPos - runBlock[i].Y].IsEmpty)//如果左边有东西则失败
                    return;
            }
            runBlock.erase(gpPalette);
            runBlock.XPos--;
            runBlock.Paint(gpPalette);
        }
        public void MoveRight()//向右移动一个单元格
        {
            int xPos = runBlock.XPos + 1;
            int yPos = runBlock.YPos;
            for(int i=0;i<runBlock.Length;i++)
            {
                if (xPos + runBlock[i].X > _width - 1)
                    return;
                if (!coorArr[xPos + runBlock[i].X, yPos - runBlock[i].Y].IsEmpty)//如果右边有东西则失败
                    return;
            }
            runBlock.erase(gpPalette);
            runBlock.XPos++;
            runBlock.Paint(gpPalette);
            
        }
        public void DeasilRotate()//顺时针旋转
        {
            for(int i=0;i<runBlock.Length;i++)
            {
                int x = runBlock.XPos + runBlock[i].Y;
                int y = runBlock.YPos + runBlock[i].X;
                if (x < 0 || x > _width - 1)//如果超出左右边界，则旋转失败
                    return;
                if (y < 0 || y > _heihgt - 1)//如果超出上下边界，则旋转失败
                    return;
            }
            runBlock.erase(gpPalette);
            runBlock.DeasilRotate();
            runBlock.Paint(gpPalette);
        }
        public void ContraRotate()//逆时针旋转
        {
            for(int i=0;i<runBlock.Length;i++)
            {
                int x = runBlock.XPos - runBlock[i].Y;
                int y = runBlock.YPos - runBlock[i].X;
                if (x < 0 || x > _width - 1)
                    return;
                if (y < 0 || y > _heihgt - 1)
                    return;
            }
            runBlock.erase(gpPalette);
            runBlock.ContraRotate();
            runBlock.Paint(gpPalette);
        }
        private void PaintBackGround(Graphics gp)//重画画板的背景
        {
            gp.Clear(Color.Black);
            for(int i=0;i<_heihgt;i++)
            {
                for(int j=0;j<_width;j++)
                {
                    if(!coorArr[j,i].IsEmpty)
                    {
                        SolidBrush sb = new SolidBrush(coorArr[j, i]);
                        gp.FillRectangle(sb, j * recPix + 1, i * recPix + 1, recPix - 2, recPix - 2);
                    }
                }
            }
        }
        public void PaintPalette(Graphics gp)//重画整个画板
        {
            PaintBackGround(gp);//先画背景
            if(runBlock!=null)//再画活动的砖块
            {
                runBlock.Paint(gp);
            }
        }
        public void PaintReady(Graphics gp)//重画下一个砖块
        {
            if(readyBlock!=null)
            {
                readyBlock.Paint(gp);
            }
        }
        public void CheckAndOverBlock()//检查砖块是否到底，如果到底的话则把它当做固定的砖块coorarr，产生新的砖块
        {
            bool over = false;//设置一个当前运行砖块是否到底的标志
            for(int i=0;i<runBlock.Length;i++)
            {
                int x = runBlock.XPos + runBlock[i].X;
                int y = runBlock.YPos - runBlock[i].Y;
                if(y==_heihgt-1)//如果到底，则当前砖块结束
                {
                    over = true;
                    break;
                }
                if(!coorArr[x,y+1].IsEmpty)//如果下面有其它砖块，则当前砖块结束
                {
                    over = true;
                    break;
                }
            }
            if(over)
            {
                for(int i=0;i<runBlock.Length;i++)//把当前砖块归入coordinateArr
                {
                    coorArr[runBlock.XPos + runBlock[i].X, runBlock.YPos - runBlock[i].Y] = runBlock.BlockColor;
                }
                //检查是否有满行的情况，如果有，则去掉满行
                CheckAndDelFullRow();
                //产生新的砖块
                runBlock = readyBlock;//新的砖块为准备好的砖块
                runBlock.XPos = _width / 2;//确定当前运行砖块的出生位置
                int y = 0;
                for(int i=0;i<runBlock.Length;i++)
                {
                    if(runBlock[i].Y> y)
                    {
                        y = runBlock[i].Y;
                    }
                }
                runBlock.YPos = y;
                //检查新产生的砖块所占用的地方是否已经有砖块，如果有，则游戏结束
                for(int i=1;i<runBlock.Length;i++)
                {
                    if(!coorArr[runBlock.XPos+runBlock[i].X,runBlock.YPos-runBlock[i].Y].IsEmpty)
                    {
                        //游戏结束
                        StringFormat drawFormat = new StringFormat();
                        drawFormat.Alignment = StringAlignment.Center;
                        gpPalette.DrawString("GAME OVER", new Font("Arial Black", 25f), new SolidBrush(Color.White), new RectangleF(0, _heihgt * recPix / 2 - 100, _width * recPix, 100), drawFormat);
                        timerBlock.Stop();//关闭定时器                     
                        return;
                    }
                }
                runBlock.Paint(gpPalette);
                //获取新的准备砖块
                BlockGroup bGroup = new BlockGroup();
                readyBlock = bGroup.GetABlock();
                readyBlock.XPos = 2;
                readyBlock.YPos = 2;
                gpReady.Clear(Color.Black);
                readyBlock.Paint(gpReady);
            }
        }
        private void CheckAndDelFullRow()//检查并删除满行
        {
            int lowRow = runBlock.YPos - runBlock[0].Y;//LowRow代表当前砖块的Y轴的最小值
            int highRor = lowRow;//highRow代表当前砖块的y轴的最大值
            for (int i = 1; i < runBlock.Length;i++)
            {
                int y = runBlock.YPos - runBlock[i].Y;
                if(y<lowRow)
                {
                    lowRow = y;
                }
                if(y>highRor)
                {
                    highRor = y;
                }
            }
            bool repaint = false;//判断是否重画的标志
            for(int i=lowRow;i<=highRor;i++)
            {
                bool rowFull = true;
                for(int j=0;j<_width;j++)
                {
                    if(coorArr[j,i].IsEmpty)
                    {
                        rowFull = false;
                        break;
                    }
                }
                if(rowFull)//如果满行，则删除这一行
                {
                    repaint = true;
                    for(int k= i;k>0;k--)
                    {
                        for(int j=0;j<_width;j++)
                        {
                            coorArr[j, k] = coorArr[j, k - 1];
                        }
                    }
                    for(int j=0;j<_width;j++)//清空第0行
                    {
                        coorArr[j, 0] = Color.Empty;
                    }
                }
            }
            if(repaint)//重画
            {
                PaintBackGround(gpPalette);
            }
        }
        public void Pause()//暂停
        {
            if(timerBlock.Enabled==true)
            {
                timerBlock.Enabled = false;
            }
        }
        public void EndPause()//结束暂停
        {
            if(timerBlock.Enabled==false)
            {
                timerBlock.Enabled = true;
            }
        }
        public void Close()//关闭
        {
            timerBlock.Close();//关闭定时器
            gpPalette.Dispose();//释放两个画布
            gpReady.Dispose();
        }
    }
}
