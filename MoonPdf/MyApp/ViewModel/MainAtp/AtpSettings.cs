using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.ViewModel.MainAtp
{
     public class AtpSettings
    {
        private string _folderProcessedAkt;
        public string FolderProcessedAkt
        {
            get { return _folderProcessedAkt; }
            set { _folderProcessedAkt = value; }
        }
        private string _folderOutMail;
        public string FolderOutMail
        {
            get { return _folderOutMail; }
            set { _folderOutMail = value; }
        }


    }
}
