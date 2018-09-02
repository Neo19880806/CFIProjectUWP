using CFIProjectUWP.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CFIProjectUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CFIMainPage : Page
    {
        private ObservableCollection<CFIDetail> mQueryList = new ObservableCollection<CFIDetail>();
        public CFIMainPage()
        {
            this.InitializeComponent();
        }

        private void controlsButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
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

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    String sqlQueryString = String.Format("select CRN,tblsubjectcompetencies.ITSubject,`Course Title`,`Meeting Start Date`," +
                        "`Meeting Finish Date`,Day_Of_Week,Time,Room,Lecturer,Campus from tblsiscrns_sr004_2016_s2 " +
                        "left join tblsubjectcompetencies on tblsiscrns_sr004_2016_s2.`Course Code`=tblsubjectcompetencies.CourseNumber " +
                        "where `Course Title` = \"{0}\" and Day_Of_Week!='0'",
                        cmbValidSubject.SelectedValue);

                    connection.Open();
                    MySqlCommand getCommand = connection.CreateCommand();
                    getCommand.CommandText = sqlQueryString;
                    using (MySqlDataReader reader = getCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CFIDetail detail = new CFIDetail();
                            detail.CRN = reader.GetString("CRN");
                            detail.SubjectCode = reader.GetString("ITSubject");
                            detail.CompetencyName = reader.GetString("Course Title");
                            detail.StartDate = reader.GetString("Meeting Start Date");
                            detail.EndDate = reader.GetString("Meeting Finish Date");
                            detail.DayOfWeek = reader.GetString("Day_Of_Week");
                            detail.Time = reader.GetString("Time");
                            detail.Room = reader.GetString("Room");
                            detail.Lecturer = reader.GetString("Lecturer");
                            detail.Campus = reader.GetString("Campus");
                            mQueryList.Add(detail);
                        }

                        if(mQueryList.Count >0)
                        {
                            btnFiltering.IsEnabled = true;
                            btnSorting.IsEnabled = true;
                        }
                        else
                        {
                            btnFiltering.IsEnabled = false;
                            btnSorting.IsEnabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Couldn't connect to database!");
                await dialog.ShowAsync();
            }
            finally
            {


            }
        }

        private async void StackPanel_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }
    }
}
