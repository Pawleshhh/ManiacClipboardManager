using System;
using System.Threading.Tasks;

namespace ManiacClipboardManager
{
    /// <summary>
    /// Defines the interface of clipboard manager.
    /// </summary>
    public interface IClipboardManager : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets whether this manager is monitoring the clipboard or not.
        /// </summary>
        bool IsMonitoring { get; }

        ///// <summary>
        ///// Gets or sets whether some types of clipboard data can be converted to another type (for example TextClipboardData with
        ///// a file path will be converted to PathClipboardData).
        ///// </summary>
        //bool AutoConvert { get; set; }

        #endregion Properties

        #region Events

        /// <summary>
        /// Rises when the clipboard has changed.
        /// </summary>
        event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;

        #endregion Events

        #region Methods

        /// <summary>
        /// Makes this manager to start monitoring the clipboard.
        /// </summary>
        void StartMonitoring();

        /// <summary>
        /// Makes this manager to stop monitoring the clipboard.
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// Gets data that is currently stored in the clipboard.
        /// </summary>
        /// <returns>Returns data from the clipboard as <see cref="IClipboardData"/>.</returns>
        ClipboardData GetClipboardData();

        /// <summary>
        /// Sets given data on the clipboard.
        /// </summary>
        /// <param name="data">Data to be stored on the clipboard.</param>
        void SetClipboardData(ClipboardData data);

        /// <summary>
        /// Gets type of data currently stored in the clipboard.
        /// </summary>
        /// <returns>Returns type of clipboard data as <see cref="ClipboardDataType"/>.</returns>
        ClipboardDataType GetClipboardDataType();

        ///// <summary>
        ///// Tries to get data that is currently stored on the clipboard.
        ///// </summary>
        ///// <param name="clipboardData">Ref parameter where the data is going to be returned.</param>
        ///// <returns>Returns true if successfully gets clipboard data; otherwise false.</returns>
        //bool TryGetClipboardData(ref IClipboardData clipboardData);

        ///// <summary>
        ///// Tries to set data on the clipboard.
        ///// </summary>
        ///// <param name="data">Data to be stored on the clipboard.</param>
        ///// <returns>Returns true if successfully sets data on the clipboard; otherwise false.</returns>
        //bool TrySetClipboardData(IClipboardData data);

        ///// <summary>
        ///// Tries to get type of data that is currently stored on the clipboard.
        ///// </summary>
        ///// <param name="dataType">Ref parameter where the type is going to be returned.</param>
        ///// <returns>Returns true if successfully gets type of data; otherwise false.</returns>
        //bool TryGetClipboardDataType(ref ClipboardDataType dataType);

        /// <summary>
        /// Clears clipboard from any data it stores.
        /// </summary>
        void ClearClipboard();

        /// <summary>
        /// Checks whether the clipboard is empty or not.
        /// </summary>
        /// <returns>Returns true if the clipboard is empty; otherwise false.</returns>
        bool IsClipboardEmpty();

        /// <summary>
        /// Gets data that is currently stored in the clipboard asynchronously.
        /// </summary>
        /// <returns>Returns task whose result is <see cref="IClipboardData"/>.</returns>
        Task<ClipboardData> GetClipboardDataAsync();

        /// <summary>
        /// Sets given data on the clipboard asynchronously.
        /// </summary>
        /// <param name="data">Data to be stored on the clipboard.</param>
        /// <returns>Returns task that sets data on the clipboard.</returns>
        Task SetClipboardDataAsync(ClipboardData data);

        /// <summary>
        /// Gets type of data currently stored in the clipboard asynchronously.
        /// </summary>
        /// <returns>Returns task whose result is <see cref="ClipboardDataType"/>.</returns>
        Task<ClipboardDataType> GetClipboardDataTypeAsync();

        /// <summary>
        /// Clears clipboard from any data it stores asynchronously.
        /// </summary>
        /// <returns>Returns task that clears clipboard.</returns>
        Task ClearClipboardAsync();

        /// <summary>
        /// Checks whether the clipboard is empty or not asynchronously.
        /// </summary>
        /// <returns>Returns task with result that is true if the clipboard is empty; otherwise false.</returns>
        Task<bool> IsClipboardEmptyAsync();

        #endregion Methods
    }
}