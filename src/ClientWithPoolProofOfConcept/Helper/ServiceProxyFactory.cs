using System.Globalization;
using System.ServiceModel;
using ClientWithPoolProofOfConcept.ClientService;

namespace ClientWithPoolProofOfConcept.Helper
{
    public static class ServiceProxyFactory
    {
        public static CommunicationServiceClient GetCommunicationServiceClient()
        {
            return new CommunicationServiceClient(new PollingDuplexHttpBinding(),
                new EndpointAddress(
                    "http://"
                    + App.Current.Host.Source.DnsSafeHost
                    + ":"
                    + App.Current.Host.Source.Port.ToString(CultureInfo.InvariantCulture)
                    + "/Service/CommunicationService.svc"));
        }
    }
}
