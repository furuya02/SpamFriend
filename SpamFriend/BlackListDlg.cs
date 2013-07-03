using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpamFriend{
    public partial class BlackListDlg : Form{
        private ListBlack _listBlack;
        public BlackListDlg(ListBlack listBlack){
            _listBlack = listBlack;
            InitializeComponent();

            foreach (OneBlack a in _listBlack){
                listView.Items.Add(a.Key);

            }
        }

        private void listView_DragDrop(object sender, DragEventArgs e){
            var str = (String) e.Data.GetData(DataFormats.Text);
            var key = GetKey(str);
            if (key != null){
                if (_listBlack.Add(key)){
                    listView.Items.Add(key);
                }
            }
        }

        private void listView_DragEnter(object sender, DragEventArgs e){
            e.Effect = DragDropEffects.All;
        }



        private String GetKey(string str){
            var tmp = "";
            if (str == null){
                return null;
            }
            if (str.IndexOf("https://www.facebook.com/") != -1){
                tmp = str.Substring(25);

                var i = tmp.IndexOf("profile.php?id");
                if (i != -1) {
                    i = tmp.IndexOf("&");
                    if (i != -1){
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

        private void BlackListDlg_FormClosed(object sender, FormClosedEventArgs e) {
            _listBlack.Save();
        }
    }
}
