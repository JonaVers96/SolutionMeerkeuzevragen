using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeerkeuzevragenBL.Exceptions;
using MeerkeuzevragenBL.Gebruikers;

namespace MeerkeuzevragenBL.Model {
    public class Resultaat {
        public int Id { get; set; }
        public Toets AfgelegdeToets { get; set; }
        public Leerling Eigenaar { get; set; }

        public string IngeleverdeAntwoorden { get; set; }

        public int Score { get; private set; }
        public int MaxScore { get; private set; }

        public Resultaat(Toets toets, Leerling eigenaar, string ingeleverdeAntwoorden) {
            AfgelegdeToets = toets ?? throw new DomeinException("Toets mag niet null zijn.");
            Eigenaar = eigenaar ?? throw new DomeinException("Leerling mag niet null zijn.");
            IngeleverdeAntwoorden = ingeleverdeAntwoorden;

            BerekenScore();
        }

        public Resultaat(int id, Toets toets, Leerling eigenaar, string ingeleverdeAntwoorden, int score)
            : this(toets, eigenaar, ingeleverdeAntwoorden) {
            Id = id;
            Score = score;
        }

        private void BerekenScore() {
            if (string.IsNullOrWhiteSpace(IngeleverdeAntwoorden)) {
                Score = 0;
                MaxScore = AfgelegdeToets.Vragen.Count;
                return;
            }

            int behaaldeScore = 0;
            MaxScore = AfgelegdeToets.Vragen.Count;

            for (int i = 0; i < AfgelegdeToets.Vragen.Count; i++) {
                if (i >= IngeleverdeAntwoorden.Length) break;

                char gegevenAntwoordLetter = char.ToUpper(IngeleverdeAntwoorden[i]);

                int antwoordIndex = gegevenAntwoordLetter - 'A';

                Vraag huidigeVraag = AfgelegdeToets.Vragen[i];

                if (antwoordIndex >= 0 && antwoordIndex < huidigeVraag.Antwoorden.Count) {
                    if (huidigeVraag.Antwoorden[antwoordIndex].IsCorrect) {
                        behaaldeScore++;
                    }
                }
            }

            Score = behaaldeScore;
        }
    }
}
