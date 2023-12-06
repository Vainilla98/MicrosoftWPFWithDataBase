using MySql.Data.MySqlClient;
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

namespace DI_UT1_Ej24_MySQL_ContexMenu_Settings
{
    /// <summary>
    /// Lógica de interacción para AnnadirCiudadWindow.xaml
    /// </summary>
    /// 
    
    public partial class AnnadirCiudadWindow : Window
    {
        MySqlConnection dbCon = null;
        public AnnadirCiudadWindow(MySqlConnection dbCon)
        {
            InitializeComponent();
            this.dbCon = dbCon;
        }

        private void guardarCiudad()
        {
            try 
            { 
                dbCon.Open();

                // Preparamos la entencioa SQL
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = dbCon;
                string consulta = "INSERT INTO city (Name,CountryCode, District, Population) VALUES ('"+txb_nombre.Text+"','"+txb_codigo.Text+"','"+txb_distrito.Text+"','"+txb_poblacion.Text+"')";
                cmd.CommandText = consulta;
                // Ejecutamos la consulta
                cmd.ExecuteNonQuery();
                // Cerramos la consulta
                dbCon.Close(); 
            } catch (MySqlException)
            {
                MessageBox.Show("Compreube que la base de datos está disponible","Error",MessageBoxButton.OK, MessageBoxImage.Error );
            }
        }

        private void btn_aceptar_Click(object sender, RoutedEventArgs e)
        {
            guardarCiudad();
            DialogResult = true;
            Close();
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal void SetCodigo(string codigoPais) 
        { 
            txb_codigo.Text = codigoPais;
        }
    }
}
