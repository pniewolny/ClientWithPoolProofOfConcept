using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using ClientWithPoolProofOfConcept.Shared.Web;
using ClientWithPoolProofOfConcept.Web.Helper;
using ClientWithPoolProofOfConcept.Web.Interface;
using ClientWithPoolProofOfConcept.Web.Messages;
using ClientWithPoolProofOfConcept.Web.Provider;

namespace ClientWithPoolProofOfConcept.Web.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class CommunicationService : ICommunicationService
    {
        private static readonly TypedMessageConverter _messageConverter = TypedMessageConverter.Create(
            typeof(NotificationData),
            NotificationData.NotificationAction,
            "http://schemas.datacontract.org/2004/07/ClientWithPoolProofOfConcept.Web.Messages");
        private static readonly AsyncCallback _onNotifyCompleted = NotifyCompleted;

        public CommunicationService()
        {
            CommunicationProvider.Instance.DataChanged += InstanceOnDataChanged;
        }

        public int LogIn(Guid owner)
        {
            return CommunicationProvider.Instance.LogIn(owner);
        }

        private void InstanceOnDataChanged(object sender, DataChangedMessageArgs e)
        {
            NotifyClient(null, e.Message);
        }

        private void NotifyClient(string sessionId, string message)
        {
            List<INotificator> clientsToNotify;
            
            if (string.IsNullOrEmpty(sessionId) == false)
            {
                clientsToNotify = CommunicationProvider.Instance.Channels.Where(_ => _.Key != sessionId)
                    .Select(_ => _.Value)
                    .ToList();
            }
            else
            {
                clientsToNotify = CommunicationProvider.Instance.Channels.Select(_ => _.Value).ToList();
            }

            if (clientsToNotify.Count > 0)
            {
                var notificationMessageBuffer = _messageConverter.ToMessage(new NotificationData
                {
                    Content = message
                }).CreateBufferedCopy(65536);
                foreach (var callbackChannel in clientsToNotify)
                {
                    try
                    {
                        callbackChannel.BeginNotify(notificationMessageBuffer.CreateMessage(), _onNotifyCompleted, callbackChannel);
                    }
                    catch (CommunicationException)
                    {
                    }
                }
            }
        }

        public void AddPerson(Person person)
        {
            CommunicationProvider.Instance.AddPerson(person);
            if (person.AddedManually)
            {
                NotifyClient(OperationContext.Current.Channel.SessionId, "Person added manually");
            }
        }

        public void AddWorkCenter(WorkCenter workCenter)
        {
            CommunicationProvider.Instance.AddWorkCenter(workCenter);
            if (workCenter.AddedManually)
            {
                NotifyClient(OperationContext.Current.Channel.SessionId, "WorkCenter added manually");
            }
        }

        public DataDto LoadData()
        {
            return CommunicationProvider.Instance.LoadData();
        }

        static void NotifyCompleted(IAsyncResult result)
        {
            try
            {
                ((INotificator)(result.AsyncState)).EndNotify(result);
            }
            catch (CommunicationException)
            {
            }
            catch (TimeoutException)
            {
            }
        }
    }
}
