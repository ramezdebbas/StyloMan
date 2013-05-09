using System;
using EcommFashion.Data;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace EcommFashion
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ItemDetail : EcommFashion.Common.LayoutAwarePage
    {
        public static Popup settingsPopup = null;
        public ItemDetail()
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
            EnableLiveTile.CreateLiveTile.ShowliveTile(true, "Stylo Men");
            // Allow saved page state to override the initial item to display

        }
        

        /// <summary>
        
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        private void btnMyCart_Click(object sender, RoutedEventArgs e)
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

        public void image_Loaded(object sender, NavigationEventArgs e)
        {
          
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var data = e.Parameter as SampleDataCommon;
            image.Source = data.Image;
            content.Text = data.Description;
            base.OnNavigatedTo(e);
        }
    }
}
