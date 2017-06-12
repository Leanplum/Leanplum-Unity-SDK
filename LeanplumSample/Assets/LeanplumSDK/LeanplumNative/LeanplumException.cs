// Copyright 2013, Leanplum, Inc.

using System;

namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum exception.
    /// </summary>
    public class LeanplumException : Exception
    {
        public LeanplumException(string message) : base(message)
        {
        }
    }
}
