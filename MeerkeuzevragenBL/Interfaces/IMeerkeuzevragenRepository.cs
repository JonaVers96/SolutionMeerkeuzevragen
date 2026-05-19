using MeerkeuzevragenBL.Gebruikers;
using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenBL.Interfaces
{
    public interface IMeerkeuzevragenRepository {
        void VoegVraagToe(Vraag vraag);
        List<Resultaat> HaalAlleResultatenOp();
        List<Onderwerp> HaalAlleOnderwerpenOp();
        List<Vraag> HaalVragenOpPerOnderwerp(int onderwerpId);
        void UpdateVraagActiefStaat(int vraagId, bool isActief);
        void WisAlleData();
        void VoegOnderwerpToe(Onderwerp onderwerp);
        List<Vraag> HaalWillekeurigeVolledigeVragenOp(int onderwerpId, int aantalVragen);
        Gebruiker ZoekGebruiker(string naam);
        void BewaarResultaat(Resultaat resultaat);
        void VoegToetsToe(Toets toets);
        void WisAlleResultaten();
        void VerwijderOnderwerp(int onderwerpId);
    }
}
