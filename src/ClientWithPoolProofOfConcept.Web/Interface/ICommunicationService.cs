using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClientWithPoolProofOfConcept.Shared.Web;

namespace ClientWithPoolProofOfConcept.Web.Interface
{
    [ServiceContract(CallbackContract = typeof(INotificator))]
    public interface ICommunicationService
    {
        [OperationContract]
        int LogIn(Guid owner);

        [OperationContract]
        void AddPerson(Person person);

        [OperationContract]
        void AddWorkCenter(WorkCenter workCenter);

        [OperationContract]
        DataDto LoadData();
    }
}
