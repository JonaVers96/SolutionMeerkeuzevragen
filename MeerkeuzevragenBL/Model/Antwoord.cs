using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenBL.Model {
    public class Antwoord {
        public int Id { get; set; }

        private string _tekst;
        public string Tekst {
            get { return _tekst; }
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomeinException("De tekst van een antwoord mag niet leeg zijn.");
                _tekst = value;
            }
        }

        public bool IsCorrect { get; set; }

        public Antwoord(string tekst, bool isCorrect) {
            Tekst = tekst;
            IsCorrect = isCorrect;
        }

        public Antwoord(int id, string tekst, bool isCorrect) : this(tekst, isCorrect) {
            Id = id;
        }

        public override bool Equals(object obj) {
            if (obj is Antwoord ander) {
                if (this.Id != 0 && ander.Id != 0)
                    return this.Id == ander.Id;

                return this.Tekst.ToLower() == ander.Tekst.ToLower();
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Id, Tekst);
        }
    }
}
