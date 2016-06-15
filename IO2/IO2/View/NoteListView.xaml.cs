using IO2.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using IO2.Model;

namespace IO2.View
{
    /// <summary>
    /// Interaction logic for NoteListView.xaml
    /// </summary>
    public partial class NoteListView : UserControl
    {
        public NoteListView()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Write("SELECTIO CHANGED");
        }

        private void ListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var lv = sender as ListView;
            if(lv?.SelectedItem != null)
                Messenger.Default.Send(new Messages.NoteSelectedMessage((Note) lv.SelectedItem));
        }
    }
}
