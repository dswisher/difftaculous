﻿
using System.Collections.Generic;


namespace Difftaculous
{
    public interface IDiffResult
    {
        // TODO - should this just be a count of differences?
        bool AreSame { get; }

        IEnumerable<DiffAnnotation> Annotations { get; }

        IDiffResult Merge(IDiffResult other);
    }
}
