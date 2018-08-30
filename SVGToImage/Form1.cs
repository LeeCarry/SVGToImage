using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Svg;

namespace SVGToImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            //选择输入的文件夹
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description="请选择SVG文件的目录";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbInput.Text = fbd.SelectedPath;
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            //选择输入的文件夹
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "请选择保存图片的目录";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbOutput.Text = fbd.SelectedPath;
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            //清空文本，不能给text赋值，否则颜色设置会出现问题
            rtbInfo.Clear();
            //检测路径合法性
            if (!Directory.Exists(tbInput.Text)||!Directory.Exists(tbOutput.Text))
            {
                rtbInfo.SelectionColor = Color.Red;
                rtbInfo.AppendText("输入路径或输出路径不存在\n");
                return;

            }
            //准备转换工作
            btnBegin.Enabled = false;
            rtbInfo.SelectionColor = Color.Green;
            rtbInfo.AppendText("开始转换,如无特殊需要请勿修改参数...\n");
            string[] svgs = Directory.GetFiles(tbInput.Text, "*.svg");
            rtbInfo.SelectionColor = Color.Black;
            rtbInfo.AppendText("共读取到" + svgs.Length + "个svg文件\n");
            int successCount = 0;
            //开始每一次转换
            for(int i=0;i<svgs.Length;i++)
            {
                Bitmap bitmap = new Bitmap((int)nudWidth.Value,(int)nudHeight.Value);
                Graphics graphics = Graphics.FromImage(bitmap);
                //可能会出现路径问题或其他错误，记录成功个数
                try
                {
                    SvgDocument svgDocument = SvgDocument.Open(svgs[i]);
                    ISvgRenderer renderer = SvgRenderer.FromGraphics(graphics);
                    svgDocument.Width = (int)nudWidth.Value;
                    svgDocument.Height = (int)nudHeight.Value;
                    svgDocument.Draw(renderer);
                    bitmap.Save(tbOutput.Text+"\\"+Path.GetFileNameWithoutExtension(svgs[i]) + "." + cbFormat.Text);//怕读取顺序与原顺序不一样，所以按照原名字保存
                    successCount++;
                    rtbInfo.AppendText("第" + (i + 1) + "个转换成功\n");
                }
                catch
                {
                    rtbInfo.SelectionColor = Color.Red;
                    rtbInfo.AppendText("第" + (i + 1) + "个转换失败");
                    rtbInfo.SelectionColor = Color.Black;
                }
            }
            //转换完成，完成善后工作
            rtbInfo.SelectionColor = Color.Green;
            rtbInfo.AppendText("转换完成，共" + svgs.Length + "个文件，" + successCount + "个转换成功");
            btnBegin.Enabled = true;
        }
    }
}
