using System;
using System.Windows;
using ClientWithPoolProofOfConcept.ClientService;
using ClientWithPoolProofOfConcept.Messages;
using ClientWithPoolProofOfConcept.Web.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace ClientWithPoolProofOfConcept.Helper
{
    public class User
    {
        public Guid Id { get; private set; }
        public int ClientFk { get; internal set; }
        public CommunicationServiceClient Client { get; private set; }

        internal User()
        {
            Id = Guid.NewGuid();
            Client = ServiceProxyFactory.GetCommunicationServiceClient();
        }
    }

    public static class UserSessionManager
    {
        public static event EventHandler UserCreatedAndLoggedIn;
        public static void CreateUser()
        {
            CurrentUser = new User();

            CurrentUser.Client.LogInCompleted += ClientOnLogInCompleted;
            CurrentUser.Client.LogInAsync(CurrentUser.Id);
        }

        private static void ClientOnLogInCompleted(object sender, LogInCompletedEventArgs e)
        {
            CurrentUser.Client.LogInCompleted -= ClientOnLogInCompleted;
            CurrentUser.ClientFk = e.Result;

            CurrentUser.Client.NotifyReceived += ClientOnNotifyReceived;

            if (UserCreatedAndLoggedIn != null)
                UserCreatedAndLoggedIn(null, EventArgs.Empty);
        }

        private static void ClientOnNotifyReceived(object sender, NotifyReceivedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show( "ERROR: " + e.Error.Message, "Error", MessageBoxButton.OK);
                return;
            }

            Messenger.Default.Send(new DataChangedMessage
                {
                    Message = e.request.GetBody<NotificationData>().Content
                });
        }

        public static User CurrentUser { get; private set; }
    }
}
