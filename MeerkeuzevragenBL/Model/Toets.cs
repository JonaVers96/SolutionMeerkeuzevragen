using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenBL.Model {
    public class Toets {
        {
        public int Id { get; set; }
        public DateTime AanmaakDatum { get; set; }
        public Onderwerp Onderwerp { get; set; }

        public List<Vraag> Vragen { get; private set; } = new List<Vraag>();

        public Toets(Onderwerp onderwerp) {
            Onderwerp = onderwerp;
            AanmaakDatum = DateTime.Now;
        }

        public Toets(int id, Onderwerp onderwerp, DateTime aanmaakDatum) : this(onderwerp) {
            Id = id;
            AanmaakDatum = aanmaakDatum;
        }

        public void VoegVraagToe(Vraag vraag) {
            if (vraag == null) throw new DomeinException("Vraag mag niet null zijn.");

            if (!vraag.IsActief)
                throw new DomeinException("Inactieve vragen mogen niet aan een test worden toegevoegd.");

            if (vraag.Onderwerp.Id != this.Onderwerp.Id && vraag.Onderwerp.Naam != this.Onderwerp.Naam)
                throw new DomeinException("De vraag behoort niet tot het juiste onderwerp.");

            if (Vragen.Contains(vraag))
                throw new DomeinException("Deze vraag zit al in de toets.");

            vraag.ShuffleAntwoorden();
            Vragen.Add(vraag);
        }
    }
}
