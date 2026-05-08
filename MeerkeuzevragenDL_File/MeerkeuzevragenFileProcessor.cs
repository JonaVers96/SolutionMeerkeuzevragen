using System;
using System.Collections.Generic;
using System.IO;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Exceptions;
using MeerkeuzevragenBL.Interfaces;
using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenDL_File
{
    public class MeerkeuzevragenFileProcessor : IMeerkeuzevragenFileProcessor
    {
        public List<Vraag> LeesVragenBestand(string bestandsPad, Onderwerp onderwerp, Moeilijkheid standaardMoeilijkheid) {
            if (!File.Exists(bestandsPad))
                throw new FileNotFoundException("Het opgegeven tekstbestand kon niet gevonden worden.", bestandsPad);

            List<Vraag> geparsteVragen = new List<Vraag>();
            string[] lijnen = File.ReadAllLines(bestandsPad);

            Vraag huidigeVraag = null;
            bool leesAntwoordenSleutel = false;
            List<char> correcteLetters = new List<char>();

            foreach (string lijn in lijnen) {
                string getrimdeLijn = lijn.Trim();

                // Negeer lege regels
                if (string.IsNullOrWhiteSpace(getrimdeLijn))
                    continue;

                // Controleer of we bij de sectie 'Antwoorden' zijn aangekomen
                if (getrimdeLijn.Equals("Antwoorden", StringComparison.OrdinalIgnoreCase)) {
                    leesAntwoordenSleutel = true;
                    continue;
                }

                if (!leesAntwoordenSleutel) {
                    // 1. Check of het een VRAAG is (begint met een cijfer gevolgd door een punt)
                    if (char.IsDigit(getrimdeLijn[0]) && getrimdeLijn.Contains(".")) {
                        int puntIndex = getrimdeLijn.IndexOf('.');
                        string vraagTekst = getrimdeLijn.Substring(puntIndex + 1).Trim();

                        huidigeVraag = new Vraag(vraagTekst, standaardMoeilijkheid, onderwerp);
                        geparsteVragen.Add(huidigeVraag);
                    }
                    // 2. Check of het een ANTWOORD is (begint met A., B., C. of D.)
                    else if (huidigeVraag != null && getrimdeLijn.Length > 2 && getrimdeLijn[1] == '.' &&
                            (getrimdeLijn[0] >= 'A' && getrimdeLijn[0] <= 'D')) {
                        string antwoordTekst = getrimdeLijn.Substring(2).Trim();

                        // We zetten IsCorrect voorlopig op false. Dit corrigeren we straks via de sleutel.
                        Antwoord nieuwAntwoord = new Antwoord(antwoordTekst, false);
                        huidigeVraag.VoegAntwoordToe(nieuwAntwoord);
                    }
                } else {
                    // 3. We lezen de OPLOSSINGSSLEUTEL
                    // Door elk karakter in de lijn te checken, ondersteunen we zowel "ABCACABACB" als verticale letters.
                    foreach (char c in getrimdeLijn.ToUpper()) {
                        if (c >= 'A' && c <= 'D') {
                            correcteLetters.Add(c);
                        }
                    }
                }
            }

            // 4. Koppel de oplossingssleutel aan de vragen
            if (geparsteVragen.Count != correcteLetters.Count) {
                throw new Exception($"Parsingsfout: Aantal vragen ({geparsteVragen.Count}) komt niet overeen met het aantal antwoorden in de sleutel ({correcteLetters.Count}).");
            }

            for (int i = 0; i < geparsteVragen.Count; i++) {
                char correcteLetter = correcteLetters[i];

                // Converteer letter 'A' naar index 0, 'B' naar 1, etc.
                int correcteIndex = correcteLetter - 'A';

                if (correcteIndex >= 0 && correcteIndex < geparsteVragen[i].Antwoorden.Count) {
                    geparsteVragen[i].Antwoorden[correcteIndex].IsCorrect = true;
                } else {
                    throw new Exception($"Fout in de oplossingssleutel bij vraag {i + 1}.");
                }
            }

            return geparsteVragen;
        }
    }
}
