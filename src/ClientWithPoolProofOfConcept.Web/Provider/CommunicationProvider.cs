using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using ClientWithPoolProofOfConcept.Shared.Web;
using ClientWithPoolProofOfConcept.Web.Helper;
using ClientWithPoolProofOfConcept.Web.Interface;

namespace ClientWithPoolProofOfConcept.Web.Provider
{
    public class CommunicationProvider
    {
        public event EventHandler<DataChangedMessageArgs> DataChanged;
        private readonly CancellationTokenSource _token = new CancellationTokenSource();
        private int _numberOfPersons;
        private int _numberOfWorkCenters;

        private CommunicationProvider()
        {
            _clients = new Dictionary<Guid, Dictionary<string, INotificator>>();
            _persons = new List<Person>();
            _workCenters = new List<WorkCenter>();

            Task.Run(async () => await CheckForChanges());
        }
        
        private async Task CheckForChanges()
        {
            while (_token.IsCancellationRequested == false)
            {
                var p = _persons.Count;
                var w = _workCenters.Count;
                if (_numberOfPersons != p || _numberOfWorkCenters != w)
                {
                    var diffP = p - _numberOfPersons;
                    var diffW = w - _numberOfWorkCenters;
                    
                    _numberOfPersons = p;
                    _numberOfWorkCenters = w;

                    var personMsg = "Number of persons:" + p + " (+" + diffP + ")";
                    var workCenterMsg = "Number of workcenters:" + w + " (+" + diffW + ")";

                    if (DataChanged != null)
                    {
                        DataChanged(this, new DataChangedMessageArgs
                        {
                            Message = string.Format("{0}{1}{2}",
                                personMsg,
                                Environment.NewLine,
                                workCenterMsg)
                        });
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private static CommunicationProvider _instance;
        public static CommunicationProvider Instance
        {
            get
            {
                return _instance ?? (_instance = new CommunicationProvider());
            }
        }

        private readonly Dictionary<Guid, Dictionary<string, INotificator>> _clients;
        public Dictionary<string, INotificator> Channels
        {
            get
            {
                return _clients.Values.SelectMany(keyValue => keyValue).ToDictionary(key => key.Key, key => key.Value);
            }
        }

        private readonly List<Person> _persons;
        private readonly List<WorkCenter> _workCenters; 

        public int LogIn(Guid owner)
        {
            if (_clients.ContainsKey(owner) == false)
            {
                var sessionId = OperationContext.Current.Channel.SessionId;
                var callbackChannel = OperationContext.Current.GetCallbackChannel<INotificator>();

                var sessionCallback = new Dictionary<string, INotificator>();
                sessionCallback.Add(sessionId, callbackChannel);
                
                _clients.Add(owner, sessionCallback);
            }
            return _clients.Count;
        }

        public void AddPerson(Person person)
        {
            _persons.Add(person);
        }

        public void AddWorkCenter(WorkCenter workCenter)
        {
            _workCenters.Add(workCenter);
        }

        public DataDto LoadData()
        {
            var dto = new DataDto();
            dto.Persons.AddRange(_persons);
            dto.WorkCenters.AddRange(_workCenters);
            return dto;
        }
    }
}