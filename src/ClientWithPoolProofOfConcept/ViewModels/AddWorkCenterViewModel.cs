using System;
using System.Windows;
using ClientWithPoolProofOfConcept.Messages;
using ClientWithPoolProofOfConcept.Shared.Web;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ClientWithPoolProofOfConcept.ViewModels
{
    public class AddWorkCenterViewModel : ViewModelBase
    {
        public AddWorkCenterViewModel()
        {
            Messenger.Default.Register<AddWorkCenterMessage>(this, Initialize);
        }

        private void Initialize(AddWorkCenterMessage e)
        {
            WorkCenter = new WorkCenter
            {
                ClientFk = e.ClientFk, 
                Id = Guid.NewGuid(), 
                CreationDate = DateTime.Now
            };
        }

        private WorkCenter _workCenter;
        public WorkCenter WorkCenter
        {
            get { return _workCenter; }
            set 
            { 
                _workCenter = value; 
                RaisePropertyChanged("WorkCenter");
            } 
        }

        private RelayCommand<RoutedEventArgs> _saveCommand;
        public RelayCommand<RoutedEventArgs> SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand<RoutedEventArgs>(Save)); }
        }

        public void Save(RoutedEventArgs e)
        {
            Messenger.Default.Send(new CreateWorkCenterMessage()
            {
                WorkCenter = WorkCenter
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
            Messenger.Default.Unregister<AddWorkCenterMessage>(this, Initialize);

            base.Cleanup();
        }
    }
}
