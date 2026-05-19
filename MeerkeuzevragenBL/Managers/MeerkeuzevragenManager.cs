using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Exceptions;
using MeerkeuzevragenBL.Gebruikers;
using MeerkeuzevragenBL.Interfaces;
using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenBL.Managers {
    public class MeerkeuzevragenManager {

        private IMeerkeuzevragenFileProcessor _fileProcessor;
        private IMeerkeuzevragenRepository _repository;
        private Gebruiker _ingelogdeGebruiker;

        public Gebruiker IngelogdeGebruiker { get; private set; }

        public MeerkeuzevragenManager(IMeerkeuzevragenFileProcessor fileProcessor, IMeerkeuzevragenRepository repository) {
            _fileProcessor = fileProcessor;
            _repository = repository;
        }

        public void ImporteerVragenUitBestand(string bestandsPad, Onderwerp onderwerp, Moeilijkheid moeilijkheid) {
            try {
                List<Vraag> ingelezenVragen = _fileProcessor.LeesVragenBestand(bestandsPad, onderwerp, moeilijkheid);

                if (ingelezenVragen.Count == 0) {
                    throw new ManagerException("Het bestand bevatte geen geldige vragen.");
                }

                foreach (Vraag nieuweVraag in ingelezenVragen) {
                    _repository.VoegVraagToe(nieuweVraag);
                }
            }
            catch (Exception ex) {
                throw new ManagerException($"Fout bij het importeren van vragen: {ex.Message}", ex);
            }
        }

        public void ImporteerVragenUitMap(string mapPad, Moeilijkheid moeilijkheid) {
            try {
                if (!Directory.Exists(mapPad)) {
                    throw new ManagerException($"De map '{mapPad}' kon niet gevonden worden.");
                }

                string[] txtBestanden = Directory.GetFiles(mapPad, "*.txt");

                if (txtBestanden.Length == 0) {
                    throw new ManagerException($"Er zijn geen .txt bestanden gevonden in de map '{mapPad}'.");
                }

                foreach (string bestandsPad in txtBestanden) {
                    string bestandsNaam = Path.GetFileNameWithoutExtension(bestandsPad);

                    Onderwerp bestandOnderwerp = new Onderwerp(bestandsNaam);

                    _repository.VoegOnderwerpToe(bestandOnderwerp);

                    ImporteerVragenUitBestand(bestandsPad, bestandOnderwerp, moeilijkheid);
                }
            }
            catch (Exception ex) {
                throw new ManagerException($"Fout bij het importeren vanuit map: {ex.Message}", ex);
            }
        }


        public void VoegVraagToe(Vraag nieuweVraag) {
            _repository.VoegVraagToe(nieuweVraag);


        }

        public List<Resultaat> HaalMijnResultatenOp() {
            if (IngelogdeGebruiker is Leerling leerling) {
                return _repository.HaalAlleResultatenOp()
                                          .Where(r => r.Eigenaar.Id == leerling.Id)
                                          .ToList();
            } else if (IngelogdeGebruiker is Leerkracht) {
                return _repository.HaalAlleResultatenOp();
            }
            throw new ManagerException("Ongeldige gebruiker.");
        }

        public void WisAlleData() {
            try {
                _repository.WisAlleData();
            }
            catch (Exception ex) {
                throw new ManagerException($"Fout bij het leegmaken van de database: {ex.Message}", ex);
            }
        }

        public void VoegOnderwerpToe(Onderwerp onderwerp) {
            try {
                _repository.VoegOnderwerpToe(onderwerp);
            }
            catch (Exception ex) {
                throw new ManagerException($"Fout bij het aanmaken van onderwerp: {ex.Message}", ex);
            }
        }

        public List<Onderwerp> HaalAlleOnderwerpenOp() {
            try {
                return _repository.HaalAlleOnderwerpenOp();
            }
            catch (Exception ex) {
                throw new ManagerException($"Fout bij ophalen van onderwerpen: {ex.Message}", ex);
            }
        }

        public List<Vraag> HaalVragenOpPerOnderwerp(Onderwerp onderwerp) {
            return _repository.HaalVragenOpPerOnderwerp(onderwerp.Id);
        }

        public void UpdateVraagActiefStaat(Vraag vraag) {
            _repository.UpdateVraagActiefStaat(vraag.Id, vraag.IsActief);
        }

        public Toets GenereerToets(Onderwerp onderwerp, int aantalVragen) {
            try {
                List<Vraag> randomVragen = _repository.HaalWillekeurigeVolledigeVragenOp(onderwerp.Id, aantalVragen);

                if (randomVragen.Count == 0) {
                    throw new ManagerException("Er zijn geen actieve vragen gevonden voor dit onderwerp.");
                }

                Toets nieuweToets = new Toets(onderwerp);

                foreach (Vraag v in randomVragen) {
                    v.Onderwerp = onderwerp;
                    nieuweToets.VoegVraagToe(v);
                }

                return nieuweToets;
            }
            catch (Exception ex) {
                throw new ManagerException($"Fout bij het genereren van de test: {ex.Message}", ex);
            }
        }

        public bool Login(string naam) {
            Gebruiker g = _repository.ZoekGebruiker(naam);
            if (g != null) {
                IngelogdeGebruiker = g;
                return true;
            }
            return false;
        }

        public void BewaarResultaat(Toets toets, int score) {
            if (IngelogdeGebruiker is Leerling leerling) {
                if (toets.Id == 0) {
                    _repository.VoegToetsToe(toets);
                }
                Resultaat res = new Resultaat(0, toets, leerling, "", score, toets.Vragen.Count); 
                _repository.BewaarResultaat(res);
            }
        }

        public void WisAlleResultaten() {
            if (IngelogdeGebruiker is Leerkracht) {
                try {
                    _repository.WisAlleResultaten();
                }
                catch (Exception ex) {
                    throw new ManagerException($"Fout bij het wissen van de resultaten: {ex.Message}", ex);
                }
            } else {
                throw new ManagerException("Toegang geweigerd. Enkel leerkrachten mogen resultaten wissen.");
            }
        }

        public void VerwijderOnderwerp(Onderwerp onderwerp) {
            if (IngelogdeGebruiker is Leerkracht) {
                try {
                    _repository.VerwijderOnderwerp(onderwerp.Id);
                }
                catch (Exception ex) {
                    throw new ManagerException($"Fout bij verwijderen van onderwerp: {ex.Message}", ex);
                }
            } else {
                throw new ManagerException("Toegang geweigerd. Enkel leerkrachten mogen onderwerpen verwijderen.");
            }
        }
        public void ExporteerToetsNaarBestand(Toets toets, string bestandsPad) {
            try {
                _fileProcessor.SchrijfToetsNaarBestand(toets, bestandsPad);
            }
            catch (Exception ex) {
                throw new ManagerException($"Fout bij het exporteren van de toets naar een bestand: {ex.Message}", ex);
            }
        }

    }
}
