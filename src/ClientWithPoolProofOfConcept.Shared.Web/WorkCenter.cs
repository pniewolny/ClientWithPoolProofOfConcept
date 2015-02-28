using System;
using System.Runtime.Serialization;

namespace ClientWithPoolProofOfConcept.Shared.Web
{
    [DataContract]
    public class WorkCenter 
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public int ClientFk { get; set; }

        [DataMember]
        public bool AddedManually { get; set; }
    }
}
