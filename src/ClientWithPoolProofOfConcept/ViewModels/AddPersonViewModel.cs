using System;
using System.Windows;
using ClientWithPoolProofOfConcept.Messages;
using ClientWithPoolProofOfConcept.Shared.Web;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ClientWithPoolProofOfConcept.ViewModels
{
    public class AddPersonViewModel: ViewModelBase
    {
        public AddPersonViewModel()
        {
            Messenger.Default.Register<AddPersonMessage>(this, Initialize);
        }

        private void Initialize(AddPersonMessage e)
        {
            Person = new Person
            {
                ClientFk = e.ClientFk, 
                Id = Guid.NewGuid()
            };
        }

        private Person _person;
        public Person Person
        {
            get { return _person; }
            set 
            { 
                _person = value; 
                RaisePropertyChanged("Person");
            } 
        }

        private RelayCommand<RoutedEventArgs> _saveCommand;
        public RelayCommand<RoutedEventArgs> SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand<RoutedEventArgs>(Save)); }
        }

        public void Save(RoutedEventArgs e)
        {
	        Messenger.Default.Send(new CreatePersonMessage
	        {
	            Person = Person
	        });
            Messenger.Default.Send(new CloseMessage());
        }

        private RelayCommand<RoutedEventArgs> _closeCommand;
        public RelayCommand<RoutedEventArgs> CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand<RoutedEventArgs>(Close)); }
        }

        public void Close(RoutedEventArgs e)
        {
            Messenger.Default.Send(new CloseMessage());
        }

        public override void Cleanup()
        {
            Messenger.Default.Unregister<AddPersonMessage>(this, Initialize);

            base.Cleanup();
        }
    }
}
