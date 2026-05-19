using MeerkeuzevragenBL.Model;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenTests {
    public class AntwoordTests {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Tekst_NullOfLeeg_GooitDomeinException(string fouteTekst) {
            // Arrange & Act & Assert
            Assert.Throws<DomeinException>(() => new Antwoord(fouteTekst, true));
        }

        [Fact]
        public void Equals_ZelfdeTekstVerschillendeCasing_ReturnsTrue() {
            // Arrange
            Antwoord a1 = new Antwoord("Parijs", true);
            Antwoord a2 = new Antwoord("parijs", false);

            // Act
            bool resultaat = a1.Equals(a2);

            // Assert
            Assert.True(resultaat);
        }
    }
}
