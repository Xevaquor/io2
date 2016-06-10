using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IO2.Model;

namespace IO2.ViewModel
{
    public class NotesListViewModel : ObservableObject
    {
        private bool readMode = true;

        public bool ReadMode
        {
            get { return readMode; }
            set
            {
                readMode = value;
                RaisePropertyChanged();
            }
        }

        string searchTerm;
        public string SearchTerm
        {
            get { return searchTerm; }
            set
            {
                if (searchTerm == value)
                    return;
                SelectedNote = null;
                searchTerm = value;
                SearchResults = GetSearchResults(value);
                RaisePropertyChanged();
            }
        }

        private Note selectedNote;

        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                Messenger.Default.Send(new Messages.NoteSelectedMessage(SelectedNote));
                RaisePropertyChanged();
            }
        }

        public RelayCommand<List<Note>> ExecuteSearch { get; private set; }
        public RelayCommand CreateNote { get; private set; }

        private ObservableCollection<Note> searchResults;

        public ObservableCollection<Note> SearchResults
        {
            get { return searchResults; }
            private set
            {
                searchResults = value;
                if (!searchResults.Any())
                {
                    Messenger.Default.Send(new Messages.UpdateStatusMessage("Brak wyników filtrowania"));
                }
                else
                {
                    Messenger.Default.Send(new Messages.UpdateStatusMessage("Gotowy"));
                }
                RaisePropertyChanged();
            }
        }

        readonly INoteRepository noteRepository;

        public NotesListViewModel()
        {
            noteRepository = IoCKernel.Container.Resolve<INoteRepository>();
            noteRepository.Changed += () => SearchResults = GetSearchResults(SearchTerm);
            SearchResults = GetSearchResults("");

            CreateNote = new RelayCommand(
                PerformCreateNote
                );
        }

        private void PerformCreateNote()
        {
            ReadMode = false;
            var note = new Note();
            Messenger.Default.Send(new Messages.NoteSelectedMessage(note));
        }

        private ObservableCollection<Note> GetSearchResults(string term)
        {
            var obs = new ObservableCollection<Note>(noteRepository.Search(term).OrderByDescending(x => x.Updated));
            return obs;
        }
    }
}
