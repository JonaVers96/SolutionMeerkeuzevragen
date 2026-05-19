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
        private MeerkeuzevragenManager _manager;

        public TestAfleggenWindow(MeerkeuzevragenManager manager, Onderwerp onderwerp, int aantalVragen) {
            InitializeComponent();
            _manager = manager;

            try {
                _huidigeToets = manager.GenereerToets(onderwerp, aantalVragen);
                txtTitel.Text = $"Test: {onderwerp.Naam} ({_huidigeToets.Vragen.Count} vragen)";

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

        private void BtnIndienen_Click(object sender, RoutedEventArgs e) {
            int behaaldeScore = 0;
            int maxScore = _weergaveLijst.Count;

            foreach (VraagWeergave item in _weergaveLijst) {
                Antwoord correctAntwoord = item.DeVraag.Antwoorden.FirstOrDefault(a => a.IsCorrect);

                if (item.GekozenAntwoord == null) {
                    item.FeedbackKleur = "Red";
                    item.FeedbackTekst = $"Niet beantwoord! Het juiste antwoord was: {correctAntwoord?.Tekst}";
                } else if (item.GekozenAntwoord.IsCorrect) {
                    behaaldeScore++;
                    item.FeedbackKleur = "Green";
                    item.FeedbackTekst = "Juist!";
                } else {
                    item.FeedbackKleur = "Red";
                    item.FeedbackTekst = $"Fout! Je koos '{item.GekozenAntwoord.Tekst}'. Het juiste antwoord was: {correctAntwoord?.Tekst}";
                }
            }

            icVragen.ItemsSource = null;
            icVragen.ItemsSource = _weergaveLijst;

            txtTitel.Text = $"Jouw Score: {behaaldeScore} / {maxScore}";
            txtTitel.Foreground = behaaldeScore >= (maxScore / 2.0) ? Brushes.Green : Brushes.Red;

            if (sender is System.Windows.Controls.Button btn) {
                btn.IsEnabled = false;
                btn.Content = "Test Ingediend";
            }

            try {
                _manager.BewaarResultaat(_huidigeToets, behaaldeScore);
            }
            catch (Exception ex) {
                MessageBox.Show($"Score kon niet worden opgeslagen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MessageBox.Show($"Test afgerond!\nJe behaalde {behaaldeScore} op {maxScore}.", "Resultaat", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExporteer_Click(object sender, RoutedEventArgs e) {
            try {

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

                saveFileDialog.Filter = "Tekstbestanden (*.txt)|*.txt";

                saveFileDialog.FileName = $"Toets_{_huidigeToets.Onderwerp.Naam}_{DateTime.Now:yyyyMMdd}";

                if (saveFileDialog.ShowDialog() == true) {
                    string gekozenPad = saveFileDialog.FileName;

                    _manager.ExporteerToetsNaarBestand(_huidigeToets, gekozenPad);

                    MessageBox.Show("De toets is succesvol geëxporteerd en opgeslagen!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij het exporteren van de toets: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
