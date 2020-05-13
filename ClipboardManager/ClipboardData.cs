using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ManiacClipboardManager
{
    /// <summary>
    /// Represents data that were stored on the clipboard.
    /// </summary>
    public class ClipboardData
    {
        #region Constructors

        /// <summary>
        /// Initializes an instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data from the clipboard.</param>
        /// <param name="type">Type of the data.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public ClipboardData(object data, ClipboardDataType type)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "data param cannot be null.");
            if (!Enum.IsDefined(typeof(ClipboardDataType), type))
                throw new ArgumentException("type param value is not recognized.", nameof(type));

            Data = data;
            DataType = type;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data from the clipboard.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="source">Source of the data.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public ClipboardData(object data, ClipboardDataType type, ClipboardSource source) : this(data, type)
        {
            Source = source;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data from the clipboard.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="formats">Formats of the data.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public ClipboardData(object data, ClipboardDataType type, string[] formats) : this(data, type)
        {
            _formats = formats;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ClipboardData"/> class.
        /// </summary>
        /// <param name="data">Data from the clipboard.</param>
        /// <param name="type">Type of the data.</param>
        /// <param name="formats">Formats of the data.</param>
        /// <param name="source">Source of the data.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public ClipboardData(object data, ClipboardDataType type, string[] formats, ClipboardSource source):
            this(data, type, formats)
        {
            Source = source;
        }

        #endregion Constructors

        #region Private fields

        private readonly string[] _formats;

        #endregion

        #region Properties

        /// <summary>
        /// Gets data that were stored on the clipboard.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Gets the type of <see cref="Data"/>.
        /// </summary>
        public ClipboardDataType DataType { get; }

        /// <summary>
        /// Gets the source of <see cref="Data"/>. Can be null.
        /// </summary>
        public ClipboardSource Source { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets formats of the stored data.
        /// </summary>
        /// <returns>Returns an array of formats.</returns>
        public string[] GetFormats()
        {
            if (_formats == null)
                return new string[] { };

            string[] formats = new string[_formats.Length];
            _formats.CopyTo(formats, 0);

            return formats;
        }

        /// <summary>
        /// Returns returned value from the <see cref="Data"/>.ToString()
        /// </summary>
        public override string ToString()
        {
            return Data.ToString();
        }

        #endregion Methods

        #region Static Methods

        private static void ThrowIfDataIsNull(ClipboardData data)
        {
            if (data == null)
                throw new ArgumentNullException("data", "data parameter cannot be null.");
        }

        /// <summary>
        /// Gets text from given <see cref="ClipboardData"/>. If it does not store a text it returns null.
        /// </summary>
        /// <param name="data">Data to get text from.</param>
        /// <returns>Text from data or null if it does not store text.</returns>
        /// <exception cref="ArgumentNullException">Throws when data parameter is null.</exception>
        public static string GetText(ClipboardData data)
        {
            ThrowIfDataIsNull(data);

            if (data.DataType == ClipboardDataType.Text)
                return data.Data as string;

            return null;
        }

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Text"/> type.
        /// </summary>
        /// <param name="text">Text data.</param>
        public static ClipboardData CreateTextData(string text)
            => new ClipboardData(text, ClipboardDataType.Text);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Text"/> type.
        /// </summary>
        /// <param name="text">Text data.</param>
        /// <param name="source">Source of the data.</param>
        public static ClipboardData CreateTextData(string text, ClipboardSource source)
            => new ClipboardData(text, ClipboardDataType.Text, source);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Text"/> type.
        /// </summary>
        /// <param name="text">Text data.</param>
        /// <param name="formats">Formats of the data.</param>
        public static ClipboardData CreateTextData(string text, string[] formats)
            => new ClipboardData(text, ClipboardDataType.Text, formats);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Text"/> type.
        /// </summary>
        /// <param name="text">Text data.</param>
        /// <param name="formats">Formats of the data.</param>
        /// <param name="source">Source of the data.</param>
        public static ClipboardData CreateTextData(string text, string[] formats, ClipboardSource source)
            => new ClipboardData(text, ClipboardDataType.Text, formats, source);

        ///// <summary>
        ///// Gets a path from given <see cref="ClipboardData"/>. If it does not store a path it returns null.
        ///// </summary>
        ///// <param name="data">Data to get path from.</param>
        ///// <returns>Path from data or null if it does not store a path.</returns>
        ///// <exception cref="ArgumentNullException">Throws when data parameter is null.</exception>
        //public static string GetPath(ClipboardData data)
        //{
        //    ThrowIfDataIsNull(data);

        //    if (data.DataType == ClipboardDataType.Path)
        //        return data.Data as string;

        //    return null;
        //}

        ///// <summary>
        ///// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Path"/> type.
        ///// </summary>
        ///// <param name="path">Text with path.</param>
        //public static ClipboardData CreatePathData(string path)
        //    => new ClipboardData(path, ClipboardDataType.Path);

        ///// <summary>
        ///// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Path"/> type.
        ///// </summary>
        ///// <param name="path">Text with path.</param>
        ///// <param name="source">Source of the data.</param>
        //public static ClipboardData CreatePathData(string path, ClipboardSource source)
        //    => new ClipboardData(path, ClipboardDataType.Path, source);

        /// <summary>
        /// Gets a collection of paths from given <see cref="ClipboardData"/>. If it does not store a collection of paths it returns null.
        /// </summary>
        /// <param name="data">Data to get a collection of paths from.</param>
        /// <returns>A collection of paths from data or null if it does not store a collection of paths.</returns>
        /// <exception cref="ArgumentNullException">Throws when data parameter is null.</exception>
        public static string[] GetPathList(ClipboardData data)
        {
            ThrowIfDataIsNull(data);

            if (data.DataType == ClipboardDataType.PathList)
            {
                if (data.Data is string[] arr)
                    return arr;
                else if (data.Data is ICollection<string> collection)
                    return collection.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.PathList"/> type.
        /// </summary>
        /// <param name="paths">Array of paths.</param>
        public static ClipboardData CreatePathListData(string[] paths)
            => new ClipboardData(paths, ClipboardDataType.PathList);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.PathList"/> type.
        /// </summary>
        /// <param name="paths">Array of paths.</param>
        /// <param name="source">Source of the data.</param>
        public static ClipboardData CreatePathListData(string[] paths, ClipboardSource source)
            => new ClipboardData(paths, ClipboardDataType.PathList, source);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.PathList"/> type.
        /// </summary>
        /// <param name="paths">Array of paths.</param>
        /// <param name="formats">Formats of the data.</param>
        public static ClipboardData CreatePathListData(string[] paths, string[] formats)
            => new ClipboardData(paths, ClipboardDataType.PathList, formats);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.PathList"/> type.
        /// </summary>
        /// <param name="paths">Array of paths.</param>
        /// <param name="formats">Formats of the data.</param>
        /// <param name="source">Source of the data.</param>
        public static ClipboardData CreatePathListData(string[] paths, string[] formats, ClipboardSource source)
            => new ClipboardData(paths, ClipboardDataType.PathList, formats, source);

        /// <summary>
        /// Gets an image from given <see cref="ClipboardData"/>. If it does not store an image it returns null.
        /// </summary>
        /// <param name="data">Data to get an image from.</param>
        /// <returns>An image from data or null if it does not store an image.</returns>
        /// <exception cref="ArgumentNullException">Throws when data parameter is null.</exception>
        public static Bitmap GetImage(ClipboardData data)
        {
            ThrowIfDataIsNull(data);

            if (data.DataType == ClipboardDataType.Image)
            {
                return data.Data as Bitmap;
            }

            return null;
        }

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Image"/> type.
        /// </summary>
        /// <param name="image">Image data.</param>
        public static ClipboardData CreateImageData(Bitmap image)
            => new ClipboardData(image, ClipboardDataType.Image);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Image"/> type.
        /// </summary>
        /// <param name="image">Image data.</param>
        /// <param name="source">Source of the data.</param>
        public static ClipboardData CreateImageData(Bitmap image, ClipboardSource source)
            => new ClipboardData(image, ClipboardDataType.Image, source);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Image"/> type.
        /// </summary>
        /// <param name="image">Image data.</param>
        /// <param name="formats">Formats of the data.</param>
        public static ClipboardData CreateImageData(Bitmap image, string[] formats)
            => new ClipboardData(image, ClipboardDataType.Image, formats);

        /// <summary>
        /// Creates an instance of <see cref="ClipboardData"/> with data of <see cref="ClipboardDataType.Image"/> type.
        /// </summary>
        /// <param name="image">Image data.</param>
        /// <param name="formats">Formats of the data.</param>
        /// <param name="source">Source of the data.</param>
        public static ClipboardData CreateImageData(Bitmap image, string[] formats, ClipboardSource source)
            => new ClipboardData(image, ClipboardDataType.Image, formats, source);

        #endregion Static Methods
    }

    /// <summary>
    /// Types of clipboard data that are recognized by the ClipboardManager.
    /// </summary>
    public enum ClipboardDataType
    {
        /// <summary>
        /// Unknown type of data.
        /// </summary>
        Unknown,

        /// <summary>
        /// Text type of data.
        /// </summary>
        Text,

        /// <summary>
        /// Collection of paths to files/folders.
        /// </summary>
        PathList,

        /// <summary>
        /// Image type of data.
        /// </summary>
        Image
    }
}