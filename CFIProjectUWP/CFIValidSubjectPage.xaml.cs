using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CFIProjectUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CFIValidSubjectPage : Page
    {
        public CFIValidSubjectPage()
        {
            this.InitializeComponent();
        }

        private static String connectionString = "database=work;Password=123456;User ID=root;server=127.0.0.1;SslMode=None";

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand getCommand = connection.CreateCommand();
                    getCommand.CommandText = "select DISTINCT `Course Title` from tblsiscrns_sr004_2016_s2 where Day_Of_Week!='0'";
                    using (MySqlDataReader reader = getCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            String subject = reader.GetString("Course Title");
                            cmbValidSubject.Items.Add(subject);
                        }

                        if (cmbValidSubject.Items.Count > 0)
                        {
                            cmbValidSubject.SelectedIndex = 0;
                        }
                    }

                }
            }
            catch (MySqlException)
            {
                MessageDialog dialog = new MessageDialog("Couldn't connect to database!");
                await dialog.ShowAsync();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CFIMainPage),cmbValidSubject.SelectedValue.ToString());
        }
    }
}
