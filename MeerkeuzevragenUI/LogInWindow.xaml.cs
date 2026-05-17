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
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MeerkeuzevragenFactory;

namespace MeerkeuzevragenUI {
    public partial class LogInWindow : Window {
        private MeerkeuzevragenManager _manager;

        public LogInWindow() {
            InitializeComponent();
            InitialiseerManager();
        }

        // We initialiseren de manager NU al in het inlogscherm!
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
                MessageBox.Show($"Fout bij verbinden met database: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e) {
            if (_manager == null) return; // Beveiliging

            if (_manager.Login(txtNaam.Text)) {
                // Geef de werkende manager mét de ingelogde gebruiker door aan het hoofdscherm!
                MainWindow main = new MainWindow(_manager);
                main.Show();
                this.Close();
            } else {
                MessageBox.Show("Gebruiker niet gevonden in de database. Typ een geldige naam in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
