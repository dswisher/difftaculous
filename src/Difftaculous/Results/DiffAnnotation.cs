
using Difftaculous.Paths;

namespace Difftaculous.Results
{
    public class DiffAnnotation
    {
        public DiffAnnotation(DiffPath path, string message)
        {
            Path = path;
            Message = message;
        }

        public DiffPath Path { get; private set; }
        public string Message { get; private set; }
    }
}
