using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeerkeuzevragenBL.Exceptions;
using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenBL.Gebruikers {
    public class Leerling : Gebruiker {
        public Klas Klas { get; set; }
        public List<Resultaat> Resultaten { get; private set; } = new List<Resultaat>();

        public Leerling(string naam, Klas klas) : base(naam) {
            Klas = klas ?? throw new DomeinException("Een leerling moet aan een klas toegewezen worden.");
        }

        public Leerling(int id, string naam, Klas klas) : base(id, naam) {
            Klas = klas;
        }

        public void VoegResultaatToe(Resultaat resultaat) {
            if (resultaat == null) throw new DomeinException("Resultaat mag niet null zijn.");
            if (!Resultaten.Contains(resultaat)) {
                Resultaten.Add(resultaat);
            }
        }
    }
}
