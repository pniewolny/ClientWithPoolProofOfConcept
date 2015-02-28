using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClientWithPoolProofOfConcept.Shared.Web
{
    [DataContract]
    public class DataDto
    {
        public DataDto()
        {
            Persons = new List<Person>();
            WorkCenters = new List<WorkCenter>();
        }

        [DataMember]
        public List<Person> Persons { get; set; }

        [DataMember]
        public List<WorkCenter> WorkCenters { get; set; }
    }
}
