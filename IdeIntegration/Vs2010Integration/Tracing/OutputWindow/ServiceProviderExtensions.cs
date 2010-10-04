using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace TechTalk.SpecFlow.Vs2010Integration.Tracing.OutputWindow
{
    public static class ServiceProviderExtensions
    {
        public static TServiceInterface TryGetGlobalService<TServiceClass, TServiceInterface>(this IServiceProvider sp)
            where TServiceInterface : class
        {
            if (sp == null) throw new ArgumentNullException("sp");

            Guid guidService = typeof(TServiceInterface).GUID;
            Guid riid = typeof(TServiceInterface).GUID;
            IntPtr obj = IntPtr.Zero;
            int result = ErrorHandler.CallWithCOMConvention(() => sp.QueryService(ref guidService, ref riid, out obj));
            if (ErrorHandler.Failed(result) || obj == IntPtr.Zero)
                return null;

            try
            {
                TServiceInterface service = (TServiceInterface)Marshal.GetObjectForIUnknown(obj);
                return service;
            }
            finally
            {
                Marshal.Release(obj);
            }
        }
    }
}