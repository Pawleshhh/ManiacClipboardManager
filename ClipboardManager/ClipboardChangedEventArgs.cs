using System;

namespace ManiacClipboardManager
{
    /// <summary>
    /// Event arguments for the <see cref="IClipboardManager.ClipboardChanged"/> event.
    /// </summary>
    public class ClipboardChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets clipboard data.
        /// </summary>
        public ClipboardData Data { get; }

        /// <summary>
        /// Initializes the <see cref="ClipboardChangedEventArgs"/> instance.
        /// </summary>
        /// <param name="clipboardData">Clipboard data.</param>
        /// <exception cref="ArgumentNullException"/>
        public ClipboardChangedEventArgs(ClipboardData clipboardData)
        {
            if (clipboardData == null)
                throw new ArgumentNullException("clipboardData");

            Data = clipboardData;
        }
    }
}