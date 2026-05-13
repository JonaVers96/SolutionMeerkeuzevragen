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
using MeerkeuzevragenBL.Managers;
using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenUI {

    public class VraagWeergave {
        public Vraag DeVraag { get; set; }
        public Antwoord GekozenAntwoord { get; set; }
        public string FeedbackTekst { get; set; }
        public string FeedbackKleur { get; set; }
    }
    public partial class TestAfleggenWindow : Window {
        private Toets _huidigeToets;
        private List<VraagWeergave> _weergaveLijst;

        public TestAfleggenWindow(MeerkeuzevragenManager manager, Onderwerp onderwerp, int aantalVragen) {
            InitializeComponent();

            try {
                _huidigeToets = manager.GenereerToets(onderwerp, aantalVragen);
                txtTitel.Text = $"Test: {onderwerp.Naam} ({_huidigeToets.Vragen.Count} vragen)";

                // 2. Vul onze wrapper-lijst in plaats van direct de vragenlijst
                _weergaveLijst = new List<VraagWeergave>();
                foreach (Vraag v in _huidigeToets.Vragen) {
                    _weergaveLijst.Add(new VraagWeergave { DeVraag = v });
                }

                icVragen.ItemsSource = _weergaveLijst;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Fout bij laden test", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        // 3. De logica voor het indienen!
        private void BtnIndienen_Click(object sender, RoutedEventArgs e) {
            int behaaldeScore = 0;
            int maxScore = _weergaveLijst.Count;

            // Overloop elke vraag die op het scherm staat
            foreach (VraagWeergave item in _weergaveLijst) {
                // Zoek het juiste antwoord op uit de lijst
                Antwoord correctAntwoord = item.DeVraag.Antwoorden.FirstOrDefault(a => a.IsCorrect);

                if (item.GekozenAntwoord == null) {
                    // De leerling heeft niets aangevinkt
                    item.FeedbackKleur = "Red";
                    item.FeedbackTekst = $"Niet beantwoord! Het juiste antwoord was: {correctAntwoord?.Tekst}";
                } else if (item.GekozenAntwoord.IsCorrect) {
                    // Het antwoord is JUIST
                    behaaldeScore++;
                    item.FeedbackKleur = "Green";
                    item.FeedbackTekst = "Juist!";
                } else {
                    // Het antwoord is FOUT
                    item.FeedbackKleur = "Red";
                    item.FeedbackTekst = $"Fout! Je koos '{item.GekozenAntwoord.Tekst}'. Het juiste antwoord was: {correctAntwoord?.Tekst}";
                }
            }

            // 4. Update het scherm door de databinding even te verversen
            icVragen.ItemsSource = null;
            icVragen.ItemsSource = _weergaveLijst;

            // Pas de titel aan met de behaalde score
            txtTitel.Text = $"Jouw Score: {behaaldeScore} / {maxScore}";
            txtTitel.Foreground = behaaldeScore >= (maxScore / 2.0) ? Brushes.Green : Brushes.Red;

            // Maak de knop onklikbaar zodat ze niet nog een keer kunnen indienen
            if (sender is System.Windows.Controls.Button btn) {
                btn.IsEnabled = false;
                btn.Content = "Test Ingediend";
            }

            // Toon een mooie pop-up
            MessageBox.Show($"Test afgerond!\nJe behaalde {behaaldeScore} op {maxScore}.", "Resultaat", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
