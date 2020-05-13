using System;

namespace ManiacClipboardManager
{
    /// <summary>
    /// Contains information about the source of stored data from the clipboard.
    /// </summary>
    public class ClipboardSource : IEquatable<ClipboardSource>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardSource"/> class.
        /// </summary>
        /// <param name="appName">Name of the source.</param>
        /// <exception cref="ArgumentException">Throws when appName is null or empty.</exception>
        public ClipboardSource(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentException("The appName parameter cannot be null or empty.", "appName");

            AppName = appName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardSource"/> class.
        /// </summary>
        /// <param name="appName">Name of the source.</param>
        /// <exception cref="ArgumentNullException">Throws when appName is null or empty.</exception>
        public ClipboardSource(string appName, string iconPath) : this(appName)
        {
            IconPath = iconPath;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets name of the source.
        /// </summary>
        public string AppName { get; }

        /// <summary>
        /// Gets optional path to the icon of the source application.
        /// </summary>
        public string IconPath { get; }

        #endregion Properties

        #region Methods

        public bool Equals(ClipboardSource other)
            => other != null && AppName == other.AppName;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is ClipboardSource source)
                return Equals(source);

            return false;
        }

        public override int GetHashCode() => AppName.GetHashCode() * 13;

        /// <summary>
        /// Returns the <see cref="AppName"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => AppName;

        #endregion Methods
    }
}