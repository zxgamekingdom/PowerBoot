#nullable enable
using SetPropertyControl.SetPropertyControlLogics.Options;

using System;

namespace SetPropertyControl.SetPropertyControlLogics.DataInfos
{
    internal class SourceInstanceContext
    {
        public object Instance { get; set; } = null!;
        public Type Type { get; set; } = null!;
        public SetPropertyUserControlOptions? ViewModelControlOptions { get; set; }
    }
}