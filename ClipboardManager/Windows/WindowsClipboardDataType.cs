namespace ManiacClipboardManager.Windows
{
    /// <summary>
    /// Types of data that are recognizable.
    /// </summary>
    internal enum WindowsClipboardDataType
    {
        /// <summary>
        /// Not defined type of clipboard data.
        /// </summary>
        None = 0,

        /// <summary>
        /// Text type of clipboard data.
        /// </summary>
        Text = 1,

        /// <summary>
        /// File and folder type of clipboard data.
        /// </summary>
        FileList = 2,

        /// <summary>
        /// Audio type of clipboard data.
        /// </summary>
        Audio = 3,

        /// <summary>
        /// Image type of clipboard data.
        /// </summary>
        Image = 4,
    }
}