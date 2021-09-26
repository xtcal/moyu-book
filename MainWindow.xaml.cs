using GlobalHotKey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

//wpf设置字体颜色渐变和字体阴影 (样式)
//https://www.cnblogs.com/changbaishan/p/4358341.html
//WPF知识点--渐变色
//https://www.cnblogs.com/kuangxiangnice/p/5820631.html

namespace MoyuBook {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow () {
            LoadDll();
            InitializeComponent();
            Init();
        }

        private void LoadDll() {
            //var files = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            //foreach (var file in files) {
            //    MessageBox.Show(file);
            //}
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args) {
            var requestedAssemblyName = new AssemblyName(args.Name);
            string resourceName = "MoyuBook.plugin." + requestedAssemblyName.Name + ".dll";
            try {
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    return Assembly.Load(buffer);
                }
            } catch (Exception ex) {
                //System.Windows.Forms.MessageBox.Show(String.Format(@"{0}\{1}", ex.Message, ex.StackTrace));
                throw;
            }
        }
        public void InitNIcon () {
            //托盘初始化
            System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
            nIcon.Icon = MoyuBook.Properties.Resources.header;
            nIcon.Visible = true;
            nIcon.Text = "Check for updates";
            nIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            nIcon.ContextMenu.MenuItems.Add("退出" , (e , o) => {
                this.Close();
            });
            {
                System.Windows.Forms.MenuItem item = new System.Windows.Forms.MenuItem();
                item.Text = "置顶";
                item.Checked = this.Topmost;
                item.Click += (e , o) => {
                    item.Checked = !item.Checked;
                    this.Topmost = item.Checked;
                };
                nIcon.ContextMenu.MenuItems.Add(item);
            }
            nIcon.DoubleClick += (e , o) => {
                MessageBox.Show("1");
            };
        }

        public void InitAllBooks () {
            if (!Directory.Exists(GlobalData.Self.Config.DirUrl)) {
                return;
            }
            var files = Directory.GetFiles(GlobalData.Self.Config.DirUrl);
            foreach (var file in files) {
                string ext = System.IO.Path.GetExtension(file).ToLower();
                if (!ext.Equals(".txt")) {
                    continue;
                }
                BookInfo bookInfo = new BookInfo();
                bookInfo.Name = System.IO.Path.GetFileNameWithoutExtension(file);
                string[] chapter = Regex.Split(File.ReadAllText(file) , "------------");    //章节
                List<List<string>> list = new List<List<string>>();
                for (int i = 0 ; i < chapter.Length ; i++) {
                    List<string> ret = new List<string>();
                    foreach (var item in chapter[i].Split('\n')) {
                        string str = item.Trim();
                        if (!str.Equals(""))
                            ret.Add(str);
                    }
                    if (ret.Count > 0)
                        list.Add(ret);
                }
                for (int i = 0 ; i < list.Count ; i++) {
                    PageInfo pageInfo = new PageInfo();
                    for (int j = 0 ; j < list[i].Count ; j++)
                        pageInfo.Lines.Add(list[i][j]);
                    bookInfo.Pages.Add(pageInfo);
                }
                GlobalData.Self.AllBookInfos.Add(bookInfo);
            }
        }

        public void ItemClickEvent (object sender , RoutedEventArgs e) {
            MenuItem mitem = (MenuItem)sender;
            object header = mitem.Header;
            if (header.Equals("置顶")) {
                mitem.IsChecked = !mitem.IsChecked;
                this.Topmost = mitem.IsChecked;
            } else {
                if (this.Topmost) {
                    this.Topmost = false;
                    foreach (MenuItem item in this.ContextMenu.Items) {
                        if (item.Header.Equals("置顶")) {
                            item.IsChecked = false;
                            break;
                        }
                    }
                }
                if (header.Equals("锁定")) {
                    double opacityv = GlobalData.Self.Config.BackgroundOpacity;
                    if (opacityv == 0)
                        GlobalData.Self.Config.BackgroundOpacity = 0.1f;
                    else
                        GlobalData.Self.Config.BackgroundOpacity = 0f;
                    mitem.IsChecked = GlobalData.Self.Config.BackgroundOpacity == 0f;
                } else if (header.Equals("书库")) {
                    BooksList bookList = new BooksList();
                    bookList.Show();
                } else if (header.Equals("设置")) {
                    Setting setting = new Setting();
                    setting.Show();
                } else if (header.Equals("退出")) {
                    this.Close();
                }
            }
        }

        public void InitContextMenu () {
            this.ContextMenu = new ContextMenu();
            {
                MenuItem mitem = new MenuItem();
                mitem.Header = "置顶";
                mitem.IsChecked = this.Topmost;
                mitem.Click += ItemClickEvent;
                this.ContextMenu.Items.Add(mitem);
            }
            {
                MenuItem mitem = new MenuItem();
                mitem.Header = "锁定";
                mitem.IsChecked = GlobalData.Self.Config.BackgroundOpacity == 0f;
                mitem.Click += ItemClickEvent;
                this.ContextMenu.Items.Add(mitem);
            }
            {
                MenuItem mitem = new MenuItem();
                mitem.Header = "书库";
                mitem.Click += ItemClickEvent;
                this.ContextMenu.Items.Add(mitem);
            }
            {
                MenuItem mitem = new MenuItem();
                mitem.Header = "设置";
                mitem.Click += ItemClickEvent;
                this.ContextMenu.Items.Add(mitem);
            }
            {
                MenuItem mitem = new MenuItem();
                mitem.Header = "退出";
                mitem.Click += ItemClickEvent;
                this.ContextMenu.Items.Add(mitem);
            }
        }
        public void InitGlobalHotKey () {
            HotKeyManager _hotKeyManager = new HotKeyManager();
            _hotKeyManager.Register(Key.OemPeriod, ModifierKeys.Windows);
            _hotKeyManager.Register(Key.OemComma , ModifierKeys.Windows);
            _hotKeyManager.Register(Key.D1 , ModifierKeys.Alt);
            _hotKeyManager.KeyPressed += (e , o) => {
                if (o.HotKey.Key == Key.OemComma) { // win + ,
                    BookInfo info = GlobalData.Self.CurBook;
                    BookConfigInfo configInfo = GlobalData.Self.Config.GetBookConfig(info.Name);
                    configInfo.LastLine--;
                    if (configInfo.LastLine < 0) {
                        configInfo.LastPage--;
                        configInfo.LastLine = info.Pages[configInfo.LastPage].LinesCount - 1;
                    }
                    GlobalData.Self.CurText = "";
                }
                if (o.HotKey.Key == Key.OemPeriod) { // win + .
                    BookInfo info = GlobalData.Self.CurBook;
                    BookConfigInfo configInfo = GlobalData.Self.Config.GetBookConfig(info.Name);
                    configInfo.LastLine++;
                    if (configInfo.LastLine >= info.Pages[configInfo.LastPage].LinesCount) {
                        configInfo.LastPage++;
                        configInfo.LastLine = 0;
                    }
                    GlobalData.Self.CurText = "";
                }
                if (o.HotKey.Key == Key.D1) {   // alt + 1
                    GlobalData.Self.IsHide = !GlobalData.Self.IsHide;
                    GlobalData.Self.CurText = "";
                }
            };
        }

        public void Init () {
            string configFile = "Config.json";
            if (File.Exists(configFile)) {
                GlobalData.Self.Config = JsonConvert.DeserializeObject<MoyuConfig>(File.ReadAllText(configFile));
            }

            InitAllBooks();
            InitNIcon();
            InitContextMenu();
            //InitGlobalHotKey();

            //读取配置文件
            try {
                //设置位置、大小
                Rect rect = GlobalData.Self.Config.LastWindowPos;
                this.Left = rect.Left;
                this.Top = rect.Top;
                this.Width = rect.Width;
                this.Height = rect.Height;
            } catch { }

            this.txt_curTxt.DataContext = GlobalData.Self;
            this.txt_curTxt.IsReadOnly = true;
            this.txt_curTxt.IsTabStop = false;
            this.txt_curTxt.SelectionBrush = new SolidColorBrush(Colors.Gray);
            this.txt_curTxt.SelectionOpacity = 0.5;
            this.txt_curTxt.TextAlignment = TextAlignment.Left;
            this.txt_curTxt.VerticalContentAlignment = VerticalAlignment.Center;
            this.DataContext = GlobalData.Self;
        }

        private void Window_MouseLeftButtonDown_1 (object sender , MouseButtonEventArgs e) {
            try { this.DragMove(); } catch { }
        }

        private void txt_curTxt_MouseDown (object sender , MouseButtonEventArgs e) {
            try { this.DragMove(); } catch { }
        }

        private void Window_Closed (object sender , EventArgs e) {
            try {
                GlobalData.Self.Config.LastWindowPos = new Rect(this.Left , this.Top , this.Width , this.Height);
                File.WriteAllText("Config.json" , GlobalData.Self.ToJson());
            } catch (Exception) {
                throw;
            }
        }
    }
}