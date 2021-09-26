using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoyuBook {
    public class GlobalData : INotifyPropertyChanged {
        private GlobalData() { }
        static private GlobalData _self;
        static public GlobalData Self {
            get {
                if (_self == null) _self = new GlobalData();
                return _self;
            }
        }
        private MoyuConfig _config = new MoyuConfig();
        public MoyuConfig Config {
            get { return _config; }
            set { _config = value; NotifyPropertyChanged(); }
        }

        private bool _isHide = false;
        public bool IsHide {
            get { return _isHide; }
            set { _isHide = value; NotifyPropertyChanged(); }
        }

        public string CurText {
            set {
                NotifyPropertyChanged();
            }
            get {
                if (CurBook != null && !IsHide) {
                    BookConfigInfo config = Config.GetBookConfig(CurBook.Name);
                    if (config == null) {
                        config = new BookConfigInfo();
                        Config.AllBookConfigInfos.Add(CurBook.Name, config);
                    }
                    return CurBook.Pages[config.LastPage].Lines[config.LastLine];
                }
                return "......";
            }
        }

        public BookInfo CurBook {
            get {
                BookInfo info = GetBook(Config.LastBook);
                if (info != null) {
                    return info;
                }
                return null;
            }
        }

        public BookInfo GetBook(string _name) => AllBookInfos.FirstOrDefault((n) => n.Name == _name);

        private List<BookInfo> allBookInfos = new List<BookInfo>();
        public List<BookInfo> AllBookInfos {
            get { return allBookInfos; }
            set { allBookInfos = value; NotifyPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string info = "默认值") {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

    public class MoyuConfig : INotifyPropertyChanged {
        private string dirUrl = "./books";
        public string DirUrl {
            get { return dirUrl; }
            set { dirUrl = value; NotifyPropertyChanged(); }
        }

        private int fontSize = 18;
        public int FontSize {
            get { return fontSize; }
            set { fontSize = value; NotifyPropertyChanged(); }
        }

        private double backgroundOpacity = 0.3;
        public double BackgroundOpacity {
            get { return backgroundOpacity; }
            set { backgroundOpacity = value; NotifyPropertyChanged(); }
        }

        private string backgroundColor = "#000000";
        public string BackgroundColor {
            get { return backgroundColor; }
            set { backgroundColor = value; NotifyPropertyChanged(); }
        }

        private string fontColor = "#ffffff";
        public string FontColor {
            get { return fontColor; }
            set { fontColor = value; NotifyPropertyChanged(); }
        }

        private string lastBook = "斗";
        public string LastBook {
            get { return lastBook; }
            set { lastBook = value; NotifyPropertyChanged(); }
        }

        private Rect lastWindowPos = new Rect(0, 0, 800, 130);
        public Rect LastWindowPos {
            get { return lastWindowPos; }
            set { lastWindowPos = value; NotifyPropertyChanged(); }
        }

        public BookConfigInfo GetBookConfig(string _name) {
            if (!AllBookConfigInfos.ContainsKey(_name)) {
                AllBookConfigInfos.Add(_name, new BookConfigInfo());
            }
            return AllBookConfigInfos[_name];
        }

        private Dictionary<string, BookConfigInfo> allBookConfigInfos = new Dictionary<string, BookConfigInfo>();
        public Dictionary<string, BookConfigInfo> AllBookConfigInfos {
            get { return allBookConfigInfos; }
            set { allBookConfigInfos = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string info = "默认值") {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
    public class BookConfigInfo : INotifyPropertyChanged {
        private int lastPage = 0;
        public int LastPage {
            get { return lastPage; }
            set { lastPage = value; NotifyPropertyChanged(); }
        }
        private int lastLine = 0;
        public int LastLine {
            get { return lastLine; }
            set { lastLine = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string info = "默认值") {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
    public class BookInfo {
        public string Name { get; set; } = "";
        public List<PageInfo> Pages { get; set; } = new List<PageInfo>();

        public int PageCount {
            get {
                return Pages.Count;
            }
        }
    }
    public class PageInfo {
        public List<string> Lines { get; set; } = new List<string>();
        public string Title {
            set {

            }
            get {
                return Lines.Count > 0 ? Lines[0] : "";
            }
        }
        public int LinesCount {
            set {

            }
            get {
                return Lines.Count;
            }
        }
    }
}