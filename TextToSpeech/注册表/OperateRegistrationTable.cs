using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


    public class OperateRegistrationTable
{
//         static UIntPtr HKEY_CLASSES_ROOT = (UIntPtr)0x80000000;
//         static UIntPtr HKEY_CURRENT_USER = (UIntPtr)0x80000001;
//         static UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
//         static UIntPtr HKEY_USERS = (UIntPtr)0x80000003;
//         static UIntPtr HKEY_CURRENT_CONFIG = (UIntPtr)0x80000005;

//    34         // 关闭64位（文件系统）的操作转向
// 35          [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
// 36         public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
// 37         // 开启64位（文件系统）的操作转向
// 38          [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
// 39         public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);    
// 40    
// 41         // 获取操作Key值句柄
// 42          [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
// 43         public static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions,
//int samDesired, out IntPtr phkResult);
// 44         //关闭注册表转向（禁用特定项的注册表反射）
// 45         [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
// 46         public static extern long RegDisableReflectionKey(IntPtr hKey);
// 47         //使能注册表转向（开启特定项的注册表反射）
// 48         [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
// 49         public static extern long RegEnableReflectionKey(IntPtr hKey);
// 50         //获取Key值（即：Key值句柄所标志的Key对象的值）
// 51         [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
// 52         private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved,
//                                                   out uint lpType, System.Text.StringBuilder lpData,
//                                                    ref uint lpcbData);

}


