using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.VnePlan
{
    public class VnePlanZayavka:Abonent
    {
        private string _tempFio;
        public string TempFIO
        {
            get { return this._tempFio; }
            set
            {
                this._tempFio = value;
            }
        }

        private string _primechanie;
        public string Primechanie
        {
            get { return this._primechanie; }
            set
            {
                this._primechanie = value;
            }
        }
        private int _regNumber;
        public int RegNumber
        {
            get { return this._regNumber; }
            set
            {
                this._regNumber = value;
            }
        }
        private DateTime _dateReg;
        public DateTime DateReg
        {
            get { return this._dateReg; }
            set
            {
                this._dateReg = value;
            }
        }
        private string _prichina;
        public string Prichina
        {
            get { return this._prichina; }
            set
            {
                this._prichina = value;
            }
        }
        public bool Complete {
            get {
                return true;
            } }
        private string _numberAktTehProverki;
        public string NumberAktTehProverki
        {
            get { return this._numberAktTehProverki; }
            set
            {
                this._numberAktTehProverki = value;
            }
        }
        private List< string> _phoneNumbers = new List<string>();
        public List<string> PhoneNumbers
        {
            get { return this._phoneNumbers; }
            set
            {
                this._phoneNumbers = value;
            }
        }
    }
}
