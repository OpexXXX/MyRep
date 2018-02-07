using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.VnePlan
{
    public class VnePlanZayavka:Abonent
    {
        public string BriefInformation
        {
            get
            {
                string result = "";
                if(FIO=="") result += base.FIO + "\n " +base.Adress + "\n " + base.NumberLS;
                return result;
            }

        }
        private string _tempFio;
        public string TempFIO
        {
            get { return this._tempFio; }
            set
            {
                this._tempFio = value;
            }
        }
        private bool _provFlag;
        public bool ProvFlag
        {
            get { return this._provFlag; }
            set
            {
                this._provFlag = value;
                this.OnPropertyChanged("ProvFlag");
            }
        }
        private bool _dopuskFlag;
        public bool DopuskFlag
        {
            get { return this._dopuskFlag; }
            set
            {
                this._dopuskFlag = value;
                this.OnPropertyChanged("DopuskFlag");
            }
        }
        private bool _demontageFlag;
        public bool DemontageFlag
        {
            get { return this._demontageFlag; }
            set
            {
                this._demontageFlag = value;
                this.OnPropertyChanged("DemontageFlag");
            }
        }



        private string _primechanie;
        public string Primechanie
        {
            get { return this._primechanie; }
            set
            {
                this._primechanie = value;
                this.OnPropertyChanged("Primechanie");
            }
        }
        private int _regNumber;
        public int RegNumber
        {
            get { return this._regNumber; }
            set
            {
                this._regNumber = value;
                this.OnPropertyChanged("RegNumber");
            }
        }
        private DateTime _dateReg = DateTime.Now;
        public DateTime DateReg
        {
            get { return this._dateReg; }
            set
            {
                this._dateReg = value;
                this.OnPropertyChanged("DateReg");
            }
        }
        private string _prichina;
        public string Prichina
        {
            get { return this._prichina; }
            set
            {
                this._prichina = value;
                this.OnPropertyChanged("Prichina");
            }
        }
      
        private string _numberAktTehProverki;
        public string NumberAktTehProverki
        {
            get { return this._numberAktTehProverki; }
            set
            {
                this._numberAktTehProverki = value;
                this.OnPropertyChanged("NumberAktTehProverki");
            }
        }
        private  string _phoneNumbers;
        public string PhoneNumbers
        {
            get { return this._phoneNumbers; }
            set
            {
                this._phoneNumbers = value;
            }
        }

        internal bool CanAdd()
        {
            bool result = true;
            if (DateReg == null) result = false;
            if (FIO == null || FIO == "") result = false;
            if (!(ProvFlag || DopuskFlag || DemontageFlag)) result = false;
            return result;

        }
    }
}
