using EcommFashion.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace EcommFashion
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class WomenGroupedItemsPage : EcommFashion.Common.LayoutAwarePage
    {
		public static Popup settingsPopup = null;
        public WomenGroupedItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var WomenDataGroups = WomenDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = WomenDataGroups;

            EnableLiveTile.CreateLiveTile.ShowliveTile(false, "ECommerce Fashion");
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((WomenDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
           // var itemId = ((WomenDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetail));
        }

        private void btnMyCart_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
       
        {
            settingsPopup = new Popup();
            Rect windowsBounds = Window.Current.Bounds;
            settingsPopup.Closed += settingsPopup_Closed;
            Window.Current.Activated += Current_Activated;

            settingsPopup.IsLightDismissEnabled = true;
            settingsPopup.Height = windowsBounds.Height;
            myCartPage cartPage = new myCartPage();

            cartPage.Height = windowsBounds.Height;

            settingsPopup.Child = cartPage;
            settingsPopup.SetValue(Canvas.LeftProperty, windowsBounds.Width - 425);
            settingsPopup.SetValue(Canvas.TopProperty, 0);

            settingsPopup.IsOpen = true;
        }

        void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                settingsPopup.IsOpen = false;
            }
        }

        void settingsPopup_Closed(object sender, object e)
        {
            Window.Current.Activated -= Current_Activated;
        }

        private void btnHome_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	 this.Frame.Navigate(typeof(GroupedItemsPage), "AllGroups");
        }

    
    }
}