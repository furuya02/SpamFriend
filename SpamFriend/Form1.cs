using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SpamFriend {
    public partial class Form1 : Form{

        private int _size;
        private Color _color;


        public Form1(){
            InitializeComponent();

            const string topUrl = "https://www.facebook.com/toshiyuki.katayama.50/friends";

            textBox1.Text = topUrl;
            timer1.Interval = 1000; //1秒
        }


        //友達一覧検索
        private void SearchFriend(){
            if (webBrowser1.Document == null){
                return;
            }
            var body = webBrowser1.Document.Body;
            var s = body.InnerHtml;
            if (s == null){
                return;
            }
            var start = s.LastIndexOf("友達を検索");
            var target = s.Substring(start);
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
                if(-1==(i = target.IndexOf(">"))){
                    break;
                }
                target = target.Substring(i);
                i = target.IndexOf("<");
                if (i == -1){
                    break;
                }

                var name = target.Substring(1, i - 1);

                listBox.Items.Add(String.Format("[{0}] {1} {2}", count++, name, url));

                target = target.Substring(i);

            }
        }
   

        //新しいURLに変化した場合、TextBoxを書き換える
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e){
            var url = e.Url.ToString();
            if (url.IndexOf("ai.php?") == -1){
                int i = url.IndexOf("?");
                if (i != -1){
                    textBox1.Text = url.Substring(0, i);
                }
            }
        }

        private void buttonStatr_Click(object sender, EventArgs e){
            Start();
        }

        private void buttonStop_Click(object sender, EventArgs e){
            Stop();
        }

        //タイマー処理 1秒後にサイズが変わっていたら下スクロールしてAjaxを起動させる
        private void timer1_Tick(object sender, EventArgs e){
            if (webBrowser1.Document == null){
                return;
            }
            if (webBrowser1.Document.Body == null){
                return;
            }
            if (webBrowser1.Document.Body.InnerHtml == null){
                return;
            }
            var len = webBrowser1.Document.Body.InnerText.Length;
            if (len != _size){
                _size = len;
                webBrowser1.Document.Window.ScrollTo(0, 50000);
            } else{
                Stop();
            }

        }


        //textBoxのURLでブラウジング開始(スクロールして最後まで取得する)
        private void Start(){

            listBox.Items.Clear();

            if (textBox1.Text.IndexOf("/friends") == -1){
                textBox1.Text = textBox1.Text + @"/friends";
            }

            //ボタン状態
            buttonStart.Enabled =false;
            buttonStop.Enabled = true;
            _color = panel1.BackColor;
            panel1.BackColor = Color.Red;

            webBrowser1.Navigate(textBox1.Text);
            _size = 0;
            timer1.Enabled = true;
        }

        private void Stop(){
            timer1.Enabled = false;
            //スクロールを一番上に戻す
            webBrowser1.Document.Window.ScrollTo(0, 0);

            //ボタン状態
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            panel1.BackColor = _color;

            //友達一覧
            SearchFriend();

            var sb = new StringBuilder();
            foreach (var imem in listBox.Items){
                sb.Append(imem);
                sb.Append("\r\n");
            }
            Clipboard.SetDataObject(sb.ToString(), true);

        }

        private void buttonBack_Click(object sender, EventArgs e){
            webBrowser1.GoBack();
        }
    }
}
