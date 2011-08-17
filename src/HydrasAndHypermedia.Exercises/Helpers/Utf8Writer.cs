using System.IO;
using System.Text;

namespace HydrasAndHypermedia.Exercises.Helpers
{
    public class Utf8Writer : StringWriter
    {
        public Utf8Writer(StringBuilder sb) : base(sb)
        {
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}