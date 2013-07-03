using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpamFriend {
    public partial class ComfirmDlg : Form{
        private MainForm _mainForm;
        public ComfirmDlg(List<OneFriend> ar, ListBlack listBlack,MainForm mainForm) {
            InitializeComponent();

            _mainForm = mainForm;

            var countSpam = 0;
            foreach (var a in ar){
                var item = listView.Items.Add(a.Name, -1);
                item.SubItems.Add(a.Key);
                item.SubItems.Add(a.Jpg);

                if (null != listBlack.Search(a.Key)){
                    item.ForeColor = Color.Red;
                    countSpam++;
                }

            }

//            //デバッグため色を付けてみる
//            if (listView.Items.Count > 0) {
//                ListViewItem lvItem = listView.Items[0];
//                lvItem.ForeColor = Color.Red;
//            }
            var sb = new StringBuilder();
            foreach (var o in ar) {
                var s = String.Format("{0} {1}\r\n", o.Name, o.Key);
                sb.Append(s);
            }
            Clipboard.SetDataObject(sb.ToString(), true);

            statusLabel.Text = String.Format("すべての友達 {0} スパムアカウント {1}",  ar.Count,countSpam);


        }

        private void PopupMenuSearch_Click(object sender, EventArgs e){
            var items = listView.SelectedItems;
            if (items.Count > 0){
                _mainForm.Target = items[0].SubItems[1].Text;
                Close();
            }
        }
    }
}
