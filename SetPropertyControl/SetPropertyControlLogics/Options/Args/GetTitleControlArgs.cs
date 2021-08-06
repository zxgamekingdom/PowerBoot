﻿#nullable enable
using System;
using System.Reflection;

namespace SetPropertyControl.SetPropertyControlLogics.Options.Args
{
    public class GetTitleControlArgs : IGetSourceInstanceInfoArgs
    {
        public PropertyInfo? PropertyInfo { get; set; }
        public object? SourceInstance { get; set; }
        public Type? SourceInstanceType { get; set; }
    }
}