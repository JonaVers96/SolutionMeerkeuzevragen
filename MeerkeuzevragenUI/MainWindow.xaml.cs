using System.Configuration;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MeerkeuzevragenBL.Gebruikers;
using MeerkeuzevragenBL.Managers;
using MeerkeuzevragenBL.Model;
using MeerkeuzevragenFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


namespace MeerkeuzevragenUI {

    public partial class MainWindow : Window {

        private MeerkeuzevragenManager _manager;

        public MainWindow(MeerkeuzevragenManager manager) {
            InitializeComponent();
            _manager = manager;

            StelRechtenIn();
            LaadOnderwerpen();
        }

        private void StelRechtenIn() {
            if (_manager.IngelogdeGebruiker is Leerling leerling) {
                buttonNieuweVraag.Visibility = Visibility.Collapsed;
                dgVragen.Visibility = Visibility.Collapsed;
            } else if (_manager.IngelogdeGebruiker is Leerkracht leerkracht) {
                buttonNieuweVraag.Visibility = Visibility.Visible;
                dgVragen.Visibility = Visibility.Visible;
            }
        }

        private void LaadOnderwerpen() {
            try {
                List<Onderwerp> onderwerpen = _manager.HaalAlleOnderwerpenOp();

                cmbOnderwerpen.ItemsSource = onderwerpen;
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij laden van onderwerpen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbOnderwerpen_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                if (cmbOnderwerpen.SelectedItem is Onderwerp geselecteerdOnderwerp) {
                    txtGekozenOnderwerp.Text = $"Vragen voor onderwerp: {geselecteerdOnderwerp.Naam}";

                    dgVragen.ItemsSource = _manager.HaalVragenOpPerOnderwerp(geselecteerdOnderwerp);
                }
            }
            catch (Exception ex) {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                MessageBox.Show($"Fout bij ophalen vragen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActiefCheckBox_Click(object sender, RoutedEventArgs e) {
            try {
                if (sender is CheckBox cb && cb.DataContext is Vraag geselecteerdeVraag) {
                    _manager.UpdateVraagActiefStaat(geselecteerdeVraag);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij het opslaan van de status: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonNieuweVraag_Click(object sender, RoutedEventArgs e) {
            if (cmbOnderwerpen.SelectedItem is Onderwerp geselecteerdOnderwerp) {
                NieuweVraagWindow venster = new NieuweVraagWindow(_manager, geselecteerdOnderwerp);

                venster.Owner = this;

                venster.ShowDialog();

                dgVragen.ItemsSource = _manager.HaalVragenOpPerOnderwerp(geselecteerdOnderwerp);
            } else {
                MessageBox.Show("Kies eerst een onderwerp uit de lijst links voordat je een vraag toevoegt.", "Opgelet", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonTestGenereren_Click(object sender, RoutedEventArgs e) {
            if (cmbOnderwerpen.SelectedItem is Onderwerp geselecteerdOnderwerp) {
                TestAfleggenWindow testVenster = new TestAfleggenWindow(_manager, geselecteerdOnderwerp, 10);
                testVenster.Owner = this;
                testVenster.ShowDialog();
            } else {
                MessageBox.Show("Kies eerst een onderwerp uit de lijst links.", "Opgelet", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonAfmelden_Click(object sender, RoutedEventArgs e) {
            LogInWindow loginWindow = new LogInWindow();

            loginWindow.Show();

            this.Close();

        }

        private void buttonResultaten_Click(object sender, RoutedEventArgs e) {
            ResultatenWindow venster = new ResultatenWindow(_manager);
            venster.Owner = this;
            venster.ShowDialog();
        }
    }
}