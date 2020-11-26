namespace LeanplumSDK
{
    /// <summary>
    ///     Represents time interval to periodically upload events to server.
    ///     Possible values are 5, 10, or 15 minutes.
    /// </summary>
    public enum EventsUploadInterval
    {
        /// <summary>
        ///     5 minutes interval
        /// </summary>
        AtMost5Minutes = 5,

        /// <summary>
        ///     10 minutes interval
        /// </summary>
        AtMost10Minutes = 10,

        /// <summary>
        ///     15 minutes interval
        /// </summary>
        AtMost15Minutes = 15
    }
}