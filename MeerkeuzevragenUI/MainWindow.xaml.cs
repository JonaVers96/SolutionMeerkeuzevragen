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

        public MainWindow() {
            InitializeComponent();
            InitialiseerManager();
            LaadOnderwerpen();
        }

        private void InitialiseerManager() {
            try {
                IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration config = builder.Build();
                string connString = config.GetConnectionString("MijnDatabank");

                _manager = ManagerFactory.CreateManager(connString);
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij initialiseren: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LaadOnderwerpen() {
            try {
                // Haal de lijst op via de manager
                List<Onderwerp> onderwerpen = _manager.HaalAlleOnderwerpenOp();

                // Koppel de lijst aan de ComboBox in XAML
                cmbOnderwerpen.ItemsSource = onderwerpen;
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij laden van onderwerpen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbOnderwerpen_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                // Welk onderwerp is geselecteerd?
                if (cmbOnderwerpen.SelectedItem is Onderwerp geselecteerdOnderwerp) {
                    // Pas de titel bovenaan de lijst aan
                    txtGekozenOnderwerp.Text = $"Vragen voor onderwerp: {geselecteerdOnderwerp.Naam}";

                    // Haal de vragen op via de manager en vul de DataGrid!
                    dgVragen.ItemsSource = _manager.HaalVragenOpPerOnderwerp(geselecteerdOnderwerp);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij ophalen vragen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event dat afgaat zodra je op een Vinkje (Actief in/uit) klikt
        private void ActiefCheckBox_Click(object sender, RoutedEventArgs e) {
            try {
                // Achterhaal op welke checkbox van welke vraag je geklikt hebt
                if (sender is CheckBox cb && cb.DataContext is Vraag geselecteerdeVraag) {
                    // Dankzij de 'Binding' is geselecteerdeVraag.IsActief nu al automatisch True of False geworden.
                    // We hoeven de vraag enkel nog naar de database te sturen om te updaten!
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
                // 10 is hier het aantal vragen dat in de test komt. Je kan hier later een invulvakje voor maken!
                TestAfleggenWindow testVenster = new TestAfleggenWindow(_manager, geselecteerdOnderwerp, 10);
                testVenster.Owner = this;
                testVenster.ShowDialog();
            } else {
                MessageBox.Show("Kies eerst een onderwerp uit de lijst links.", "Opgelet", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}