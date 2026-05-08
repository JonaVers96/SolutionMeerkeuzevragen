using System.Collections.Generic;
using System.Linq;
using MeerkeuzevragenBL.Enum; 
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenBL.Model {
    public class Vraag {
        public int Id { get; set; }

        private string _vraagTekst;
        public string VraagTekst {
            get { return _vraagTekst; }
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomeinException("Vraagtekst is verplicht.");
                _vraagTekst = value;
            }
        }

        public Moeilijkheid Moeilijkheidsgraad { get; set; }
        public bool IsActief { get; set; } = true;
        public Onderwerp Onderwerp { get; set; }

        public List<Antwoord> Antwoorden { get; private set; } = new List<Antwoord>();

        public Vraag(string vraagTekst, Moeilijkheid moeilijkheidsgraad, Onderwerp onderwerp) {
            VraagTekst = vraagTekst;
            Moeilijkheidsgraad = moeilijkheidsgraad;
            Onderwerp = onderwerp;
        }

        public Vraag(int id, string vraagTekst, Moeilijkheid moeilijkheidsgraad, Onderwerp onderwerp, bool isActief)
            : this(vraagTekst, moeilijkheidsgraad, onderwerp) {
            Id = id;
            IsActief = isActief;
        }

        public void VoegAntwoordToe(Antwoord antwoord) {
            if (antwoord == null) throw new DomeinException("Antwoord mag niet null zijn.");

            if (Antwoorden.Contains(antwoord))
                throw new DomeinException("Dit antwoord bestaat al voor deze vraag.");

            Antwoorden.Add(antwoord);
        }

        public bool IsGeldig() {
            return Antwoorden.Count >= 2 && Antwoorden.Count(a => a.IsCorrect) == 1;
        }

        public void ShuffleAntwoorden() {
            Random rand = new Random();
            int n = Antwoorden.Count;
            while (n > 1) {
                n--;
                int k = rand.Next(n + 1);
                Antwoord value = Antwoorden[k];
                Antwoorden[k] = Antwoorden[n];
                Antwoorden[n] = value;
            }
        }

        public override bool Equals(object obj) {
            if (obj is Vraag andereVraag) {
                if (this.Id != 0 && andereVraag.Id != 0) return this.Id == andereVraag.Id;
                return this.VraagTekst == andereVraag.VraagTekst;
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Id, VraagTekst);
        }
    }
}