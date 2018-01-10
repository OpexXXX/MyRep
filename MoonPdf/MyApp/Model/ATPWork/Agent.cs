using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    public class Agent
    {
        private string sapNumber;
        public string SapNumber
        {
            get { return this.sapNumber; }
            set { this.sapNumber = value; }
        }
        private string surname;
        public string Surname
        {
            get { return this.surname; }
            set { this.surname = value; }
        }
        private string post;
        public string Post
        {
            get { return this.post; }
            set { this.post = value;  }
        }
        private string searchString;
        public string SearchString
        {
            get { return this.searchString; }
            set { this.searchString = value;  }
        }
        public Agent(string SapNumber, string Post, string Surname, string SearchString)
        {
            this.SapNumber = SapNumber;
            this.Surname = Surname;
            this.Post = Post;
            this.SearchString = SearchString;
        }

        public override string ToString()
        {
            return this.surname.ToString();
        }
    }
}
