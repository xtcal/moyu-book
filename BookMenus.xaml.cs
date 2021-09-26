using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MoyuBook {
    /// <summary>
    /// BookMenus.xaml 的交互逻辑
    /// </summary>
    public partial class BookMenus : Window {
        BookInfo info = null;

        public BookMenus (BookInfo _info = null) {
            InitializeComponent();
            info = _info;
            Init();
        }

        public void Init () {
            this.Title = info.Name;
            this.listView.ItemsSource = info.Pages;
        }

        private void Button_Click (object sender , RoutedEventArgs e) {

        }

        private void listView_MouseDoubleClick (object sender , MouseButtonEventArgs e) {
            int pageIdx = listView.SelectedIndex;
            if (pageIdx != -1) {
                BookConfigInfo configInfo = GlobalData.Self.Config.GetBookConfig(info.Name);
                GlobalData.Self.Config.LastBook = info.Name;
                configInfo.LastPage = pageIdx;
                configInfo.LastLine = 0;
                GlobalData.Self.CurText = "";
                Close();
            }
        }
    }
}