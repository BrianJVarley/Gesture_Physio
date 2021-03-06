﻿using Facebook;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoTestv4.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// <author>Brian Varley</author>
// <summary>HomeView</summary>

namespace MyoTestv4
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {


        private HomeViewModel ViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class.
        /// </summary>
        public HomeView()
        {
            InitializeComponent();
            ViewModel = new HomeViewModel(new UserLoginModel());
            this.DataContext = ViewModel;
            WBrowser.Navigate(new Uri("https://graph.facebook.com/oauth/authorize?client_id=559878724040104&redirect_uri=http://www.facebook.com/connect/login_success.html&type=user_agent&display=popup").AbsoluteUri);
            

        }

        /// <summary>
        /// Handles the OnNavigated event of the WBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        private void WBrowser_OnNavigated(object sender, NavigationEventArgs e)
        {
            ViewModel = ViewModel;
            this.ViewModel.initLogin(e); 
        }
    }
}
