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
        ListBlack _listBlack = new ListBlack();

        public MainForm(){
            InitializeComponent();

            //const string topUrl = "https://www.facebook.com/toshiyuki.katayama.50/friends";
            //const string topUrl = "https://www.facebook.com/";
            const string topUrl = "https://www.facebook.com/emi.oumi";

            textBoxUrl.Text = topUrl;
            timer1.Interval = 500; //500 counter<6
            buttonHome_Click(null,null);

        }

        public String Target { get; set; }


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

            const string parseStr = "<div class=\"clearfix\">";
            var lines = target.Split(new[]{parseStr}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lines){
                var o = new OneFriend();
                if (o.Parse(l)){
                    _listFriend.Add(o);
                } else{
                    o.Parse(l);
                }
            }

        }
    

        //新しいURLに変化した場合、TextBoxを書き換える
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e){
            //確認中は処理しない
            if (!buttonComfirm.Enabled){
                return;
            }
            InitTarget(e.Url.ToString());
        }

        //Target更新
        void InitTarget(String url){
            if (url.IndexOf("ai.php?") == -1 && url.IndexOf("/ajax/") == -1) {

                if (url.IndexOf("profile.php?id=") != -1) {
                    int i = url.IndexOf("&");
                    if (i != -1) {
                        textBoxUrl.Text = url.Substring(0, i);
                    }
                    Target = textBoxUrl.Text.Substring(25);
                    Text = String.Format("SpamFriend -{0}-", Target);
                } else {
                    int i = url.IndexOf("?");
                    if (i != -1){
                        textBoxUrl.Text = url.Substring(0, i);
                    }
                    Target = textBoxUrl.Text.Substring(25);
                    Text = String.Format("SpamFriend -{0}-", Target);
                }
            }
        }


        //タイマー処理 1秒後にサイズが変わっていたら下スクロールしてAjaxを起動させる
        private int counter=0;
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
                counter = 0;
            } else{
                counter++;
                if (counter < 6){
                    return;
                }
                
                timer1.Enabled = false;

                //フィニッシュ
                //スクロールを一番上に戻す
                webBrowser.Document.Window.ScrollTo(0, 0);

                //ボタン状態
                ButtonInit(false);

                //友達一覧
                SearchFriend();

                var dlg = new ComfirmDlg(_listFriend, _listBlack,this);
                dlg.ShowDialog();

            }

        }

        void ButtonInit(bool sw){
            if (sw){//検索中
                textBoxUrl.Enabled = false;
                buttonComfirm.Enabled = false;
                buttonBack.Enabled = false;
                buttonForward.Enabled = false;
                buttonHome.Enabled = false;
                buttonRefresh.Enabled = false;
                buttonStop.Enabled = true;
                panel1.BackColor = Color.Red;
                panel1.Enabled = false;
                panel1.Cursor = Cursors.WaitCursor;

            } else{
                textBoxUrl.Enabled = true;
                buttonComfirm.Enabled = true;
                buttonBack.Enabled = true;
                buttonForward.Enabled = true;
                buttonHome.Enabled = true;
                buttonRefresh.Enabled = true;
                buttonStop.Enabled = false;
                panel1.BackColor = _color;
                panel1.Enabled = true;
                panel1.Cursor= Cursors.Default;
            }
            
        }




        //戻る
        private void buttonBack_Click(object sender, EventArgs e) {
            webBrowser.GoBack();
        }
        //停止
        private void buttonStop_Click(object sender, EventArgs e) {
            webBrowser.Stop();
            timer1.Enabled = false;
            ButtonInit(false);
        }

        //更新
        private void buttonRefresh_Click(object sender, EventArgs e){
            InitTarget(textBoxUrl.Text);
            webBrowser.Navigate(textBoxUrl.Text);
        }
        //ホーム
        private void buttonHome_Click(object sender, EventArgs e) {
            textBoxUrl.Text = "https://www.facebook.com/";
            webBrowser.Navigate(textBoxUrl.Text);

        }
        //進む
        private void buttonForward_Click(object sender, EventArgs e){
            webBrowser.GoForward();
        }
        //確認
        private void buttonComfirm_Click(object sender, EventArgs e) {
            //ボタン状態
            ButtonInit(true);

            _listFriend.Clear();
            var url = "https://www.facebook.com/friends";

            if (Target != null && Target != "") {
                url = String.Format("https://www.facebook.com/{0}/friends", Target);
                if (Target.IndexOf("profile.php?id=") >= 0) {
                    url = String.Format("https://www.facebook.com/{0}&sk=friends", Target);
                }
            }


            webBrowser.Navigate(url);
            _size = 0;
            counter = 0;
            timer1.Enabled = true;
        }

        //ブラッグリスト
        private void buttonBlackList_Click(object sender, EventArgs e){
            var dlg = new BlackListDlg(_listBlack);
            dlg.ShowDialog();
        }

        private void toolStrip1_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.All;
        }

        private void toolStrip1_DragDrop(object sender, DragEventArgs e) {
            textBoxUrl.Text = (String)e.Data.GetData(DataFormats.Text);
        }
    }
}
