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

namespace MeerkeuzevragenUI {
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window {
        private MeerkeuzevragenManager _manager;

        public LogInWindow() {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e) {
            if (_manager.Login(txtNaam.Text)) {
                MainWindow main = new MainWindow(_manager);
                main.Show();
                this.Close();
            } else {
                MessageBox.Show("Gebruiker niet gevonden in de database.");
            }
        }
    }
}
