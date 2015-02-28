using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ClientWithPoolProofOfConcept.ClientService;
using ClientWithPoolProofOfConcept.Helper;
using ClientWithPoolProofOfConcept.Messages;
using ClientWithPoolProofOfConcept.Shared.Web;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ClientWithPoolProofOfConcept.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private CommunicationServiceClient _serviceClient;

        public MainPageViewModel()
        {
            _employees = new ObservableCollection<Person>();
            _workCenters = new ObservableCollection<WorkCenter>();
            _logs = new ObservableCollection<string>();

            Messenger.Default.Register<CreatePersonMessage>(this, CreatePerson);
            Messenger.Default.Register<CreateWorkCenterMessage>(this, CreateWorkCenter);
            Messenger.Default.Register<DataChangedMessage>(this, DataChangedOnServer);
        }

        #region Properties

        private int _client;
        public int Client
        {
            get { return _client; }
            set
            {
                _client = value;
                RaisePropertyChanged("Client");
                LoginCommand.RaiseCanExecuteChanged();
                AddPersonCommand.RaiseCanExecuteChanged();
                AddWorkCenterCommand.RaiseCanExecuteChanged();
                RefreshCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<Person> _employees;
        public ObservableCollection<Person> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged("Employees");
            }
        }

        private ObservableCollection<WorkCenter> _workCenters;
        public ObservableCollection<WorkCenter> WorkCenters
        {
            get { return _workCenters; }
            set
            {
                _workCenters = value;
                RaisePropertyChanged("WorkCenters");
            }
        }

        private ObservableCollection<string> _logs;
        public ObservableCollection<string> Logs
        {
            get { return _logs; }
            set 
            { 
                _logs = value; 
                RaisePropertyChanged("Logs");
            } 
        }

        #endregion

        #region Commands

        private RelayCommand<RoutedEventArgs> _loginCommand;
        public RelayCommand<RoutedEventArgs> LoginCommand
        {
            get { return _loginCommand ?? (_loginCommand = new RelayCommand<RoutedEventArgs>(Login, CanLogiIn)); }
        }

        private RelayCommand<RoutedEventArgs> _addWorkCenterCommand;
        public RelayCommand<RoutedEventArgs> AddWorkCenterCommand
        {
            get { return _addWorkCenterCommand ?? (_addWorkCenterCommand = new RelayCommand<RoutedEventArgs>(AddWorkCenter, IsLoggedIn)); }
        }

        private RelayCommand<RoutedEventArgs> _addPersonCommand;
        public RelayCommand<RoutedEventArgs> AddPersonCommand
        {
            get { return _addPersonCommand ?? (_addPersonCommand = new RelayCommand<RoutedEventArgs>(AddPerson, IsLoggedIn)); }
        }

        private RelayCommand<RoutedEventArgs> _refreshCommand;
        public RelayCommand<RoutedEventArgs> RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand<RoutedEventArgs>(Refresh, IsLoggedIn)); }
        }


        #endregion

        #region Methods

        private void DataChangedOnServer(DataChangedMessage e)
        {
            Logs.Add(e.Message);
        }

        private void CreateWorkCenter(CreateWorkCenterMessage e)
        {
            _serviceClient = ServiceProxyFactory.GetCommunicationServiceClient();
            _serviceClient.AddWorkCenterCompleted += ServiceClientOnAddWorkCenterCompleted;
            _serviceClient.AddWorkCenterAsync(e.WorkCenter);

            WorkCenters.Add(e.WorkCenter);
        }

        private void ServiceClientOnAddWorkCenterCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (_serviceClient != null)
            {
                _serviceClient.AddWorkCenterCompleted -= ServiceClientOnAddWorkCenterCompleted;
                _serviceClient.CloseAsync();
                _serviceClient = null;
            }
        }

        private void CreatePerson(CreatePersonMessage e)
        {
            _serviceClient = ServiceProxyFactory.GetCommunicationServiceClient();
            _serviceClient.AddPersonCompleted += ServiceClientOnAddPersonCompleted;
            _serviceClient.AddPersonAsync(e.Person);

            Employees.Add(e.Person);
        }

        private void ServiceClientOnAddPersonCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            if (_serviceClient != null)
            {
                _serviceClient.AddPersonCompleted -= ServiceClientOnAddPersonCompleted;
                _serviceClient.CloseAsync();
                _serviceClient = null;
            }
        }

        private bool CanLogiIn(RoutedEventArgs arg)
        {
            return Client == 0;
        }

        public void Login(RoutedEventArgs e)
        {
            UserSessionManager.UserCreatedAndLoggedIn += UserSessionManagerOnUserCreatedAndLoggedIn;
            UserSessionManager.CreateUser();
        }

        private void UserSessionManagerOnUserCreatedAndLoggedIn(object sender, EventArgs eventArgs)
        {
            UserSessionManager.UserCreatedAndLoggedIn -= UserSessionManagerOnUserCreatedAndLoggedIn;

            Client = UserSessionManager.CurrentUser.ClientFk;
        }

        private bool IsLoggedIn(RoutedEventArgs arg)
        {
            return Client != 0;
        }

        public void AddWorkCenter(RoutedEventArgs e)
        {
            var dialog = new AddWorkCenter();
            dialog.ShowDialog();

            Messenger.Default.Send(new AddWorkCenterMessage
            {
                ClientFk = Client
            });

        }

        public void AddPerson(RoutedEventArgs e)
        {
            var dialog = new AddPerson();
            dialog.ShowDialog();

            Messenger.Default.Send(new AddPersonMessage()
            {
                ClientFk = Client
            });
        }

        public void Refresh(RoutedEventArgs e)
        {
            _serviceClient = ServiceProxyFactory.GetCommunicationServiceClient();
            _serviceClient.LoadDataCompleted += ServiceClientOnLoadDataCompleted;
            _serviceClient.LoadDataAsync();
        }

        private void ServiceClientOnLoadDataCompleted(object sender, LoadDataCompletedEventArgs e)
        {
            var existinPersons = Employees.Select(_ => _.Id).ToList();
            foreach (var person in e.Result.Persons.Where(_ => existinPersons.Contains(_.Id) == false))
            {
                Employees.Add(person);
            }

            var existingWorkCenters = WorkCenters.Select(_ => _.Id).ToList();
            foreach (var workCenter in e.Result.WorkCenters.Where(_ => existingWorkCenters.Contains(_.Id) == false))
            {
                WorkCenters.Add(workCenter);
            }
        }

        #endregion

    }
}
