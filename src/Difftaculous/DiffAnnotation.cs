

namespace Difftaculous
{
    public class DiffAnnotation
    {
        public DiffAnnotation(IDiffPath path, string message)
        {
            Path = path;
            Message = message;
        }

        public IDiffPath Path { get; private set; }
        public string Message { get; private set; }
    }
}
