using System;
using System.Diagnostics;
using System.Windows.Data;
using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using IO2.Messages;
using IO2.Model;
using IO2.Services;

namespace IO2.ViewModel
{
    public class NoteViewModel : ObservableObject
    {
        private bool performingUpdates = false;
        private PromptService prompt = new PromptService();

        private bool isDirty;

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                isDirty = value;
                RaisePropertyChanged();
            }
        }

        private bool @readonly;

        public bool Readonly
        {
            get { return @readonly; }
            set
            {
                @readonly = value; 
                RaisePropertyChanged();
            }
        }

        private Note selectedNote;

        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                /*
                if (value != null && selectedNote != value && IsDirty)
                    AskDirty();
                IsDirty = false;*/
                selectedNote = value;
                RaisePropertyChanged();
            }
        }

        private void AskDirty()
        {
            if (SelectedNote == null)
                return;

            var confirmation = prompt.OkCancel("Zapisać zmiany?");

            switch (confirmation)
            {
                case true:
                    SelectedNote.Updated = DateTime.Now;
                    SelectedNote.Content = Content;
                    SelectedNote.Title = Title;
                    repo.Upsert(SelectedNote);
                    repo.Save();
                    IsDirty = false;
                    break;
                case false:
                    SelectedNote = null;
                    IsDirty = false;
                    break;
            }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value; 
                RaisePropertyChanged();
            }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set
            {
                content = value; 
                RaisePropertyChanged();
            }
        }

        public RelayCommand AddCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand EditCommand { get; set; }

        private readonly INoteRepository repo;

        public NoteViewModel()
        {
            Messenger.Default.Register<Messages.NoteSelectedMessage>(this, OnNoteSelected);
            AddCommand = new RelayCommand(PerformAdd, () => true);
            DeleteCommand = new RelayCommand(PerformDelete, () => SelectedNote != null);
            SaveCommand = new RelayCommand(PerformSave, () => SelectedNote != null && IsDirty);
            EditCommand = new RelayCommand(PerformEdit, () => SelectedNote  != null && !IsDirty);

            repo = IoCKernel.Container.Resolve<INoteRepository>();
        }

        private void PerformEdit()
        {
            Readonly = false;
            IsDirty = true;
            CreatingNew = false;
            ModyfyingExisting = true;
        }

        public bool SaveConfirmationRequired { get; set; } = false;
        public bool CreatingNew { get; set; } = false;
        public bool ModyfyingExisting { get; set; } = false;

        private void PerformSave()
        {
            if (ModyfyingExisting)
            {
                var answer = prompt.YesNoCancel("Zapisać zmiany?");
                if (answer == false)
                {
                    // użytkownik anulował chęć zapisu zmian
                    CreatingNew = false;
                    ModyfyingExisting = false;
                    OnNoteSelected(new NoteSelectedMessage(SelectedNote));
                    return;
                }
                else if (answer == true)
                {
                    // tak, zapisac zmiany
                }
                else
                {
                    return; //nic się nie stało, Polacy nic się nie stało...
                }
            }
            
            SelectedNote.Updated = DateTime.Now;
            SelectedNote.Content = Content;
            SelectedNote.Title = Title;
            repo.Upsert(SelectedNote);
            repo.Save();
            IsDirty = false;
            CreatingNew = false;
            ModyfyingExisting = false;
            OnNoteSelected(new NoteSelectedMessage(SelectedNote));

            return;

            performingUpdates = true;
            if (SelectedNote == null)
                return;

            var confirmation = prompt.YesNoCancel("Zapisać zmiany?");

            switch (confirmation)
            {
                case true:
                    SelectedNote.Updated = DateTime.Now;
                    SelectedNote.Content = Content;
                    SelectedNote.Title = Title;
                    repo.Upsert(SelectedNote);
                    repo.Save();
                    IsDirty = false;
                    Readonly = true;
                    break;
                case false:
                    SelectedNote = null;
                    IsDirty = false;
                    Readonly = true;
                    break;
                default:
                    break;
            }
            performingUpdates = false;
        }

        private void PerformDelete()
        {
            if (SelectedNote == null)
            {
                return;
            }

            if (CreatingNew)
            {
                var result = prompt.OkCancel("Anulować tworzenie notatki?");
                if (result)
                {
                    // tak, na pewno chcemy anulować
                    SelectedNote = null;
                    Readonly = true;
                    CreatingNew = false;
                    IsDirty = false;
                    SaveConfirmationRequired = false;
                }
                else
                {
                    // pomyłka, pracujemy dalej
                }
                return;
            }

            if (prompt.OkCancel("Na pewno usunąć?"))
            {
                repo.Delete(SelectedNote);
                SelectedNote = null;
                Readonly = true;
                CreatingNew = false;
                IsDirty = false;
                SaveConfirmationRequired = false;
            }
        }

        private void PerformAdd()
        {
            if (CreatingNew)
            {
                var result = prompt.OkCancel("Anulować tworzenie notatki?");
                if (result)
                {
                    // tak, na pewno chcemy anulować
                }
                else
                {
                    // pomyłka, pracujemy dalej
                    return;
                }
            }
            SelectedNote = new Note();
            Title = "";
            Content = "";
            SaveConfirmationRequired = false;
            IsDirty = true;
            Readonly = false;
            CreatingNew = true;
            ModyfyingExisting = false;
        }

        private void OnNoteSelected(NoteSelectedMessage msg)
        {
            Debug.Assert(msg != null);
            // mamy niezapisane zmiany, co z nimi?
            if (CreatingNew)
            {
                var result = prompt.OkCancel("Anulować tworzenie notatki?");
                if (result)
                {
                    // tak, na pewno chcemy anulować
                }
                else
                {
                    // pomyłka, pracujemy dalej
                    return;
                }
            }
            if (ModyfyingExisting)
            {
                //
            }

            //if(performingUpdates) return;
         
            //performingUpdates = true;
            SelectedNote = msg.Note;
            IsDirty = false;
            Readonly = true;
            Title = msg.Note?.Title;
            Content = msg.Note?.Content;
            CreatingNew = false;
            ModyfyingExisting = false;
            //performingUpdates = false;
        }
    }
}