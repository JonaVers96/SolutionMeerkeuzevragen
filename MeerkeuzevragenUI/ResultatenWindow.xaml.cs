using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MeerkeuzevragenBL.Gebruikers;
using MeerkeuzevragenBL.Managers;

namespace MeerkeuzevragenUI {
    /// <summary>
    /// Interaction logic for ResultatenWindow.xaml
    /// </summary>
    public partial class ResultatenWindow : Window {

        private MeerkeuzevragenManager _manager;
        public ResultatenWindow(MeerkeuzevragenManager manager) {
            InitializeComponent();
            _manager = manager;

            try {
                // Pas de titel dynamisch aan op basis van wie er kijkt
                if (manager.IngelogdeGebruiker is Leerling) {
                    txtTitel.Text = "Mijn Behaalde Resultaten";
                } else {
                    txtTitel.Text = "Overzicht Alle Resultaten (Leerkracht)";
                    btnWisResultaten.Visibility = Visibility.Visible;
                }

                // Vul de DataGrid via onze gecorrigeerde managermethode
                dgResultaten.ItemsSource = manager.HaalMijnResultatenOp();
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij laden van resultaten: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnWisResultaten_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult bevestiging = MessageBox.Show(
                "Weet je zeker dat je alle behaalde resultaten wilt wissen? Dit kan niet ongedaan worden gemaakt.",
                "Waarschuwing",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            // Als de leerkracht op 'Yes' klikt:
            if (bevestiging == MessageBoxResult.Yes) {
                try {
                    // Wis de data in de databank
                    _manager.WisAlleResultaten();

                    // Herlaad het scherm (de DataGrid zal nu leeg worden)
                    dgResultaten.ItemsSource = _manager.HaalMijnResultatenOp();

                    MessageBox.Show("Alle resultaten zijn succesvol gewist.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Fout bij wissen", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
