using System;

namespace MyApp.Model
{
    public class Agent : IEquatable<Agent>
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
        private string _dateB;
        public string DateB
        {
            get { return this._dateB; }
            set { this._dateB = value; }
        }
        private string _placeB;
        public string PlaceB
        {
            get { return this._placeB; }
            set { this._placeB = value; }
        }
        private string post;
        public string Post
        {
            get { return this.post; }
            set { this.post = value; }
        }
        private string searchString;
        public string SearchString
        {
            get { return this.searchString; }
            set { this.searchString = value; }
        }
        public Agent(string SapNumber, string Post, string Surname, string SearchString, string DateB, string PlaceB)
        {
            this.SapNumber = SapNumber;
            this.Surname = Surname;
            this.Post = Post;
            this.SearchString = SearchString;
            this.DateB = DateB;
            this.PlaceB = PlaceB;
        }

        public override string ToString()
        {
            return this.surname.ToString();
        }

        public bool Equals(Agent other)
        {
            if (other == null)
                return false;

            if (this.SapNumber == other.SapNumber)
                return true;
            else
                return false;
        }
    }
}
