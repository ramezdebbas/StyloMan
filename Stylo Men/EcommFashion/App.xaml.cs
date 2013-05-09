using EcommFashion.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using EcommFashion.SettingsFlyouts;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace EcommFashion
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        double settingsWidth = 370;
        Popup settingsPopup;
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
           protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();
            if (!rootFrame.Navigate(typeof(GroupedItemsPage), "AllGroups"))
            {
                throw new Exception("Failed to create initial page");
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
           //private void OnSuspending(object sender, SuspendingEventArgs e)
           //{
           //    var deferral = e.SuspendingOperation.GetDeferral();
           //    //TODO: Save application state and stop any background activity
           //    deferral.Complete();
           //}

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
        }

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            UICommandInvokedHandler handler = new UICommandInvokedHandler(onSettingsCommand);

            SettingsCommand aboutCommand = new SettingsCommand("AU", "About Us", handler);
            args.Request.ApplicationCommands.Add(aboutCommand);

          /* // SettingsCommand contactCommand = new SettingsCommand("CU", "Contact Us", handler);
           // args.Request.ApplicationCommands.Add(contactCommand);

            SettingsCommand privacyCommand = new SettingsCommand("PP", "Privacy Policy", handler);
            args.Request.ApplicationCommands.Add(privacyCommand);

           // SettingsCommand termsCommand = new SettingsCommand("TC", "Terms and Conditions", handler);
           // args.Request.ApplicationCommands.Add(termsCommand); */
        }

        private void onSettingsCommand(IUICommand command)
        {
            Rect windowBounds = Window.Current.Bounds;
            settingsPopup = new Popup();

            settingsPopup.Closed += settingsPopup_Closed;
            Window.Current.Activated += Current_Activated;
            settingsPopup.IsLightDismissEnabled = true;
            Page settingPage = null;

            switch (command.Id.ToString())
            {
                case "AU":
                    settingPage = new AboutUs();
                    break;
                case "CU":
                    settingPage = new ContactUs();
                    break;
                case "PP":
                    settingPage = new PrivacyPolicy();
                    break;
                case "TC":
                    settingPage = new TermsAndConditions();
                    break;
            }

            settingsPopup.Width = settingsWidth;
            settingsPopup.Height = windowBounds.Height;

            // Add the proper animation for the panel.
            settingsPopup.ChildTransitions = new TransitionCollection();
            settingsPopup.ChildTransitions.Add(new PaneThemeTransition()
            {

                Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) ?
                EdgeTransitionLocation.Right :
                EdgeTransitionLocation.Left
            });

            if (settingPage != null)
            {
                settingPage.Width = settingsWidth;
                settingPage.Height = windowBounds.Height;
            }

            // Place the SettingsFlyout inside our Popup window.
            settingsPopup.Child = settingPage;

            // Let's define the location of our Popup.
            settingsPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (windowBounds.Width - settingsWidth) : 0);
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
    

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                EcommFashion.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await EcommFashion.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (EcommFashion.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(SearchResultsPage1), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }
    }
}
