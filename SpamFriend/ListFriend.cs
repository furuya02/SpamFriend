using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpamFriend {
    class ListFriend {
        private readonly List<OneFriend> _ar = new List<OneFriend>();
        private readonly ListView _listView;

        private readonly Progress _progress;
        readonly ImageList _imageList = new ImageList();

        public ListFriend(ListView listView,Progress progress){
            
            _imageList.ImageSize = new Size(64,64);
            
            _listView = listView;
            _listView.LargeImageList = _imageList;
            _progress = progress;
        }

        public int Count{
            get{
                return _ar.Count;
            }
        }

        public void Init(String str){

            var start = str.LastIndexOf("友達を検索");
            if (start < 0) {
                return;
            }
            
            var target = str.Substring(start);
            const string parseStr = "<div class=\"clearfix\">";
            var lines = target.Split(new[] { parseStr }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lines) {
                var o = new OneFriend();
                if (o.Parse(l)) {
                    _ar.Add(o);
                }
            }

            _progress.Init(_ar.Count);

            var i = 0;
            foreach (var a in _ar) {
                var item = _listView.Items.Add(a.Name, i++);
                item.SubItems.Add(a.Key);
                item.SubItems.Add(a.Jpg);
                _imageList.Images.Add(a.Image);
                _progress.Inc();
            }
            _progress.Finish();

            //デバッグ情報
            var sb = new StringBuilder();
            foreach (var o in _ar) {
                var s = String.Format("{0} {1}\r\n", o.Name, o.Key);
                sb.Append(s);
            }
            Clipboard.SetDataObject(sb.ToString(), true);

        }

        public int SpamCheck(ListBlack listBlack){
            List<ListViewItem> tmp = new List<ListViewItem>();
            int count = 0;
            foreach (ListViewItem item in _listView.Items){
                if (null != listBlack.Search(item.SubItems[1].Text)) {

                    //item.ForeColor = Color.Red;
                    //item.BackColor = Color.Pink;
                    //item.Font = new Font("Arial", 11, FontStyle.Bold);
                    tmp.Add(item);
                    _listView.Items.Remove(item);
                    count++;
                }
            }
            foreach (var item in tmp){
                item.ForeColor = Color.Red;
                item.BackColor = Color.Pink;
                item.Font = new Font("Arial", 11, FontStyle.Bold);
                _listView.Items.Insert(0, item);
            }
            _listView.Refresh();
            return count;
        }

        /*
         
//リストビューに展開する
                _progress.Init(_listFriend.Count);


                var countSpam = 0;
                var i = 0;
                foreach (var a in _listFriend) {
                    var item = listViewFriend.Items.Add(a.Name, i++);
                    item.SubItems.Add(a.Key);
                    item.SubItems.Add(a.Jpg);
                    imageListFriend.Images.Add(a.Image);
                    if (null != _listBlack.Search(a.Key)) {
                        item.ForeColor = Color.Red;
                        countSpam++;
                    }
                    _progress.Inc();
                }
                _progress.Finish();
         * */

        public void Clear(){
            _listView.Clear();
            _imageList.Images.Clear();
            _ar.Clear();
        }
    }
}
