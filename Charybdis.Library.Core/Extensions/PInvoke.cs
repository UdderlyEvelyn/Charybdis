using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Charybdis.Library.Core
{
    public static class PInvoke
    {
        public static R LoadAndCall<R>(string dllName, string procName, params object[] args)
        {
            return (R)Marshal.GetDelegateForFunctionPointer(PInvoke.KERNEL32.GetProcAddress(PInvoke.KERNEL32.LoadLibrary(dllName), procName), typeof(Func<object[], R>)).DynamicInvoke(args);
        }

        public enum SystemErrorCode : uint
        {
            /// <summary>
            /// The operation completed successfully.
            /// </summary>
            ERROR_SUCCESS = 0,
            /// <summary>
            /// Incorrect function.
            /// </summary>
            ERROR_INVALID_FUNCTION = 1,
            /// <summary>
            /// The system cannot find the file specified.
            /// </summary>
            ERROR_FILE_NOT_FOUND = 2,
            /// <summary>
            /// The system cannot find the path specified.
            /// </summary>
            ERROR_PATH_NOT_FOUND = 3,
            /// <summary>
            /// The system cannot open the file.
            /// </summary>
            ERROR_TOO_MANY_OPEN_FILES = 4,
            /// <summary>
            /// Access is denied.
            /// </summary>
            ERROR_ACCESS_DENIED = 5,
            /// <summary>
            /// The handle is invalid.
            /// </summary>
            ERROR_INVALID_HANDLE = 6,
            /// <summary>
            /// The storage control blocks were destroyed.
            /// </summary>
            ERROR_ARENA_TRASHED = 7,
            /// <summary>
            /// Not enough storage is available to process this command.
            /// </summary>
            ERROR_NOT_ENOUGH_MEMORY = 8,
            /// <summary>
            /// The storage control block address is invalid.
            /// </summary>
            ERROR_INVALID_BLOCK = 9,
            /// <summary>
            /// The environment is incorrect.
            /// </summary>
            ERROR_BAD_ENVIRONMENT = 10,
            /// <summary>
            /// An attempt was made to load a program with an incorrect format.
            /// </summary>
            ERROR_BAD_FORMAT = 11,
            ERROR_NO_MORE_ITEMS = 0x00000103,
            ERROR_MORE_DATA = 234,

            ERROR_INVALID_PARAMETER = 87,
            ERROR_BUSY = 107,
            /// <summary>
            /// The dwFlags parameter has a value that is not valid.
            /// </summary>
            NTE_BAD_FLAGS = 0x80090009,
            /// <summary>
            /// The user password has changed since the private keys were encrypted.
            /// </summary>
            NTE_BAD_KEY_STATE = 0x8009000B,
            /// <summary>
            /// The key container could not be opened. A common cause of this error is that the key container does not exist. To create a key container, call CryptAcquireContext using the CRYPT_NEWKEYSET flag. This error code can also indicate that access to an existing key container is denied. Access rights to the container can be granted by the key set creator by using CryptSetProvParam.
            /// </summary>
            NTE_BAD_KEYSET = 0x80090016,
            /// <summary>
            /// The pszContainer or pszProvider parameter is set to a value that is not valid.
            /// </summary>
            NTE_BAD_KEYSET_PARAM = 0x8009001F,
            /// <summary>
            /// The value of the dwProvType parameter is out of range. All provider types must be from 1 through 999, inclusive.
            /// </summary>
            NTE_BAD_PROV_TYPE = 0x80090014,
            /// <summary>
            /// The provider DLL signature could not be verified. Either the DLL or the digital signature has been tampered with.
            /// </summary>
            NTE_BAD_SIGNATURE = 0x80090006,
            /// <summary>
            /// The dwFlags parameter is CRYPT_NEWKEYSET, but the key container already exists.
            /// </summary>
            NTE_EXISTS = 0x8009000F,
            /// <summary>
            /// The pszContainer key container was found but is corrupt.
            /// </summary>
            NTE_KEYSET_ENTRY_BAD = 0x8009001A,
            /// <summary>
            /// The requested provider does not exist.
            /// </summary>
            NTE_KEYSET_NOT_DEF = 0x80090019,
            /// <summary>
            /// The CSP ran out of memory during the operation.
            /// </summary>
            NTE_NO_MEMORY = 0x8009000E,
            /// <summary>
            /// The provider DLL file does not exist or is not on the current path.
            /// </summary>
            NTE_PROV_DLL_NOT_FOUND = 0x8009001E,
            /// <summary>
            /// The provider type specified by dwProvType is corrupt. This error can relate to either the user default CSP list or the computer default CSP list.
            /// </summary>
            NTE_PROV_TYPE_ENTRY_BAD = 0x80090018,
            /// <summary>
            /// The provider type specified by dwProvType does not match the provider type found. Note that this error can only occur when pszProvider specifies an actual CSP name.
            /// </summary>
            NTE_PROV_TYPE_NO_MATCH = 0x8009001B,
            /// <summary>
            /// No entry exists for the provider type specified by dwProvType.
            /// </summary>
            NTE_PROV_TYPE_NOT_DEF = 0x80090017,
            /// <summary>
            /// The provider DLL file could not be loaded or failed to initialize.
            /// </summary>
            NTE_PROVIDER_DLL_FAIL = 0x8009001D,
            /// <summary>
            /// An error occurred while loading the DLL file image, prior to verifying its signature.
            /// </summary>
            NTE_SIGNATURE_FILE_BAD = 0x8009001C,
            //Unofficial description, http://serverfault.com/questions/673212/how-to-give-a-user-access-to-the-certificate-store-on-windows-server-2012
            /// <summary>
            /// No permission, access denied.
            /// </summary>
            NTE_PERM = 0x80090010,
            NTE_BAD_KEY = 0x80090003,
            NTE_BAD_UID = 0x80090001,
            //Unofficial description.
            /// <summary>
            /// The key wasn't found.
            /// </summary>
            NTE_NO_KEY = 0x8009000D,

            GENERAL_EXCEPTION = 0x80131500, //unofficial enum value, .NET returns this HResult when an exception is thrown.
        }

        public static class USER32
        {
            public enum SystemMetric
            {
                SM_CXSCREEN = 0,  // 0x00
                SM_CYSCREEN = 1,  // 0x01
                SM_CXVSCROLL = 2,  // 0x02
                SM_CYHSCROLL = 3,  // 0x03
                SM_CYCAPTION = 4,  // 0x04
                SM_CXBORDER = 5,  // 0x05
                SM_CYBORDER = 6,  // 0x06
                SM_CXDLGFRAME = 7,  // 0x07
                SM_CXFIXEDFRAME = 7,  // 0x07
                SM_CYDLGFRAME = 8,  // 0x08
                SM_CYFIXEDFRAME = 8,  // 0x08
                SM_CYVTHUMB = 9,  // 0x09
                SM_CXHTHUMB = 10, // 0x0A
                SM_CXICON = 11, // 0x0B
                SM_CYICON = 12, // 0x0C
                SM_CXCURSOR = 13, // 0x0D
                SM_CYCURSOR = 14, // 0x0E
                SM_CYMENU = 15, // 0x0F
                SM_CXFULLSCREEN = 16, // 0x10
                SM_CYFULLSCREEN = 17, // 0x11
                SM_CYKANJIWINDOW = 18, // 0x12
                SM_MOUSEPRESENT = 19, // 0x13
                SM_CYVSCROLL = 20, // 0x14
                SM_CXHSCROLL = 21, // 0x15
                SM_DEBUG = 22, // 0x16
                SM_SWAPBUTTON = 23, // 0x17
                SM_CXMIN = 28, // 0x1C
                SM_CYMIN = 29, // 0x1D
                SM_CXSIZE = 30, // 0x1E
                SM_CYSIZE = 31, // 0x1F
                SM_CXSIZEFRAME = 32, // 0x20
                SM_CXFRAME = 32, // 0x20
                SM_CYSIZEFRAME = 33, // 0x21
                SM_CYFRAME = 33, // 0x21
                SM_CXMINTRACK = 34, // 0x22
                SM_CYMINTRACK = 35, // 0x23
                SM_CXDOUBLECLK = 36, // 0x24
                SM_CYDOUBLECLK = 37, // 0x25
                SM_CXICONSPACING = 38, // 0x26
                SM_CYICONSPACING = 39, // 0x27
                SM_MENUDROPALIGNMENT = 40, // 0x28
                SM_PENWINDOWS = 41, // 0x29
                SM_DBCSENABLED = 42, // 0x2A
                SM_CMOUSEBUTTONS = 43, // 0x2B
                SM_SECURE = 44, // 0x2C
                SM_CXEDGE = 45, // 0x2D
                SM_CYEDGE = 46, // 0x2E
                SM_CXMINSPACING = 47, // 0x2F
                SM_CYMINSPACING = 48, // 0x30
                SM_CXSMICON = 49, // 0x31
                SM_CYSMICON = 50, // 0x32
                SM_CYSMCAPTION = 51, // 0x33
                SM_CXSMSIZE = 52, // 0x34
                SM_CYSMSIZE = 53, // 0x35
                SM_CXMENUSIZE = 54, // 0x36
                SM_CYMENUSIZE = 55, // 0x37
                SM_ARRANGE = 56, // 0x38
                SM_CXMINIMIZED = 57, // 0x39
                SM_CYMINIMIZED = 58, // 0x3A
                SM_CXMAXTRACK = 59, // 0x3B
                SM_CYMAXTRACK = 60, // 0x3C
                SM_CXMAXIMIZED = 61, // 0x3D
                SM_CYMAXIMIZED = 62, // 0x3E
                SM_NETWORK = 63, // 0x3F
                SM_CLEANBOOT = 67, // 0x43
                SM_CXDRAG = 68, // 0x44
                SM_CYDRAG = 69, // 0x45
                SM_SHOWSOUNDS = 70, // 0x46
                SM_CXMENUCHECK = 71, // 0x47
                SM_CYMENUCHECK = 72, // 0x48
                SM_SLOWMACHINE = 73, // 0x49
                SM_MIDEASTENABLED = 74, // 0x4A
                SM_MOUSEWHEELPRESENT = 75, // 0x4B
                SM_XVIRTUALSCREEN = 76, // 0x4C
                SM_YVIRTUALSCREEN = 77, // 0x4D
                SM_CXVIRTUALSCREEN = 78, // 0x4E
                SM_CYVIRTUALSCREEN = 79, // 0x4F
                SM_CMONITORS = 80, // 0x50
                SM_SAMEDISPLAYFORMAT = 81, // 0x51
                SM_IMMENABLED = 82, // 0x52
                SM_CXFOCUSBORDER = 83, // 0x53
                SM_CYFOCUSBORDER = 84, // 0x54
                SM_TABLETPC = 86, // 0x56
                SM_MEDIACENTER = 87, // 0x57
                SM_STARTER = 88, // 0x58
                SM_SERVERR2 = 89, // 0x59
                SM_MOUSEHORIZONTALWHEELPRESENT = 91, // 0x5B
                SM_CXPADDEDBORDER = 92, // 0x5C
                SM_DIGITIZER = 94, // 0x5E
                SM_MAXIMUMTOUCHES = 95, // 0x5F

                SM_REMOTESESSION = 0x1000, // 0x1000
                SM_SHUTTINGDOWN = 0x2000, // 0x2000
                SM_REMOTECONTROL = 0x2001, // 0x2001

                SM_CONVERTABLESLATEMODE = 0x2003,
                SM_SYSTEMDOCKED = 0x2004,
            }

            [DllImport("user32.dll")]
            public static extern int GetSystemMetrics(SystemMetric smIndex);

            public const int GWL_STYLE = -16;
            public const int WS_SYSMENU = 0x80000;

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        }

        public static class ADVAPI32
        {
            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptSetProvParam(IntPtr hProv, uint dwParam, [In] byte[] pbData, uint dwFlags);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptGetProvParam(IntPtr hProv, uint dwParam, [In, Out] byte[] pbProvData, ref uint pdwProvDataLen, uint dwFlags);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptGetProvParam(IntPtr hProv, uint dwParam, [MarshalAs(UnmanagedType.LPStr)] StringBuilder pbProvData, ref uint pdwProvDataLen, uint dwFlags);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptGetUserKey(IntPtr hProv, uint dwKeySpec, ref IntPtr hKey);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptAcquireContext(ref IntPtr hProv, string pszContainer, string pszProvider, uint dwProvType, uint dwFlags);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptReleaseContext(IntPtr hProv, uint dwFlags);

            public const string MS_DEF_PROV = "Microsoft Base Cryptographic Provider v1.0";
            public const string MS_SCARD_PROV = "Microsoft Base Smart Card Crypto Provider";

            public static uint CRYPT_NEWKEYSET = 0x8;
            public static uint CRYPT_DELETEKEYSET = 0x10;
            public static uint CRYPT_MACHINE_KEYSET = 0x20;
            public static uint CRYPT_SILENT = 0x40;
            public static uint CRYPT_DEFAULT_CONTAINER_OPTIONAL = 0x80;
            public static uint CRYPT_VERIFYCONTEXT = 0xF0000000;

            public static uint AT_KEYEXCHANGE = 1;
            public static uint AT_SIGNATURE = 2;

            public static uint KP_CERTIFICATE = 26;

            public static uint PP_CLIENT_HWND = 1;
            public static uint PP_ENUMCONTAINERS = 2;
            public static uint PP_CONTAINER = 6;
            public static uint PP_KEYSET_SEC_DESCR = 8;
            public static uint PP_UI_PROMPT = 21;
            public static uint PP_DELETEKEY = 24;
            public static uint PP_EXCHANGE_PIN = 32;
            public static uint PP_SIGNATURE_PIN = 33;
            public static uint PP_USE_HARDWARE_RNG = 38;
            public static uint PP_USER_CERTSTORE = 42;
            public static uint PP_SMARTCARD_READER = 43;
            public static uint PP_PIN_PROMPT_STRING = 44;
            public static uint PP_SMARTCARD_GUID = 45;
            public static uint PP_ROOT_CERTSTORE = 46;
            public static uint PP_SECURE_KEYEXCHANGE_PIN = 47;
            public static uint PP_SECURE_SIGNATURE_PIN = 48;

            public static uint CRYPT_FIRST = 1;
            public static uint CRYPT_NEXT = 2;

            public static uint PROV_RSA_FULL = 1;
            public static uint PROV_SSL;
            public static uint PROV_MS_EXCHANGE;
            public static uint PROV_FORTEZZA;
            public static uint PROV_DH_SCHANNEL;
            public static uint PROV_DSS_DH;
            public static uint PROV_DSS;
            public static uint PROV_RSA_SCHANNEL;
            public static uint PROV_RSA_SIG;
            public static uint PROV_RSA_AES;
        }

        public static class SCHANNEL
        {
            [DllImport("SCHANNEL.DLL", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SslEmptyCache(IntPtr pszTargetName, uint dwFlags);
        }

        public static class WININET
        {
            [DllImport("WININET.DLL", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IncrementUrlCacheHeaderData(int a, IntPtr b);
        }

        public static class KERNEL32
        {
            [DllImport("KERNEL32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr LoadLibrary(string dllName);

            [DllImport("KERNEL32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("KERNEL32.DLL")]
            public static extern uint SetThreadExecutionState(uint esFlags);
            public const uint ES_CONTINUOUS = 0x80000000;
            public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        }
    }
}
