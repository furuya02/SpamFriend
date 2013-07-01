using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SpamFriend{
    public partial class MainForm : Form{

        private int _size;
        private Color _color;
        List<OneFriend> _listFriend = new List<OneFriend>(); 


        public MainForm(){
            InitializeComponent();

            //const string topUrl = "https://www.facebook.com/toshiyuki.katayama.50/friends";
            const string topUrl = "https://www.facebook.com/";

            textBoxUrl.Text = topUrl;
            timer1.Interval = 1000; //1秒
        }


        //友達一覧検索
        private void SearchFriend(){
            if (webBrowser.Document == null){
                return;
            }
            var body = webBrowser.Document.Body;
            var s = body.InnerHtml;
            if (s == null){
                return;
            }
            var start = s.LastIndexOf("友達を検索");
            if (start < 0){
                return;
            }
            var target = s.Substring(start);

            /***/
            const string parseStr = "<div class=\"clearfix\">";
            var lines = target.Split(new[]{parseStr},StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lines) {
                var o = new OneFriend();
                if (o.Parse(l)){
                    _listFriend.Add(o);
                } else{
                    o.Parse(l);
                }
            }

            /***/

            int count = 0;
            while (true){
                int i = target.IndexOf("src=\"");
                if (i == -1){
                    break;
                }
                target = target.Substring(i);
                i = target.IndexOf("\">");
                if (i == -1){
                    break;
                }
                var url = target.Substring(5, i - 5);
                try{
                    var ext = Path.GetExtension(url);
                    if (ext != null && ext.ToUpper() != ".JPG"){
                        target = target.Substring(i);
                        continue;
                    }
                } catch (Exception){
                    target = target.Substring(i);
                    continue;
                }
                target = target.Substring(i);



                i = target.IndexOf("/ajax/hovercard/user.php?");
                if (i == -1){
                    break;
                }
                target = target.Substring(i);
                if (-1 == (i = target.IndexOf(">"))){
                    break;
                }
                target = target.Substring(i);
                i = target.IndexOf("<");
                if (i == -1){
                    break;
                }

                var name = target.Substring(1, i - 1);


//                var imgFile = "$.JPG";
//                var wc = new WebClient();
//                wc.DownloadFile(url, imgFile);
//                wc.Dispose();
//
//                var img = Image.FromFile(imgFile);
//                imageList.Images.Add(img);
//                img.Dispose();
//
//                listView.Items.Add(name, imageList.Images.Count - 1);
                listView.Items.Add(name,-1);


                target = target.Substring(i);

            }
        }


        //新しいURLに変化した場合、TextBoxを書き換える
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            //webBrowser.Document.Body.Style = "zoom:80%"; 
            var url = e.Url.ToString();
            if (url.IndexOf("ai.php?") == -1 && url.IndexOf("/ajax/") == -1) {
                
                if (url.IndexOf("profile.php?id=")!=-1){
                    int i = url.IndexOf("&");
                    if (i != -1) {
                        textBoxUrl.Text = url.Substring(0, i);
                        listView.Items.Clear();
                    }
                } else{
                    int i = url.IndexOf("?");
                    if (i != -1) {
                        textBoxUrl.Text = url.Substring(0, i);
                        listView.Items.Clear();
                    }
                }
                
            }
        }


        //タイマー処理 1秒後にサイズが変わっていたら下スクロールしてAjaxを起動させる
        private void timer1_Tick(object sender, EventArgs e){
            if (webBrowser.Document == null){
                return;
            }
            if (webBrowser.Document.Body == null){
                return;
            }
            if (webBrowser.Document.Body.InnerHtml == null){
                return;
            }
            var len = webBrowser.Document.Body.InnerText.Length;
            if (len != _size){
                _size = len;
                webBrowser.Document.Window.ScrollTo(0, 500000);
            } else{
                Stop();
            }

        }


        //textBoxのURLでブラウジング開始(スクロールして最後まで取得する)
        private void Start(){

            toolStripStatusLabel1.Text = "";
            listView.Items.Clear();
            _listFriend.Clear();

            if (textBoxUrl.Text.IndexOf("profile.php?id=") != -1){
                if (textBoxUrl.Text.IndexOf("&sk=friends") == -1) {
                    textBoxUrl.Text = textBoxUrl.Text + @"&sk=friends";
                }
            } else {
                if (textBoxUrl.Text.IndexOf("/friends") == -1) {
                    textBoxUrl.Text = textBoxUrl.Text + @"/friends";
                }
            }



            //ボタン状態
            buttonSearch.Enabled = false;
            buttonStop.Enabled = true;
            _color = panel1.BackColor;
            panel1.BackColor = Color.Red;

            webBrowser.Navigate(textBoxUrl.Text);
            _size = 0;
            timer1.Enabled = true;
        }

        private void Stop(){
            timer1.Enabled = false;
            //スクロールを一番上に戻す
            webBrowser.Document.Window.ScrollTo(0, 0);

            //ボタン状態
            buttonSearch.Enabled = true;
            buttonStop.Enabled = false;
            panel1.BackColor = _color;

            //友達一覧
            SearchFriend();

            toolStripStatusLabel1.Text = String.Format("すべての友達 {0} {1}", listView.Items.Count, _listFriend.Count);

            //デバッグため色を付けてみる
            if (listView.Items.Count > 0){
                ListViewItem lvItem = listView.Items[0];
                lvItem.ForeColor = Color.Red;
            }
            var sb = new StringBuilder();
            foreach (var o in _listFriend){
                var s = String.Format("{0} {1}\r\n", o.Name, o.Url);
                sb.Append(s);
            }
            Clipboard.SetDataObject(sb.ToString(), true);


        }

        private void buttonBack_Click(object sender, EventArgs e){
            webBrowser.GoBack();
        }

        private void buttonStart_Click(object sender, EventArgs e){
            Start();

        }


        private void buttonStop_Click(object sender, EventArgs e){
            webBrowser.Stop();
            Stop();

        }


    }
}
