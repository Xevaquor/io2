using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IO2.Model
{
    public interface INoteRepository
    {
        void SerializeNotes(IEnumerable<Note> notes, string filename);
        IEnumerable<Note> Deserialize(string filename);
        List<Note> Search(string term);
        void Save();
        void Upsert(Note note);
        event Action Changed;
        void Delete(Note selectedNote);
    }

    public class JsonNoteRepository : INoteRepository
    {
        List<Note> notes = new List<Note>();

        public void SerializeNotes(IEnumerable<Note> notes, string filename)
        {
            using (var sw = new StreamWriter(filename, false, new UTF8Encoding(false)))
            {
                var json = JsonConvert.SerializeObject(notes, Formatting.Indented);
                sw.Write(json);
            }
        }

        public JsonNoteRepository()
        {
            try
            {
                notes = Deserialize("data.json").ToList();
            }
            catch 
            {

            }
        }
        public IEnumerable<Note> Deserialize(string filename)
        {
            using (var sw = new StreamReader(filename, new UTF8Encoding(false)))
            {
                string json = sw.ReadToEnd();
                notes = JsonConvert.DeserializeObject(json, typeof(List<Note>)) as List<Note>;
            }
            return notes ?? new List<Note>();
        }

        public List<Note> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return notes;

            var titles = notes.Where(n => n.Title.Contains(term)).OrderByDescending(n => n.Updated);
            var contents = notes.Where(n => n.Content.Contains(term)).OrderByDescending(n => n.Updated);
            return titles.Concat(contents).Distinct().OrderByDescending(x => x.Updated).ToList();
        }

        public void Save()
        {
            SerializeNotes(notes, "data.json");
        }

        public void Upsert(Note note)
        {
            note.Updated = DateTime.Now;
            // check if already present, then update. If not add
            if (!notes.Contains(note))
                notes.Add(note);

            Changed?.Invoke();
            Save();
        }

        public event Action Changed;
        public void Delete(Note note)
        {
            notes.Remove(note);
            Changed?.Invoke();
            Save();
        }
    }

    public class MockNoteSerializer : INoteRepository
    {
        List<Note> notes;

        public MockNoteSerializer()
        {
            notes = Deserialize("asd").ToList();
        }

        public IEnumerable<Note> Deserialize(string filename)
        {
            yield return new Note { Content = "asdf", Created = DateTime.Now, Title = "kekeke", Updated = DateTime.Now };
            yield return new Note { Content = "luj", Created = DateTime.Now, Title = "woah", Updated = DateTime.Now };
            yield return new Note { Content = "adżej duda", Created = DateTime.Now, Title = "kitten", Updated = DateTime.Now };
            yield return new Note { Content = "lubię placki", Created = DateTime.Now, Title = "omgwtf", Updated = DateTime.Now };
            yield return new Note { Content = "aleococho", Created = DateTime.Now, Title = "yo!", Updated = DateTime.Now };
        }

        public List<Note> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return GetAll();

            var titles = notes.Where(n => n.Title.Contains(term)).OrderByDescending(n => n.Updated);
            var contents = notes.Where(n => n.Content.Contains(term)).OrderByDescending(n => n.Updated);
            return titles.Concat(contents).Distinct().OrderByDescending(x => x.Updated).ToList();
        }

        public void Save()
        {
            //   throw new NotImplementedException();
        }

        public void Upsert(Note note)
        {
            note.Updated = DateTime.Now;
            // check if already present, then update. If not add
            if (notes.Contains(note))
            {
                // update
            }
            else
            {
                notes.Add(note);
            }
            Changed?.Invoke();
        }

        public event Action Changed;
        public void Delete(Note note)
        {
            notes.Remove(note);
            Changed?.Invoke();
        }

        private List<Note> GetAll()
        {
            return notes;
        }

        public void SerializeNotes(IEnumerable<Note> notes, string filename)
        {

        }
    }
}
