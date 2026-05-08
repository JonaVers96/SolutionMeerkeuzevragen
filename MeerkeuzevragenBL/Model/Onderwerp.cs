using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenBL.Model {
    public class Onderwerp {
        public int Id { get; set; }

        private string _naam;
        public string Naam {
            get { return _naam; }
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomeinException("Naam van het onderwerp is verplicht.");
                _naam = value;
            }
        }

        public Onderwerp(string naam) {
            Naam = naam;
        }

        public Onderwerp(int id, string naam) : this(naam) {
            Id = id;
        }

        public override bool Equals(object obj) {
            if (obj is Onderwerp ander) {
                if (this.Id != 0 && ander.Id != 0) return this.Id == ander.Id;
                return this.Naam.ToLower() == ander.Naam.ToLower();
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Id, Naam);
        }
    }
}
