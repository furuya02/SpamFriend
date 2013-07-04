using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SpamFriend {
    public class OneFriend{
        public String Name { get; private set; }
        public String Jpg { get; private set; }
        public String Key { get; private set; }
        public Image Image { get; private set; }
        public OneFriend() {
            Name = "";
            Jpg = "";
            Key = "";
            Image = null;
        }
        public bool Parse(String str){
            int i;

            //Jpg取得
            if (-1 == (i = str.IndexOf("src=\""))){
                return false;
            }
            str = str.Substring(i);
            if (-1 == (i = str.IndexOf("\">"))){
                return false;
            }
            var jpg = str.Substring(5, i - 5);
            try {
                var ext = Path.GetExtension(jpg);
                if (ext != null && ext.ToUpper() != ".JPG") {
                    return false;
                }
            } catch (Exception) {
                return false;
            }
            str = str.Substring(i);


            //Name取得
            if (-1 == (i = str.IndexOf("/ajax/hovercard/user.php?"))) {
                return false;
            }
            str = str.Substring(i);
            if (-1 == (i = str.IndexOf(">"))) {
                return false;
            }
            str = str.Substring(i);
            if (-1 == (i = str.IndexOf("<"))){
                return false;
            }
            var name = str.Substring(1, i - 1);


            //Url取得
            str = str.Substring(i);
            if (-1 == (i = str.IndexOf("href="))) {
                return false;
            }
            str = str.Substring(i+31);
            i = str.IndexOf("/friends");
            if (i != -1){
                Key = str.Substring(0, i);
            } else{
                i = str.IndexOf("sk=friends");
                if (i != -1){
                    Key = str.Substring(0, i - 5);
                } else{
                    return false;
                }
            }
            Name = name;
            Jpg = jpg;

            var info = IECache.GetUrlCacheEntryInfo(jpg);
            
            try{
                Image = Image.FromFile(info.lpszLocalFileName);
            }catch (Exception){
                Image = null;
            }
            return true;

        }
        public override String ToString(){
            return String.Format("{0},{1},{2}", Name, Key, Jpg);
        }
    }
}
