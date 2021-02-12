using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextToSpeech
{
    /// <summary>
    /// RecordControl.xaml 的交互逻辑
    /// </summary>
    public partial class RecordControl : UserControl
    {
        public RecordControl()
        {
            InitializeComponent();


            LoadButton.Click += BrowseButtonClickCallback;

        }


        //有点意思，我需要写一个异步的读取QQBase64位加密的轮子
        


        private void BrowseButtonClickCallback(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog data = new System.Windows.Forms.OpenFileDialog();

            data.Filter = "(*.mht)|*.mht|All files (*.*)|*.*";

            //   data.ShowDialog();
            if (data.ShowDialog() == System.Windows.Forms.DialogResult.OK)//用于指示让文件夹展开对文件选中才允许运行。
            {
                var T = data.OpenFile();

               // WebBrowser

                web.NavigateToStream(data.OpenFile());

                
                //StreamReader streamReader = new StreamReader(data.OpenFile(), Encoding.UTF8);
                //string tempData = streamReader.ReadToEnd();
                //LogPath = TextBox.Text = data.FileName;//取得名称并输入到界面上。

                TextBox.Text = data.FileName;

                //#endregion

                //TextDispose(tempData);
            }



            data.Dispose();

            // 是否满足生成条件();
        }

        public void 读取(object sender, RoutedEventArgs e)
        {


            //web
        }

    }
}
