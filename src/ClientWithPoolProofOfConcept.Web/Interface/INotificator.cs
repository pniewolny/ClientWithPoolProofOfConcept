using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClientWithPoolProofOfConcept.Web.Messages;

namespace ClientWithPoolProofOfConcept.Web.Interface
{
    [ServiceContract]
    public interface INotificator
    {
        [OperationContract(IsOneWay = true, AsyncPattern = true, Action = NotificationData.NotificationAction)]
        IAsyncResult BeginNotify(Message message, AsyncCallback callback, object state);
        void EndNotify(IAsyncResult result);
    }
}