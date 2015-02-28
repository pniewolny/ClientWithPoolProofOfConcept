using ClientWithPoolProofOfConcept.Messages;
using GalaSoft.MvvmLight.Messaging;
using Telerik.Windows.Controls;

namespace ClientWithPoolProofOfConcept.Helper
{
    public class RadWindowBase : RadWindow
    {
        public RadWindowBase()
        {
            Messenger.Default.Register<CloseMessage>(this, CloseDialog);
            this.Closed += OnClosed;
        }

        private void OnClosed(object sender, WindowClosedEventArgs windowClosedEventArgs)
        {
            var dialog = sender as RadWindowBase;
            if (dialog != null)
            {
                dialog.Closed -= OnClosed;
                Messenger.Default.Unregister<CloseMessage>(this, CloseDialog);
            }
        }

        private void CloseDialog(CloseMessage e)
        {
            Close();
        }
    }
}
