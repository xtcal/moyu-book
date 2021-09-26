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
using System.IO;

namespace MoyuBook {
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class BooksList : Window {
        static BooksList self;
        public static BooksList Self {
            get {
                if (self == null) {
                    self = new BooksList();
                }
                return self;
            }
        }
        public BooksList () {
            InitializeComponent();
            if (!Directory.Exists(GlobalData.Self.Config.DirUrl)) {
                Directory.CreateDirectory(GlobalData.Self.Config.DirUrl);
            }
            listView1.ItemsSource = GlobalData.Self.AllBookInfos;
        }

        private void Button_Click (object sender , RoutedEventArgs e) {
            BookInfo book = (BookInfo)listView1.SelectedItem;
            if (book != null) {
                GlobalData.Self.Config.LastBook = book.Name;
                GlobalData.Self.CurText = "";
            }
        }

        private void Button_Click_1 (object sender , RoutedEventArgs e) {
            BookInfo book = (BookInfo)listView1.SelectedItem;
            if (book != null) {
                BookMenus bookMenus = new BookMenus(book);
                bookMenus.Show();
                Close();
            }
        }
    }
}