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
        public ResultatenWindow(MeerkeuzevragenManager manager) {
            InitializeComponent();

            try {
                // Pas de titel dynamisch aan op basis van wie er kijkt
                if (manager.IngelogdeGebruiker is Leerling) {
                    txtTitel.Text = "Mijn Behaalde Resultaten";
                } else {
                    txtTitel.Text = "Overzicht Alle Resultaten (Leerkracht)";
                }

                // Vul de DataGrid via onze gecorrigeerde managermethode
                dgResultaten.ItemsSource = manager.HaalMijnResultatenOp();
            }
            catch (Exception ex) {
                MessageBox.Show($"Fout bij laden van resultaten: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
