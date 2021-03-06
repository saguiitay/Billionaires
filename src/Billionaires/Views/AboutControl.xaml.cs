﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;

namespace Billionaires.Views
{
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();
        }

        private void WebsiteClick(object sender, RoutedEventArgs e)
        {
            var task = new WebBrowserTask
                {
                    Uri = new Uri("http://itaysagui.wordpress.com")
                };
            task.Show();
        }

        private void RateClick(object sender, RoutedEventArgs e)
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        private void OtherAppsClick(object sender, RoutedEventArgs e)
        {
            var task = new MarketplaceSearchTask
                {
                    ContentType = MarketplaceContentType.Applications,
                    SearchTerms = "saguiitay"
                };
            task.Show();
        }
    }
}
