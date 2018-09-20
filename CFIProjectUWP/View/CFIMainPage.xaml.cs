using CFIProjectUWP.Model;
using CFIProjectUWP.View;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
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
        private List<CFIDetail> mResultList = new List<CFIDetail>();
        private ObservableCollection<CFIDetail> mQueryList = new ObservableCollection<CFIDetail>();
        public CFIMainPage()
        {
            this.InitializeComponent();
        }

        private void controlsButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }

        //private static String connectionString = "database=work;Password=123456;User ID=root;server=127.0.0.1;SslMode=None";
        private static String connectionString = "database=tafebuddy;Password=lgj123456;User ID=cfiproject;server=www.db4free.net;old guids=true;SslMode=None";

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            String selectedSubject = e.Parameter.ToString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    String sqlQueryString = String.Format("select CRN,tblSubjectCompetencies.ITSubject,`Course Title`,`Meeting Start Date`," +
                        "`Meeting Finish Date`,Day_Of_Week,Time,Room,Lecturer,Campus from tblSISCRNs_SR004_2016_S2 " +
                        "left join tblSubjectCompetencies on tblSISCRNs_SR004_2016_S2.`Course Code`=tblSubjectCompetencies.CourseNumber " +
                        "where `Course Title` = \"{0}\" and Day_Of_Week!='0'",
                        selectedSubject);

                    connection.Open();
                    MySqlCommand getCommand = connection.CreateCommand();
                    getCommand.CommandText = sqlQueryString;
                    using (MySqlDataReader reader = getCommand.ExecuteReader())
                    {
                        mResultList.Clear();
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
                            mResultList.Add(detail);
                        }

                        if (mResultList.Count > 0)
                        {
                            btnFiltering.IsEnabled = true;
                            btnSorting.IsEnabled = true;
                        }
                        else
                        {
                            btnFiltering.IsEnabled = false;
                            btnSorting.IsEnabled = false;
                        }
                        RefreshListView(mResultList);
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
            var detailDialog = new CFIDetailDialog(myListView.SelectedItem);
            await detailDialog.ShowAsync();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (mResultList.Count <= 0) return;
            RefreshListView(mResultList);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CFIValidSubjectPage));
        }

        private async void btnSorting_Click(object sender, RoutedEventArgs e)
        {
            if (mResultList.Count <= 0) return;
            CFISortingBYDialog dialog = new CFISortingBYDialog();
            var result = await dialog.ShowAsync();

            List<CFIDetail> list = null;
            switch(dialog.SortBYResult)
            {
                case "Campus":
                    list = mQueryList.OrderBy(x => x.Campus).ToList();
                    break;
                case "Lecturer":
                    list = mQueryList.OrderBy(x => x.Lecturer).ToList();
                    break;
                case "Room":
                    list = mQueryList.OrderBy(x => x.Room).ToList();
                    break;
            default:
                break;
            }

            if (list!=null) { RefreshListView(list); };
        }

        private async void btnFiltering_Click(object sender, RoutedEventArgs e)
        {
            if (mResultList.Count <= 0) return;
            CFIFilteringBYDialog dialog = new CFIFilteringBYDialog();
            var result = await dialog.ShowAsync();

            List<CFIDetail> list = null;
            switch (dialog.FilterBYResult)
            {
                case "Campus":
                    list = mQueryList.Where(x=>x.Campus.Contains(dialog.FilterBYValue)).ToList();
                    break;
                case "Lecturer":
                    list = mQueryList.Where(x=>x.Lecturer.Contains(dialog.FilterBYValue)).ToList();
                    break;
                default:
                    break;
            }

            if (list != null) { RefreshListView(list); };
        }

        private void RefreshListView(List<CFIDetail> list)
        {
            mQueryList.Clear();
            list.ForEach(p => mQueryList.Add(p));
        }
    }
}
