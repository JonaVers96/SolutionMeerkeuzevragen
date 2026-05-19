using System;
using System.Collections.Generic;
using System.IO;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Exceptions;
using MeerkeuzevragenBL.Interfaces;
using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenDL_File {
    public class MeerkeuzevragenFileProcessor : IMeerkeuzevragenFileProcessor {
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

                    // Bepaal of de huidige lijn een nieuwe vraag of een antwoord is
                    int puntIndex = getrimdeLijn.IndexOf('.');
                    bool isVraag = puntIndex > 0 && int.TryParse(getrimdeLijn.Substring(0, puntIndex), out _);
                    bool isAntwoord = getrimdeLijn.Length > 2 && getrimdeLijn[1] == '.' && (getrimdeLijn[0] >= 'A' && getrimdeLijn[0] <= 'D');

                    // 1. Het is de start van een NIEUWE VRAAG
                    if (isVraag) {
                        string vraagTekst = getrimdeLijn.Substring(puntIndex + 1).Trim();

                        // Oplossing voor de "Vraagtekst is verplicht" fout (als de tekst op de volgende lijn staat)
                        if (string.IsNullOrWhiteSpace(vraagTekst)) {
                            vraagTekst = "[WACHT OP TEKST]";
                        }

                        huidigeVraag = new Vraag(vraagTekst, standaardMoeilijkheid, onderwerp);
                        geparsteVragen.Add(huidigeVraag);
                    }
                    // 2. Het is een ANTWOORD
                    else if (isAntwoord && huidigeVraag != null) {
                        string antwoordTekst = getrimdeLijn.Substring(2).Trim();
                        Antwoord nieuwAntwoord = new Antwoord(antwoordTekst, false);
                        huidigeVraag.VoegAntwoordToe(nieuwAntwoord);
                    }
                    // 3. Het is MEERDERE LIJNEN TEKST voor de vraag
                    else if (huidigeVraag != null && huidigeVraag.Antwoorden.Count == 0) {
                        // Als we géén antwoorden hebben, behoort deze lijn nog steeds toe aan de vraag!

                        if (huidigeVraag.VraagTekst == "[WACHT OP TEKST]") {
                            // Vervang de tijdelijke tekst
                            huidigeVraag.VraagTekst = getrimdeLijn;
                        } else {
                            // Voeg de extra lijn tekst toe met een Enter (NewLine)
                            huidigeVraag.VraagTekst += Environment.NewLine + getrimdeLijn;
                        }
                    }
                } else {
                    // OPLOSSINGSSLEUTEL
                    foreach (char c in getrimdeLijn.ToUpper()) {
                        if (c >= 'A' && c <= 'D') {
                            correcteLetters.Add(c);
                        }
                    }
                }
            }

            // Koppel de oplossingssleutel aan de vragen
            if (geparsteVragen.Count != correcteLetters.Count) {
                throw new Exception($"Parsingsfout in {Path.GetFileName(bestandsPad)}: Aantal vragen ({geparsteVragen.Count}) komt niet overeen met het aantal antwoorden in de sleutel ({correcteLetters.Count}).");
            }

            for (int i = 0; i < geparsteVragen.Count; i++) {
                char correcteLetter = correcteLetters[i];
                int correcteIndex = correcteLetter - 'A';

                if (correcteIndex >= 0 && correcteIndex < geparsteVragen[i].Antwoorden.Count) {
                    geparsteVragen[i].Antwoorden[correcteIndex].IsCorrect = true;
                } else {
                    throw new Exception($"Fout in de oplossingssleutel bij vraag {i + 1}.");
                }
            }

            return geparsteVragen;
        }

        public void SchrijfToetsNaarBestand(Toets toets, string bestandsPad) {
            List<string> lijnen = new List<string>();

            lijnen.Add($"Raad de artiest – {toets.Onderwerp.Naam} ({toets.Vragen.Count} vragen)");
            lijnen.Add(""); // Lege regel

            List<char> correcteLettersSleutel = new List<char>();

            for (int i = 0; i < toets.Vragen.Count; i++) {
                Vraag vraag = toets.Vragen[i];

                lijnen.Add($"{i + 1}. {vraag.VraagTekst}");
                lijnen.Add(""); // Witregel onder de vraag zoals in je voorbeeld

                for (int j = 0; j < vraag.Antwoorden.Count; j++) {
                    Antwoord antwoord = vraag.Antwoorden[j];
                    char letter = (char)('A' + j); // Verander index 0 naar 'A', 1 naar 'B', etc.

                    lijnen.Add($"{letter}. {antwoord.Tekst}");

                    if (antwoord.IsCorrect) {
                        correcteLettersSleutel.Add(letter);
                    }
                }
                lijnen.Add(""); // Witregel tussen de vragen
            }

            File.WriteAllLines(bestandsPad, lijnen);
        }
    }
}