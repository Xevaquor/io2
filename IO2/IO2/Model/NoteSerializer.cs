using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO2.Model
{
    public interface INoteRepository
    {
        void SerializeNotes(IEnumerable<Note> notes, string filename);
        IEnumerable<Note> Deserialize(string filename);
        List<Note> Search(string term);
    }

    public class JsonNoteSerializer
    {
        public void SerializeNotes(IEnumerable<Note> notes, string filename)
        {
            // serialize
        }
        public IEnumerable<Note> Deserialize(string filename)
        {
            return null;
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
            return titles.Concat(contents).ToList();
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
