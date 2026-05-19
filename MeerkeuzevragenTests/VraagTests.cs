using MeerkeuzevragenBL.Model;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenTests {
    public class VraagTests {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void VraagTekst_NullOfLeeg_GooitDomeinException(string fouteTekst) {
            // Arrange
            Onderwerp ond = new Onderwerp("Test");

            // Act & Assert
            Assert.Throws<DomeinException>(() => new Vraag(fouteTekst, Moeilijkheid.Makkelijk, ond));
        }

        [Fact]
        public void VoegAntwoordToe_BestaandAntwoord_GooitDomeinException() {
            // Arrange
            Onderwerp ond = new Onderwerp("Test");
            Vraag vraag = new Vraag("Wat is 1+1?", Moeilijkheid.Makkelijk, ond);
            Antwoord a1 = new Antwoord("2", true);
            Antwoord a2 = new Antwoord("2", false); // Zelfde tekst, dus Equals() geeft true!

            vraag.VoegAntwoordToe(a1);

            // Act & Assert
            Assert.Throws<DomeinException>(() => vraag.VoegAntwoordToe(a2));
        }

        [Fact]
        public void IsGeldig_TeWeinigAntwoorden_ReturnsFalse() {
            // Arrange
            Onderwerp ond = new Onderwerp("Test");
            Vraag vraag = new Vraag("Wat is 1+1?", Moeilijkheid.Makkelijk, ond);
            vraag.VoegAntwoordToe(new Antwoord("2", true)); // Slechts 1 antwoord

            // Act
            bool resultaat = vraag.IsGeldig();

            // Assert
            Assert.False(resultaat);
        }

        [Fact]
        public void IsGeldig_MeerdereCorrecteAntwoorden_ReturnsFalse() {
            // Arrange
            Onderwerp ond = new Onderwerp("Test");
            Vraag vraag = new Vraag("Wat is 1+1?", Moeilijkheid.Makkelijk, ond);
            vraag.VoegAntwoordToe(new Antwoord("2", true));
            vraag.VoegAntwoordToe(new Antwoord("Twee", true)); // Tweede correcte

            // Act
            bool resultaat = vraag.IsGeldig();

            // Assert
            Assert.False(resultaat);
        }

        [Fact]
        public void IsGeldig_CorrectAantalEnUniekCorrectAntwoord_ReturnsTrue() {
            // Arrange
            Onderwerp ond = new Onderwerp("Test");
            Vraag vraag = new Vraag("Wat is 1+1?", Moeilijkheid.Makkelijk, ond);
            vraag.VoegAntwoordToe(new Antwoord("2", true));
            vraag.VoegAntwoordToe(new Antwoord("3", false));

            // Act
            bool resultaat = vraag.IsGeldig();

            // Assert
            Assert.True(resultaat);
        }
    }
}