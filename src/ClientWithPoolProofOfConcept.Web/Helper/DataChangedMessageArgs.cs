using System;

namespace ClientWithPoolProofOfConcept.Web.Helper
{
    public class DataChangedMessageArgs : EventArgs
    {
        public string Message { get; set; }
    }
}