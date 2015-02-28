using System;
using System.Runtime.Serialization;

namespace ClientWithPoolProofOfConcept.Shared.Web
{
    [DataContract]
    public class Person
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int ClientFk { get; set; }

        [DataMember]
        public bool AddedManually { get; set; }
    }
}