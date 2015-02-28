using System.ServiceModel;

namespace ClientWithPoolProofOfConcept.Web.Messages
{
    [MessageContract]
    public class NotificationData
    {
        public const string NotificationAction = "http://microsoft.com/samples/pollingDuplex/notification";

        [MessageBodyMember]
        public string Content { get; set; }
    }
}