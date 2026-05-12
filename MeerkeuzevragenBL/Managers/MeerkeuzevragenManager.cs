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
            if (!(_ingelogdeGebruiker is Leerkracht))
                throw new ManagerException("Enkel een leerkracht mag vragen toevoegen.");

        }

        public List<Resultaat> HaalMijnResultatenOp() {
            if (_ingelogdeGebruiker is Leerling leerling) {
                return leerling.Resultaten; 
            } else if (_ingelogdeGebruiker is Leerkracht) {
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
    }
}
