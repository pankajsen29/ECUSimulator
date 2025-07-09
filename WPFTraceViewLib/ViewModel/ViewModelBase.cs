using WPFTraceViewLib.Model;

namespace WPFTraceViewLib.ViewModel
{
    /// <summary>
    /// base class for ViewModels, inheriting from NotifyPropertyChangedBase and implementing IDisposable.
    /// </summary>
    public class ViewModelBase : NotifyPropertyChangedBase, IDisposable
    {
        public ViewModelBase()
        {
        }

        #region Disposable Base Implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose managed and unmanaged object
        /// </summary>
        /// <param name="disposing">disposing</param>        
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                // Free other state (managed objects).
                DisposeManagedObjects();
            }

            // Free your own state (unmanaged objects).
            // Set large fields to null.
            DisposeUnmanagedObjects();

            IsDisposed = true;
        }

        /// <summary>
        /// Dispose manage object
        /// </summary>
        protected virtual void DisposeManagedObjects()
        {
            // override if required
        }

        /// <summary>
        /// dispose unmanaged object
        /// </summary>
        protected virtual void DisposeUnmanagedObjects()
        {
            // override if required
        }

        /// <summary>
        /// Proterty for remember of disposing
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Throw a Exception on Disposing
        /// </summary>
        protected void ThrowOnDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     Allows an <see cref="T:System.Object" /> to attempt to free resources and perform other cleanup operations before the
        ///     <see
        ///         cref="T:System.Object" />
        ///     is reclaimed by garbage collection.
        /// </summary>
        ~ViewModelBase()
        {
            // Use C# destructor syntax for finalization code.
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion
    }
}
