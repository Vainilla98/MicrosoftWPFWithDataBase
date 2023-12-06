using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DI_UT1_Ej24_MySQL_ContexMenu_Settings.Properties;
using MySql.Data.MySqlClient;

namespace DI_UT1_Ej24_MySQL_ContexMenu_Settings
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Declaro la conexion a la base de datos
        MySqlConnection dbCon = null;
        public MainWindow()
        {
            InitializeComponent();
            CrearConexionDB();
            CargarComboContiente();
        }

        private void CrearConexionDB() {
            dbCon = new MySqlConnection();
            string server = Settings.Default.server;
            string database = Settings.Default.database;
            string uid = "sql11651753";
            string password = "Z8VININtri";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            dbCon = new MySqlConnection(connectionString);
        }

        private void CargarComboContiente()
        {
            try 
            {
                // Abrimos la conexión
                if (dbCon.State != System.Data.ConnectionState.Open)
                {
                    // Abrimos la conexión
                    dbCon.Open();
                }
                string consulta = "SELECT distinct continent FROM country ORDER BY continent asc ";

                // Creamos el comando
                MySqlCommand comando = new MySqlCommand(consulta,dbCon);

                // Creamous un dataReaader ejecutando el comando
                MySqlDataReader lector = comando.ExecuteReader();

                // Leemos los datos y los almacenamos en el ComboBox
                while (lector.Read())
                {
                    // Añadimos nuevo continente
                    cmb_continente.Items.Add(new { continent = lector["continent"].ToString() });
                    
                }
                // Indicamos que campo sera el mostrado y cual será el que tenga el valor
                cmb_continente.SelectedValuePath = "continent";
                cmb_continente.DisplayMemberPath = "continent";

                // Cerroamos el DataReader y la Conexión
                lector.Close();
                dbCon.Close();

            } catch (MySqlException) {
                MessageBox.Show("Comprueba que la base de datos esta disponible", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CargarComboPaises()
        {
            try
            {
                if (dbCon.State != System.Data.ConnectionState.Open)
                {
                    // Abrimos la conexión
                    dbCon.Open();
                }
                string consulta = "SELECT code, name FROM country WHERE continent = '"+cmb_continente.SelectedValue+"' ORDER BY name ";

                // Creamos el comando
                MySqlCommand comando = new MySqlCommand(consulta, dbCon);

                // Creamous un dataReaader ejecutando el comando
                MySqlDataReader lector = comando.ExecuteReader();

                // Leemos los datos y los almacenamos en el ComboBox
                while (lector.Read())
                {
                    // Añadimos nuevo pais
                    cmb_pais.Items.Add(new { code = lector["code"].ToString(), name = lector["name"].ToString() });

                }
                // Indicamos que campo sera el mostrado y cual será el que tenga el valor
                cmb_pais.SelectedValuePath = "code";
                cmb_pais.DisplayMemberPath = "name";

                // Cerroamos el DataReader y la Conexión
                lector.Close();
                dbCon.Close();

            }
            catch (MySqlException)
            {
                MessageBox.Show("Comprueba que la base de datos esta disponible", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmb_continente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmb_pais.Items.Clear();
            CargarComboPaises();
        }

        private void BuscarCiudades()
        {
            try
            {
                if (dbCon.State != System.Data.ConnectionState.Open)
                {
                    // Abrimos la conexión
                    dbCon.Open();
                }
                string consulta = "SELECT id, name, countrycode, district, population FROM city " +
                    "WHERE countrycode = '" +cmb_pais.SelectedValue+ "' ORDER BY name";

                // Creamos el comando
                MySqlCommand comando = new MySqlCommand(consulta, dbCon);

                // Creamos un DataAdaapter y ejecutamos el comando
                MySqlDataAdapter da = new MySqlDataAdapter(comando);
                
                //Introducimos los datos recuperados en un DataTable
                DataTable dT = new DataTable();
                da.Fill(dT);

                // Asociamos el DataTable al DataGrid que tenemos
                dtg_Ciudad.ItemsSource = dT.DefaultView;

                // Cerroamos la Conexión
                dbCon.Close();

                // Dejamos no visible la columna del id
                dtg_Ciudad.Columns[0].Visibility = Visibility.Collapsed;
            }
            catch (MySqlException)
            {
                MessageBox.Show("Comprueba que la base de datos esta disponible", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_buscar_Click(object sender, RoutedEventArgs e)
        {
            BuscarCiudades();
        }

        private void btn_annadir_Click(object sender, RoutedEventArgs e)
        {
            AnnadirCiudadWindow annadirCiudad = new AnnadirCiudadWindow(dbCon);
            if (cmb_pais.SelectedItem != null)
            {
                annadirCiudad.SetCodigo(cmb_pais.SelectedValue.ToString());
            }
            if (annadirCiudad.ShowDialog() == true)
            {
                BuscarCiudades();
            }
        }

        private void mnu_Eliminar_Click(object sender, RoutedEventArgs e)
        {
            EliminarCiudad();
        }

        private void EliminarCiudad()
        {
            if (dtg_Ciudad.SelectedItems.Count > 0)
            {
                DataRowView ciudad = dtg_Ciudad.SelectedItems[0] as DataRowView;
                string idCiudad = ciudad[0].ToString();
                string nombreCiudad = ciudad[1].ToString();

                if (MessageBox.Show("¿Está seguro/a de eliminar "+nombreCiudad+"?","Atencion",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes )
                {
                    try
                    {
                        dbCon.Open();

                        // Preparamos la entencioa SQL
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = dbCon;
                        string consulta = "DELETE FROM city WHERE id ="+idCiudad;
                        cmd.CommandText = consulta;
                        // Ejecutamos la consulta
                        cmd.ExecuteNonQuery();
                        // Cerramos la consulta
                        dbCon.Close();
                    }
                    catch (MySqlException)
                    {
                        MessageBox.Show("Compurebe que la base de datos está disponible", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    BuscarCiudades();
                }
            }
        }
    }
}
