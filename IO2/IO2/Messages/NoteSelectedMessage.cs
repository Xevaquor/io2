using IO2.Model;

namespace IO2.Messages
{
    public class NoteSelectedMessage
    {
        public NoteSelectedMessage(Note note)
        {
            Note = note;
        }

        public Note Note { get; set; }
    }
}