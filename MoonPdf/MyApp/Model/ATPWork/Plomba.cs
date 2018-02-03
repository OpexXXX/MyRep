using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    public class Plomba 
    {
        private string type;
        public string Type
        {
            get { return this.type; }
            set { this.type = value;}
        }
        private string number;
        public string Number
        {
            get { return this.number; }
            set { this.number = value;}
        }
        private string place;
        public string Place
        {
            get { return this.place; }
            set { this.place = value;}
        }
        private bool _oldPlomb;
        public bool OldPlomb
        {
            get { return this._oldPlomb; }
            set { this._oldPlomb = value; }
        }
        private bool _demontage;
        public bool Demontage
        {
            get { return this._demontage; }
            set { this._demontage = value;}
        }
        private string _installDate;
        public string InstallDate
        {
            get { return this._installDate; }
            set { this._installDate = value; }
        }
        private string _status;
        public string Status
        {
            get { return this._status; }
            set { this._status = value; }
        }

        public Plomba(string type, string number, string place, bool remove = false, bool old =false,  string status = "",string dateInstall = "")
        {
            this.Type = type;
            this.Number = number;
            this.Place = place;
            this.Demontage = remove;
            this.Status = status;
            this.InstallDate = dateInstall;
            this.OldPlomb = old;
        }
        public override string ToString()
        {
            return Number;
        }
    }
    
}
