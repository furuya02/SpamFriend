using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SpamFriend {
    class IECache {
        
        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME {
            public UInt32 dwLowDateTime;
            public UInt32 dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME {
            public UInt16 Year;
            public UInt16 Month;
            public UInt16 DayOfWeek;
            public UInt16 Day;
            public UInt16 Hour;
            public UInt16 Minute;
            public UInt16 Second;
            public UInt16 Milliseconds;
        }


        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern long FileTimeToSystemTime(ref FILETIME FileTime, ref SYSTEMTIME SystemTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern long SystemTimeToTzSpecificLocalTime(IntPtr lpTimeZoneInformation, ref SYSTEMTIME lpUniversalTime,
               out SYSTEMTIME lpLocalTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct INTERNET_CACHE_ENTRY_INFO {
            public UInt32 dwStructSize;
            public string lpszSourceUrlName;
            public string lpszLocalFileName;
            public UInt32 CacheEntryType;
            public UInt32 dwUseCount;
            public UInt32 dwHitRate;
            public UInt32 dwSizeLow;
            public UInt32 dwSizeHigh;
            public FILETIME LastModifiedTime;
            public FILETIME ExpireTime;
            public FILETIME LastAccessTime;
            public FILETIME LastSyncTime;
            public IntPtr lpHeaderInfo;
            public UInt32 dwHeaderInfoSize;
            public string lpszFileExtension;
            public UInt32 dwExemptDelta;
        };

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool GetUrlCacheEntryInfo(string lpszUrlName, IntPtr lpCacheEntryInfo, out UInt32 lpdwCacheEntryInfoBufferSize);


        public static INTERNET_CACHE_ENTRY_INFO GetUrlCacheEntryInfo(string url) {
            IntPtr buffer = IntPtr.Zero;
            UInt32 structSize;
            bool apiResult = GetUrlCacheEntryInfo(url,buffer, out structSize);
            //CheckLastError(url, true);

            try {
                buffer = Marshal.AllocHGlobal((int)structSize);
                apiResult = GetUrlCacheEntryInfo(url,buffer, out structSize);
                if (apiResult == true) {
                    return (INTERNET_CACHE_ENTRY_INFO)
                            Marshal.PtrToStructure(buffer,
                            typeof(INTERNET_CACHE_ENTRY_INFO));
                }

                //CheckLastError(url, false);
            } finally {
                //try{
                    //if (buffer.ToInt32() > 0){
                        try{
                            Marshal.FreeHGlobal(buffer);
                        } catch{
                        }
                    //}
                //} catch{
                    
                //}

            }

            //Debug.Assert(false, "We should either early-return" +" or throw before we get here");
            return new INTERNET_CACHE_ENTRY_INFO();
        }
    }
}
