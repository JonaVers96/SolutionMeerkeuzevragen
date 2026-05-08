using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenBL.Gebruikers {
    public class Klas {
        public int Id { get; set; }
        public string Naam { get; set; }

        public List<Leerling> Leerlingen { get; private set; } = new List<Leerling>();

        public Klas(string naam) {
            Naam = naam;
        }

        public void VoegLeerlingToe(Leerling leerling) {
            if (leerling == null) throw new DomeinException("Leerling mag niet null zijn.");
            if (!Leerlingen.Contains(leerling)) {
                Leerlingen.Add(leerling);
            }
        }
    }
}
