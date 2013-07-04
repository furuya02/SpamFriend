using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpamFriend {
    public class ListBlack : IEnumerable{
        private readonly List<OneBlack> _ar = new List<OneBlack>();
        private readonly String _fileName;
        private readonly ListView _listView;

        public ListBlack(ListView listView) {
            _listView = listView;
            _listView.AllowDrop = true;
            _listView.DragEnter+=_listView_DragEnter;
            _listView.DragDrop += _listView_DragDrop;

            _fileName = "listBlack.dat";
            
            Read();

            foreach (var a in _ar){
                listView.Items.Add(a.Key);
            }

        }

        void _listView_DragDrop(object sender, DragEventArgs e) {
            var str = (String)e.Data.GetData(DataFormats.Text);
            var key = GetKey(str);
            if (key != null) {
                if (Add(key)) {
                    _listView.Items.Add(key);
                    Save();
                }
            }
        }

        private void _listView_DragEnter(object sender, DragEventArgs e){
            e.Effect = DragDropEffects.All;
        }

        void Read(){
            if (File.Exists(_fileName)){
                var lines = File.ReadLines(_fileName);
                foreach (var l in lines){
                    _ar.Add(new OneBlack(l));
                }
            }
        }

        void Save(){
            var lines = new List<string>();
            foreach (var a in _ar){
                lines.Add(a.Key);
            }
            File.WriteAllLines(_fileName, lines);
        }

        bool Add(String key){
            foreach (var a in _ar){
                if (a.Key == key){
                    return false; //既に存在する
                }
            }
            _ar.Add(new OneBlack(key));
            return true;
        }

        public IEnumerator GetEnumerator(){
            return ((IEnumerable<OneBlack>) _ar).GetEnumerator();
        }

        public OneBlack Search(string key){
            foreach (var a in _ar){
                if (a.Key == key){
                    return a;
                }
            }
            return null;
        }


        private String GetKey(string str) {
            var tmp = "";
            if (str == null) {
                return null;
            }
            if (str.IndexOf("https://www.facebook.com/") != -1) {
                tmp = str.Substring(25);

                var i = tmp.IndexOf("profile.php?id");
                if (i != -1) {
                    i = tmp.IndexOf("&");
                    if (i != -1) {
                        tmp = tmp.Substring(0, i);
                    }
                    return tmp;
                }


                i = tmp.IndexOf("/");
                if (i != -1) {
                    tmp = tmp.Substring(0, i);
                    return tmp;
                }
                i = tmp.IndexOf("&");
                if (i != -1) {
                    tmp = tmp.Substring(0, i);
                }
                return tmp;

            }
            return null;
        }

    }
}
