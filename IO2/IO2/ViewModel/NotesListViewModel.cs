using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IO2.Model;

using System.Reactive.Linq;

namespace IO2.ViewModel
{
    public class NotesListViewModel : ReactiveObject
    {
        string searchTerm;
        public string SearchTerm
        {
            get { return searchTerm; }
            set
            {
                this.RaiseAndSetIfChanged(ref searchTerm, value);
            }
        }

        public ReactiveCommand<List<Note>> ExecuteSearch { get; private set; }

        ObservableAsPropertyHelper<List<Note>> searchResults;
        public List<Note> SearchResults => searchResults.Value;
        
        INoteRepository noteRepository;

        public NotesListViewModel()
        {
            noteRepository = new MockNoteSerializer();

            ExecuteSearch = ReactiveCommand.CreateAsyncTask(param => GetSearchResults(this.SearchTerm));

            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .InvokeCommand(ExecuteSearch);

            MessageBus.Current.RegisterMessageSource(this.WhenAnyValue(x => x.SearchTerm).
                Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(y => new Messages.UpdateStatusMessage("Filtruję...")));

            /*this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(s => { });*/
                

            searchResults = ExecuteSearch.ToProperty(this, x => x.SearchResults, new List<Note>());
      
        }

        private async Task<List<Note>> GetSearchResults(string searchTerm)
        {
            return await Task.Run(() => noteRepository.Search(searchTerm).ToList());
        }
    }
}
