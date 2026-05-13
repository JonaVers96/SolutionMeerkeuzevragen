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
using System;
using MeerkeuzevragenBL.Managers;
using MeerkeuzevragenBL.Model;
using MeerkeuzevragenBL.Enum;

namespace MeerkeuzevragenUI {
    public partial class NieuweVraagWindow : Window {
        private MeerkeuzevragenManager _manager;
        private Onderwerp _onderwerp;

        // Pas de constructor aan zodat hij de Manager en het Onderwerp ontvangt
        public NieuweVraagWindow(MeerkeuzevragenManager manager, Onderwerp onderwerp) {
            InitializeComponent();
            _manager = manager;
            _onderwerp = onderwerp;

            txtGekozenOnderwerp.Text = $"Vraag toevoegen aan: {_onderwerp.Naam}";

            // Vul de combobox met de Enum waarden (Makkelijk, Gemiddeld, Moeilijk)
            cmbMoeilijkheid.ItemsSource = Enum.GetValues(typeof(Moeilijkheid));
            cmbMoeilijkheid.SelectedIndex = 1; // Zet standaard op Gemiddeld
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e) {
            try {
                // 1. Controleer of de velden zijn ingevuld
                if (string.IsNullOrWhiteSpace(txtVraagTekst.Text) ||
                    string.IsNullOrWhiteSpace(txtAntwoord1.Text) ||
                    string.IsNullOrWhiteSpace(txtAntwoord2.Text)) {
                    MessageBox.Show("Vul minstens de vraagtekst en twee antwoorden in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Maak de nieuwe vraag aan
                Moeilijkheid niveau = (Moeilijkheid)cmbMoeilijkheid.SelectedItem;
                Vraag nieuweVraag = new Vraag(txtVraagTekst.Text, niveau, _onderwerp);

                // 3. Voeg de antwoorden toe, kijk welk bolletje is aangevinkt (IsChecked == true)
                nieuweVraag.VoegAntwoordToe(new Antwoord(txtAntwoord1.Text, rbCorrect1.IsChecked == true));
                nieuweVraag.VoegAntwoordToe(new Antwoord(txtAntwoord2.Text, rbCorrect2.IsChecked == true));

                if (!string.IsNullOrWhiteSpace(txtAntwoord3.Text))
                    nieuweVraag.VoegAntwoordToe(new Antwoord(txtAntwoord3.Text, rbCorrect3.IsChecked == true));

                if (!string.IsNullOrWhiteSpace(txtAntwoord4.Text))
                    nieuweVraag.VoegAntwoordToe(new Antwoord(txtAntwoord4.Text, rbCorrect4.IsChecked == true));

                // 4. Sla de vraag op in de database!
                _manager.VoegVraagToe(nieuweVraag);

                MessageBox.Show("Vraag succesvol opgeslagen!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                // Sluit dit pop-up venster
                this.Close();
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
