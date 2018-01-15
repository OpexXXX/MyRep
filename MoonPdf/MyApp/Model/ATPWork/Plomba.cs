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
        private float number;
        public float Number
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
        private bool remove;
        public bool Remove
        {
            get { return this.remove; }
            set { this.remove = value;}
        }
        public Plomba(string type, float number, string place, bool remove)
        {
            this.Type = type;
            this.Number = number;
            this.Place = place;
            this.Remove = remove;
        }
    }
}
