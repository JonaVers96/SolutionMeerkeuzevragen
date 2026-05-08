using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenBL.Gebruikers {
    public abstract class Gebruiker {
        public int Id { get; set; }

        private string _naam;
        public string Naam {
            get { return _naam; }
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomeinException("Naam mag niet leeg zijn.");
                _naam = value;
            }
        }

        protected Gebruiker(string naam) {
            Naam = naam;
        }

        protected Gebruiker(int id, string naam) : this(naam) {
            Id = id;
        }

    }
}
