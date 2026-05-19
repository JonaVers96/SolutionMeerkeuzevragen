using Xunit;
using MeerkeuzevragenBL.Model;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Gebruikers;

namespace MeerkeuzevragenTests {
    public class ResultaatTests {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void BerekenScore_LegeOfNullAntwoorden_GeeftNulScore(string ingeleverd) {
            // Arrange
            Onderwerp ond = new Onderwerp("Test");
            Toets toets = new Toets(ond);
            Leerling leerling = new Leerling("Jona", new Klas("1A"));

            // Voeg dummy vraag toe
            Vraag v = new Vraag("Testvraag", Moeilijkheid.Makkelijk, ond);
            v.VoegAntwoordToe(new Antwoord("A", true));
            v.VoegAntwoordToe(new Antwoord("B", false));
            toets.Vragen.Add(v); // Direct toevoegen om shufflen in de test te vermijden

            // Act
            Resultaat resultaat = new Resultaat(toets, leerling, ingeleverd);

            // Assert
            Assert.Equal(0, resultaat.Score);
            Assert.Equal(1, resultaat.MaxScore);
        }

        [Fact]
        public void BerekenScore_DeelsJuistEnFout_BerekentCorrecteScore() {
            // Arrange
            Onderwerp ond = new Onderwerp("Test");
            Toets toets = new Toets(ond);
            Leerling leerling = new Leerling("Jona", new Klas("1A"));

            // Vraag 1 (Juiste is A)
            Vraag v1 = new Vraag("V1", Moeilijkheid.Makkelijk, ond);
            v1.VoegAntwoordToe(new Antwoord("Juist", true));  // A
            v1.VoegAntwoordToe(new Antwoord("Fout", false));  // B

            // Vraag 2 (Juiste is B)
            Vraag v2 = new Vraag("V2", Moeilijkheid.Makkelijk, ond);
            v2.VoegAntwoordToe(new Antwoord("Fout", false)); // A
            v2.VoegAntwoordToe(new Antwoord("Juist", true)); // B

            toets.Vragen.Add(v1);
            toets.Vragen.Add(v2);

            // Act
            // Leerling vult "A" in voor v1 (Juist) en "A" voor v2 (Fout).
            Resultaat resultaat = new Resultaat(toets, leerling, "AA");

            // Assert
            Assert.Equal(1, resultaat.Score);
            Assert.Equal(2, resultaat.MaxScore);
        }
    }
}