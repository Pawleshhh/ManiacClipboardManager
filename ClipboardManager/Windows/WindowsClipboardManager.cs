using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ManiacClipboardManager.Windows
{
    /// <summary>
    /// Provides functions to work with the Windows' clipboard.
    /// </summary>
    public sealed class WindowsClipboardManager : IClipboardManager
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of the <see cref="WindowsClipboardManager"/> class.
        /// </summary>
        /// <param name="window">Window that will be getting notified about clipboard's updates.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public WindowsClipboardManager(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            RegisterToClipboardChain(window);
        }

        #endregion Constructors

        #region Private fields

        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private const int MAX_CLIPBOARD_ACCESS = 20;

        private const int WAIT_TIME = 50;

        private HwndSource _hWndSource;

        #endregion Private fields

        #region Properties

        /// <summary>
        /// <see cref="IClipboardManager.IsMonitoring"/>.
        /// </summary>
        public bool IsMonitoring { get; private set; }

        #endregion Properties

        #region Events

        /// <summary>
        /// <see cref="IClipboardManager.ClipboardChanged"/>.
        /// </summary>
        public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;

        private void OnClipboardChanged(ClipboardData data)
        {
            if (data == null)
                return;

            ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(data));
        }

        #endregion Events

        #region Imported functions

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #endregion Imported functions

        #region Public methods

        /// <summary>
        /// <see cref="IClipboardManager.GetClipboardData"/>.
        /// </summary>
        public ClipboardData GetClipboardData()
        {
            ThrowIfDisposed();

            ClipboardDataType type = ClipboardDataType.Unknown;

            (object data, string[] formats) = WorkOnClipboardSafe(() =>
            {
                type = GetClipboardDataTypeNotSafe();

                return GetDataByType(type);
            }, (null, null));

            return ClipboardDataFactory(data, type, formats:formats);
        }

        /// <summary>
        /// <see cref="IClipboardManager.GetClipboardDataAsync"/>.
        /// </summary>
        public Task<ClipboardData> GetClipboardDataAsync()
        {
            return Task.Run(() => GetClipboardData());
        }

        /// <summary>
        /// <see cref="IClipboardManager.SetClipboardData"/>.
        /// </summary>
        public void SetClipboardData([AllowNull] ClipboardData data)
        {
            ThrowIfDisposed();
            if (data == null)
            {
                ClearClipboard();
                return;
            }

            switch (data.DataType)
            {
                case ClipboardDataType.Text:
                    WorkOnClipboardSafe(SetText, data);
                    break;
                case ClipboardDataType.PathList:
                    WorkOnClipboardSafe(SetPathList, data);
                    break;
                case ClipboardDataType.Image:
                    WorkOnClipboardSafe(SetImage, data);
                    break;
                default:
                    WorkOnClipboardSafe(SetUnknownData, data);
                    break;
            }
        }

        /// <summary>
        /// <see cref="IClipboardManager.SetClipboardDataAsync"/>.
        /// </summary>
        public Task SetClipboardDataAsync([AllowNull] ClipboardData data)
        {
            return Task.Run(() => SetClipboardData(data));
        }

        /// <summary>
        /// <see cref="IClipboardManager.GetClipboardDataType"/>.
        /// </summary>
        public ClipboardDataType GetClipboardDataType()
        {
            ThrowIfDisposed();

            return WorkOnClipboardSafe(() =>
            {
                return GetClipboardDataTypeNotSafe();
            }, ClipboardDataType.Unknown);
        }

        /// <summary>
        /// <see cref="IClipboardManager.GetClipboardDataTypeAsync"/>.
        /// </summary>
        public Task<ClipboardDataType> GetClipboardDataTypeAsync()
        {
            return Task.Run(() => GetClipboardDataType());
        }

        /// <summary>
        /// <see cref="IClipboardManager.ClearClipboard"/>.
        /// </summary>
        public void ClearClipboard()
        {
            ThrowIfDisposed();
            WorkOnClipboardSafe(() => Clipboard.Clear());
        }

        /// <summary>
        /// <see cref="IClipboardManager.ClearClipboardAsync"/>.
        /// </summary>
        public Task ClearClipboardAsync()
        {
            return Task.Run(() => ClearClipboard());
        }

        /// <summary>
        /// <see cref="IClipboardManager.IsClipboardEmpty"/>.
        /// </summary>
        public bool IsClipboardEmpty()
        {
            ThrowIfDisposed();
            return WorkOnClipboardSafe(() =>
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                string[] dataFormats = dataObject.GetFormats();

                if (dataFormats == null || dataFormats.Length == 0)
                    return true;

                object data = dataObject.GetData(dataFormats[0]);

                return data == null;
            }, true);
        }

        /// <summary>
        /// <see cref="IClipboardManager.IsClipboardEmptyAsync"/>.
        /// </summary>
        public Task<bool> IsClipboardEmptyAsync()
        {
            return Task.Run(() => IsClipboardEmpty());
        }

        /// <summary>
        /// <see cref="IClipboardManager.StartMonitoring"/>.
        /// </summary>
        public void StartMonitoring()
        {
            IsMonitoring = true;
        }

        /// <summary>
        /// <see cref="IClipboardManager.StopMonitoring"/>.
        /// </summary>
        public void StopMonitoring()
        {
            IsMonitoring = false;
        }

        #endregion Public methods

        #region Private methods

        private void RegisterToClipboardChain(Window window)
        {
            WindowInteropHelper wih = new WindowInteropHelper(window);

            _hWndSource = HwndSource.FromHwnd(wih.EnsureHandle());
            _hWndSource.AddHook(WndProc);

            bool result = AddClipboardFormatListener(_hWndSource.Handle);

            if (!result)
                throw new InvalidOperationException("Windows clipboard manager could not register to the clipboard chain.");

            StartMonitoring();
        }

        private void UnregisterFromClipboardChain()
        {
            RemoveClipboardFormatListener(_hWndSource.Handle);
        }

        private int _clipboardUpdateCounter = 0;

        private int ClipboardUpdateCounter
        {
            get
            {
                async void setClipboardUpdateCounter()
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(200);
                        ClipboardUpdateCounter = 0;
                    }
                    );
                }
                setClipboardUpdateCounter();
                return _clipboardUpdateCounter;
            }
            set
            {
                _clipboardUpdateCounter = value;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CLIPBOARDUPDATE:

                    if (IsMonitoring && ClipboardUpdateCounter < 1)
                    {
                        OnClipboardChanged(GetClipboardDataWithSource());
                        ClipboardUpdateCounter++;
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets currently stored data in the clipboard assuming the foreground window is the source.
        /// </summary>
        private ClipboardData GetClipboardDataWithSource()
        {
            ClipboardSource source = null;
            Thread thread = new Thread(() => source = GetClipboardSource());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            ClipboardDataType type = ClipboardDataType.Unknown;
            (object data, string[] formats) = WorkOnClipboardSafe(() =>
            {
                type = GetClipboardDataTypeNotSafe();

                return GetDataByType(type);
            }, (null, null));

            thread.Join();

            return ClipboardDataFactory(data, type, source, formats);
        }

        private ClipboardDataType GetClipboardDataTypeNotSafe()
        {
            if (Clipboard.ContainsText()) return ClipboardDataType.Text;
            if (Clipboard.ContainsFileDropList()) return ClipboardDataType.PathList;
            if (Clipboard.ContainsImage()) return ClipboardDataType.Image;
            //if (Clipboard.ContainsAudio()) return WindowsClipboardDataType.Audio;

            return ClipboardDataType.Unknown;
        }

        private ClipboardData ClipboardDataFactory(object data, ClipboardDataType type, ClipboardSource source = null, params string[] formats)
        {
            if (data == null)
                return null;

            switch (type)
            {
                case ClipboardDataType.Text:

                    string text = (string)data;

                    return ClipboardData.CreateTextData(text, formats, source);

                case ClipboardDataType.PathList:

                    StringCollection collection = data as StringCollection;
                    if (collection == null || collection.Count == 0)
                        return ClipboardData.CreatePathListData(new string[] { }, formats, source);
                    else
                        return ClipboardData.CreatePathListData(collection.Cast<string>().ToArray(), formats, source);

                case ClipboardDataType.Image:

                    Bitmap bitmap = (Bitmap)data;
                    return ClipboardData.CreateImageData(bitmap, formats, source);

                default:

                    return new ClipboardData(data, ClipboardDataType.Unknown, formats, source);
            }
        }

        private (object data, string[] formats) GetDataByType(ClipboardDataType type)
        {
            switch (type)
            {
                case ClipboardDataType.Text:

                    return (Clipboard.GetText(), new string[] { DataFormats.UnicodeText });

                case ClipboardDataType.PathList:

                    return (Clipboard.GetFileDropList(), new string[] { DataFormats.FileDrop });

                case ClipboardDataType.Image:

                    BitmapSource img = Clipboard.GetImage();
                    img.Freeze();
                    Bitmap bitmap = WindowsImageHelper.BitmapSource2Bitmap(img);

                    return (bitmap, new string[] { DataFormats.Bitmap });

                //case WindowsClipboardDataType.Audio:
                //    return (Clipboard.GetAudioStream(), null);

                default: //Unknown

                    IDataObject dataObject = Clipboard.GetDataObject();
                    string[] dataFormats = dataObject.GetFormats();

                    if (dataFormats == null || dataFormats.Length == 0)
                        return (null, null);

                    return (dataObject.GetData(dataFormats[0]), dataFormats);

            }
        }

        private ClipboardSource GetClipboardSource()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();

                if (hwnd == IntPtr.Zero)
                    return null;

                uint pID;
                GetWindowThreadProcessId(hwnd, out pID);
                Process process = Process.GetProcessById((int)pID);

                string appName = process.ProcessName;
                string path = process.MainModule.FileName;

                return new ClipboardSource(appName, path);
            }
            catch
            {
                return null;
            }
        }

        //private BitmapImage GetBitmap(MemoryStream stream)
        //{
        //    var bitmap = new BitmapImage();
        //    bitmap.BeginInit();
        //    bitmap.StreamSource = stream;
        //    bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmap.EndInit();
        //    bitmap.Freeze();

        //    return bitmap;
        //}

        private StringCollection GetFileDropList(params string[] paths)
        {
            StringCollection collection = new StringCollection();
            if (paths == null || paths.Length == 0)
                return collection;

            collection.AddRange(paths);
            return collection;
        }

        private DataObject GetUnknownData(object data, string[] formats)
        {
            if (formats == null || formats.Length == 0)
                return new DataObject(data.GetType(), data);

            return new DataObject(formats[0], data);
        }

        private TResult WorkOnClipboardSafe<TResult>(Func<TResult> func, TResult defaultValue)
        {
            TResult result = defaultValue;

            Thread thread = new Thread(() =>
            {
                int counter = 0;

                while (counter < MAX_CLIPBOARD_ACCESS)
                {
                    try
                    {
                        result = func();
                        break;
                    }
                    catch (ExternalException)
                    {
                        counter++;
                        Thread.Sleep(WAIT_TIME);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("When working with clipboard not external exception occurred.", ex);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return result;
        }

        private void WorkOnClipboardSafe(Action action)
        {
            Thread thread = new Thread(() =>
            {
                int counter = 0;

                while (counter < MAX_CLIPBOARD_ACCESS)
                {
                    try
                    {
                        action();
                        break;
                    }
                    catch (ExternalException)
                    {
                        counter++;
                        Thread.Sleep(WAIT_TIME);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("When working with clipboard not external exception occurred.", ex);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private void WorkOnClipboardSafe<TParam>(Action<TParam> action, TParam param)
        {
            Thread thread = new Thread(() =>
            {
                int counter = 0;

                while (counter < MAX_CLIPBOARD_ACCESS)
                {
                    try
                    {
                        action(param);
                        break;
                    }
                    catch (ExternalException)
                    {
                        counter++;
                        Thread.Sleep(WAIT_TIME);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("When working with clipboard not external exception occurred.", ex);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private void SetText(ClipboardData data)
        {
            string text = ClipboardData.GetText(data);

            Clipboard.SetText(text);
            Clipboard.Flush();
        }

        private void SetPathList(ClipboardData data)
        {
            string[] paths = ClipboardData.GetPathList(data);

            Clipboard.SetFileDropList(GetFileDropList(paths));
            Clipboard.Flush();
        }

        private void SetImage(ClipboardData data)
        {
            Bitmap bitmap = ClipboardData.GetImage(data);
            BitmapSource bitmapSource = WindowsImageHelper.Bitmap2BitmapSource(bitmap);

            Clipboard.SetImage(bitmapSource);
            Clipboard.Flush();
        }

        private void SetUnknownData(ClipboardData data)
        {
            DataObject dataObject = GetUnknownData(data.Data, data.GetFormats());
            Clipboard.SetDataObject(dataObject, true);
            Clipboard.Flush();
        }

        #endregion Private methods

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        ~WindowsClipboardManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                StopMonitoring();
                UnregisterFromClipboardChain();
                _hWndSource.Dispose();

                disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (disposedValue)
                throw new ObjectDisposedException("WindowsClipboardManager");
        }

        #endregion IDisposable Support
    }
}