using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpamFriend {
    public class ListBlack : IEnumerable{
        private readonly List<OneBlack> _ar = new List<OneBlack>();
        private readonly String _fileName;

        public ListBlack(){
            _fileName = "listBlack.dat";
            Read();
        }

        private void Read(){
            if (File.Exists(_fileName)){
                var lines = File.ReadLines(_fileName);
                foreach (var l in lines){
                    _ar.Add(new OneBlack(l));
                }
            }
        }

        public void Save(){
            List<String> lines = new List<string>();
            foreach (var a in _ar){
                lines.Add(a.Key);
            }
            File.WriteAllLines(_fileName, lines);
        }

        public bool Add(String key){
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
    }
}
