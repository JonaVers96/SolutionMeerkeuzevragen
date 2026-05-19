using MeerkeuzevragenBL.Model;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenTests {
    public class ToetsTests {
        [Fact]
        public void VoegVraagToe_InactieveVraag_GooitDomeinException() {
            // Arrange
            Onderwerp ond = new Onderwerp("Wiskunde");
            Toets toets = new Toets(ond);
            Vraag inactieveVraag = new Vraag(1, "1+1?", Moeilijkheid.Makkelijk, ond, false);

            // Act & Assert
            Assert.Throws<DomeinException>(() => toets.VoegVraagToe(inactieveVraag));
        }

        [Fact]
        public void VoegVraagToe_VraagVanAnderOnderwerp_GooitDomeinException() {
            // Arrange
            Onderwerp wiskunde = new Onderwerp(1, "Wiskunde");
            Onderwerp frans = new Onderwerp(2, "Frans");
            Toets toets = new Toets(wiskunde);

            Vraag vraagFrans = new Vraag("Bonjour?", Moeilijkheid.Makkelijk, frans);

            // Act & Assert
            Assert.Throws<DomeinException>(() => toets.VoegVraagToe(vraagFrans));
        }
    }
}