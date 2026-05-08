using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeerkeuzevragenBL.Gebruikers {
    public class Leerkracht : Gebruiker {
        public Leerkracht(string naam) : base(naam) {
        }
        public Leerkracht(int id, string naam) : base(id, naam) {
        }
    }
}
