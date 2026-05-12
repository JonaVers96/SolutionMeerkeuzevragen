using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenBL.Interfaces
{
    public interface IMeerkeuzevragenRepository {
        void VoegVraagToe(Vraag vraag);
        List<Resultaat> HaalAlleResultatenOp();
        void WisAlleData();
        void VoegOnderwerpToe(Onderwerp onderwerp);
    }
}
