using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO2.Model
{
    class NotesSource
    {
        List<Note> notes = new List<Note>();

        public IEnumerable<Note> Notes => notes;

        public NotesSource()
        {
            var ds = new MockNoteSerializer();
            foreach (var item in ds.Deserialize("asdf"))
            {
                notes.Add(item);
            }
        }
    }
}
