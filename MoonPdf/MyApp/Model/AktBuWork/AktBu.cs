using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATPWork.MyApp.Model.AktBuWork
{
    public class AktBu:Abonent
    {
        private int numberMail;
        public int NumberMail
        {
            get
            {
                return this.numberMail;
            }
            set
            {
                this.numberMail = value;
                this.OnPropertyChanged("NumberMail");
                this.OnPropertyChanged("MailName");
            }
        }
        private long _sizePDF;
        public long SizePDF
        {
            get
            {
                return this._sizePDF;
            }
            set
            {
                this._sizePDF = value;
                this.OnPropertyChanged("SizePDF");
            }
        }
        public string MailName
        {
            get
            {
                string result;
                if ((numberMail > 0) && DateMail != null) result = "исх.№91-" + numberMail + " от " + DateMail?.ToString("d");
                else result = "Неотправлено";
                return result;
            }

        }
        private int number;
        public int Number
        {
            get { return this.number; }
            set
            {
                this.number = value;
                this.OnPropertyChanged("Number");
                this.OnPropertyChanged("AktName");
            }
        }
        private string sapNumberAkt;
        public string SapNumberAkt
        {
            get { return this.sapNumberAkt; }
            set
            {
                this.sapNumberAkt = value;
                this.OnPropertyChanged("SapNumberAkt");
            }
        }
        private DateTime? dateMail;
        public DateTime? DateMail
        {
            get { return this.dateMail; }
            set
            {
                this.dateMail = value;
                this.OnPropertyChanged("DateMail");
                this.OnPropertyChanged("MailName");
            }
        }
        private DateTime? dateWork;
        public DateTime? DateWork
        {
            get { return this.dateWork; }
            set
            {
                this.dateWork = value;
                this.OnPropertyChanged("DateWork");
                this.OnPropertyChanged("AktName");
            }
        }
        private Agent agent_1;
        public Agent Agent_1
        {
            get { return this.agent_1; }
            set
            {
                this.agent_1 = value; this.OnPropertyChanged("Agent_1");
            }
        }
        private Agent agent_2;
        public Agent Agent_2
        {
            get { return this.agent_2; }
            set
            {
                this.agent_2 = value; this.OnPropertyChanged("Agent_2");
            }
        }
    }
}
