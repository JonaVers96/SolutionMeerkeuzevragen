using MeerkeuzevragenBL.Model;
using MeerkeuzevragenBL.Exceptions;

namespace MeerkeuzevragenTests {
    public class OnderwerpTests {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Naam_NullOfLeeg_GooitDomeinException(string fouteNaam) {
            // Arrange & Act & Assert
            Assert.Throws<DomeinException>(() => new Onderwerp(fouteNaam));
        }
    }
}