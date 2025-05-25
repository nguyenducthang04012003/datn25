
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PharmaDistiPro.Repositories.Infrastructures
{
    public class Disposable : IDisposable
    {
        private bool _disposedValue;
        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue && disposing)
            {
                // TODO: dispose managed state (managed objects)

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                DisposeCore();


            }
            _disposedValue = true;
        }
        protected virtual void DisposeCore()
        {
        }
    }
}
