using MySql.Data.MySqlClient;
using NyilvantartoApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Projekt_wpf {
    public partial class MainWindow : Window {
        public static ObservableCollection<Termek> Nyilvantartas { get; } = new ObservableCollection<Termek>();
        private int selectedIndex = -1; // Nem static, per-példány

        private string connectionString = "server=localhost;database=kisallat_webshop;uid=root;pwd="; // Tedd bele a jelszót ha kell

        public MainWindow() {
            InitializeComponent();
            dtgAdatok.ItemsSource = Nyilvantartas; // Eleve bindeljük az ObservableCollection-höz
        }

        private void btnBetolt_Click(object sender, RoutedEventArgs e) {
            LoadDataToGrid();
        }

        private void LoadDataToGrid() {
            string query = "SELECT id, nev, ar, akcios_ar, keszlet FROM termekek";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        Nyilvantartas.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            Nyilvantartas.Add(new Termek(
                                Convert.ToInt32(row["id"]),
                                row["nev"].ToString(),
                                Convert.ToInt32(row["ar"]),
                                Convert.ToInt32(row["akcios_ar"]),
                                Convert.ToInt32(row["keszlet"])
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a betöltésnél: " + ex.Message);
            }
        }

        // Új termék hozzáadása (memóriába, de mentés később)
        public static void TermekFelvitel(Termek ujTermek) {
            Nyilvantartas.Add(ujTermek);
        }

        private void btnKilep_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void btnNevjegy_Click(object sender, RoutedEventArgs e) {
            Nevjegy nevjegy = new Nevjegy();
            nevjegy.Show();
        }

        // Módosítás (memóriába, de mentés később)
        public void TermekModositas(Termek modositottTermek) // Nem static, mert index per-példány
        {
            if (selectedIndex >= 0 && selectedIndex < Nyilvantartas.Count)
            {
                Nyilvantartas[selectedIndex] = modositottTermek;
            }
        }

        // Kijelölés esemény (szerkesztés indítása)
        private void dtgAdatok_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (dtgAdatok.SelectedItem is Termek selectedTermek)
            {
                selectedIndex = Nyilvantartas.IndexOf(selectedTermek);
                // Ha automatikusan akarod nyitni a módosító ablakot, ide tedd:
                // new Modositas(selectedTermek, "Modify").ShowDialog();
                // De jobb, ha külön gomb van a szerkesztésre.
            }
        }

        // ÚJ: Gomb az új termék hozzáadásához (add hozzá XAML-ben: <Button Content="Új" Click="btnUj_Click"/>)
        private void btnUj_Click(object sender, RoutedEventArgs e) {
            var ujTermek = new Termek(0, "", 0, 0, 0); // ID=0, majd DB generálja
            new Modositas(ujTermek, "Add").ShowDialog();
        }

        // Mentés gomb: Visszaírja az összes változást DB-be
        private void btnMent_Click(object sender, RoutedEventArgs e) {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var termek in Nyilvantartas)
                    {
                        if (termek.Id == 0) // Új termék (ID=0 alapján)
                        {
                            string insertQuery = "INSERT INTO termekek (id, nev, ar, akcios_ar, keszlet) VALUES (@id, @nev, @ar, @akcios_ar, @keszlet)";
                            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@id", termek.Id);
                                cmd.Parameters.AddWithValue("@nev", termek.Nev);
                                cmd.Parameters.AddWithValue("@ar", termek.Ar);
                                cmd.Parameters.AddWithValue("@akcios_ar", termek.Akcios_ar);
                                cmd.Parameters.AddWithValue("@keszlet", termek.Keszlet);
                                cmd.ExecuteNonQuery();

                                // Lekérdezzük az új ID-t
                                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                                termek.Id = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        else // Módosítás
                        {
                            string updateQuery = "UPDATE termekek SET nev = @nev, ar = @ar, akcios_ar = @akcios_ar, keszlet = @keszlet WHERE id = @id";
                            using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@id", termek.Id);
                                cmd.Parameters.AddWithValue("@nev", termek.Nev);
                                cmd.Parameters.AddWithValue("@ar", termek.Ar);
                                cmd.Parameters.AddWithValue("@akcios_ar", termek.Akcios_ar);
                                cmd.Parameters.AddWithValue("@keszlet", termek.Keszlet);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                MessageBox.Show("Változások sikeresen mentve az adatbázisba!");
                LoadDataToGrid(); // Frissítjük a gridet
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a mentésnél: " + ex.Message);
            }
        }
    }
}