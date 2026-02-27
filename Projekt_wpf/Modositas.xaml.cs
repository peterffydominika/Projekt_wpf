using NyilvantartoApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

namespace Projekt_wpf {
    /// <summary>
    /// Interaction logic for Modositas.xaml
    /// </summary>
    public partial class Modositas : Window {
        string command = "";
        public Modositas(Termek termek, string command) {
            InitializeComponent();
            tbxId.Text = termek.Id.ToString();
            tbxNev.Text = termek.Nev;
            tbxAr.Text = termek.Ar.ToString();
            tbxAkciosAr.Text = termek.Akcios_ar.ToString();
            tbxKeszlet.Text = termek.Keszlet.ToString();
            this.command = command;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            if (command == "Modify")
            {
                MainWindow.TermekModositas(new Termek(int.Parse(tbxId.Text), tbxNev.Text, int.Parse(tbxAr.Text), int.Parse(tbxAkciosAr.Text), int.Parse(tbxKeszlet.Text)));
            }
            else
            {
                MainWindow.TermekFelvitel(new Termek(int.Parse(tbxId.Text), tbxNev.Text, int.Parse(tbxAr.Text), int.Parse(tbxAkciosAr.Text), int.Parse(tbxKeszlet.Text)));
            }
            Close();
        }
    }
}
