using System;

namespace MyApp.Model
{
    public class PriborUcheta : IEquatable<PriborUcheta>
    {
        private string sapNumberPU;
        public string SapNumberPU
        {
            get { return this.sapNumberPU; }
            set { this.sapNumberPU = value; }
        }
        private string nazvanie;
        public string Nazvanie
        {
            get { return this.nazvanie; }
            set { this.nazvanie = value; }
        }
        private int poverka;
        public int Poverka
        {
            get { return this.poverka; }
            set { this.poverka = value; }
        }
        private string znachnost;
        public string Znachnost
        {
            get { return this.znachnost; }
            set { this.znachnost = value; }
        }
        public PriborUcheta(string SapNumberPU, string Nazvanie, int Poverka, string Znachnost)
        {
            this.SapNumberPU = SapNumberPU;
            this.Nazvanie = Nazvanie;
            this.Poverka = Poverka;
            this.Znachnost = Znachnost;
        }
        public bool Equals(PriborUcheta other)
        {
            if (other == null)
                return false;
            if (this.SapNumberPU == other.SapNumberPU)
                return true;
            else
                return false;
        }
    }
}
