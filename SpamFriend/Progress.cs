using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpamFriend {
    class Progress{
        private readonly ToolStripProgressBar _progressBar;
        public Progress(ToolStripProgressBar progressBar){
            _progressBar = progressBar;
            _progressBar.Size = new Size(0, 10);
        }

        public void Init(int max){
            _progressBar.Size = new Size(100, 10);
            _progressBar.Minimum = 0;
            _progressBar.Maximum = max;
            _progressBar.Value = 0;
        }
        public void Inc(){
            _progressBar.Value++;
        }
        public void Finish(){
            _progressBar.Value = 0;
            _progressBar.Size = new Size(0,10);
        }

    }
}
