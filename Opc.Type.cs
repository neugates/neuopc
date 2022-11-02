//============================================================================
// TITLE: Type.cs
//
// CONTENTS:
// 
// Defines constants for standard data types.
//
// (c) Copyright 2002-2003 The OPC Foundation
// ALL RIGHTS RESERVED.
//
// DISCLAIMER:
//  This code is provided by the OPC Foundation solely to assist in 
//  understanding and use of the appropriate OPC Specification(s) and may be 
//  used as set forth in the License Grant section of the OPC Specification.
//  This code is provided as-is and without warranty or support of any sort
//  and is subject to the Warranty and Liability Disclaimers which appear
//  in the printed OPC Specification.
//
// MODIFICATION LOG:
//
// Date       By    Notes
// ---------- ---   -----
// 2003/04/03 RSA   Initial implementation.

using System;
using System.Xml;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Opc
{
	/// <summary>
	/// Exposes WIN32 and COM API functions.
	/// </summary>
	public class Interop
	{
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			private struct SERVER_INFO_100
		{
			public uint   sv100_platform_id;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string sv100_name;
		} 	

		private const uint LEVEL_SERVER_INFO_100 = 100;
		private const uint LEVEL_SERVER_INFO_101 = 101;

		private const int  MAX_PREFERRED_LENGTH  = -1;

		private const uint SV_TYPE_WORKSTATION   = 0x00000001;
		private const uint SV_TYPE_SERVER        = 0x00000002;

		[DllImport("Netapi32.dll")]
		private static extern int NetServerEnum(
			IntPtr     servername,
			uint       level,
			out IntPtr bufptr,
			int        prefmaxlen,
			out int    entriesread,
			out int    totalentries,
			uint       servertype,
			IntPtr     domain,
			IntPtr     resume_handle);

		[DllImport("Netapi32.dll")]	
		private static extern int NetApiBufferFree(IntPtr buffer);

		/// <summary>
		/// Enumerates computers on the local network.
		/// </summary>
		public static string[] EnumComputers()
		{
			IntPtr pInfo;

			int entriesRead = 0;
			int totalEntries = 0;

			int result = NetServerEnum(
				IntPtr.Zero,
				LEVEL_SERVER_INFO_100,
				out pInfo,
				MAX_PREFERRED_LENGTH,
				out entriesRead,
				out totalEntries,
				SV_TYPE_WORKSTATION | SV_TYPE_SERVER,
				IntPtr.Zero,
				IntPtr.Zero);		

			string[] computers = new string[entriesRead];

			if (result != 0)
			{
				//throw new ApplicationException("NetApi Error = " + String.Format("0x{0,0:X}", result));
				return computers;
			}


			IntPtr pos = pInfo;

			for (int ii = 0; ii < entriesRead; ii++)
			{
				SERVER_INFO_100 info = (SERVER_INFO_100)Marshal.PtrToStructure(pos, typeof(SERVER_INFO_100));
				
				computers[ii] = info.sv100_name;

				pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(SERVER_INFO_100)));
			}

			NetApiBufferFree(pInfo);

			return computers;
		}
	}

	/// <summary>
	/// Defines constants for standard data types.
	/// </summary>
	public class Type
	{
		/// <remarks/>
		public static System.Type SBYTE          = typeof(sbyte);
		/// <remarks/>
		public static System.Type BYTE           = typeof(byte);
		/// <remarks/>
		public static System.Type SHORT          = typeof(short);
		/// <remarks/>
		public static System.Type USHORT         = typeof(ushort);
		/// <remarks/>
		public static System.Type INT            = typeof(int);
		/// <remarks/>
		public static System.Type UINT           = typeof(uint);
		/// <remarks/>
		public static System.Type LONG           = typeof(long);
		/// <remarks/>
		public static System.Type ULONG          = typeof(ulong);
		/// <remarks/>
		public static System.Type FLOAT          = typeof(float);
		/// <remarks/>
		public static System.Type DOUBLE         = typeof(double);
		/// <remarks/>
		public static System.Type DECIMAL        = typeof(decimal);
		/// <remarks/>
		public static System.Type BOOLEAN        = typeof(bool);
		/// <remarks/>
		public static System.Type DATETIME       = typeof(DateTime);
		/// <remarks/>
		public static System.Type DURATION       = typeof(TimeSpan);
		/// <remarks/>
		public static System.Type STRING         = typeof(string);
		/// <remarks/>
		public static System.Type ANY_TYPE       = typeof(object);
		/// <remarks/>
		public static System.Type BINARY         = typeof(byte[]);
		/// <remarks/>
		public static System.Type ARRAY_SHORT    = typeof(short[]);
		/// <remarks/>
		public static System.Type ARRAY_USHORT   = typeof(ushort[]);
		/// <remarks/>
		public static System.Type ARRAY_INT      = typeof(int[]);
		/// <remarks/>
		public static System.Type ARRAY_UINT     = typeof(uint[]);
		/// <remarks/>
		public static System.Type ARRAY_LONG     = typeof(long[]);
		/// <remarks/>
		public static System.Type ARRAY_ULONG    = typeof(ulong[]);
		/// <remarks/>
		public static System.Type ARRAY_FLOAT    = typeof(float[]);
		/// <remarks/>
		public static System.Type ARRAY_DOUBLE   = typeof(double[]);
		/// <remarks/>
		public static System.Type ARRAY_DECIMAL  = typeof(decimal[]);
		/// <remarks/>
		public static System.Type ARRAY_BOOLEAN  = typeof(bool[]);
		/// <remarks/>
		public static System.Type ARRAY_DATETIME = typeof(DateTime[]);
		/// <remarks/>
		public static System.Type ARRAY_STRING   = typeof(string[]);
		/// <remarks/>
		public static System.Type ARRAY_ANY_TYPE = typeof(object[]);

		/// <summary>
		/// Returns an array of all well-known types.
		/// </summary>
		public static System.Type[] Enumerate()
		{
			ArrayList values = new ArrayList();

			FieldInfo[] fields = typeof(Opc.Type).GetFields(BindingFlags.Static | BindingFlags.Public);

			foreach (FieldInfo field in fields)
			{
				values.Add(field.GetValue(typeof(System.Type)));
			}

			return (System.Type[])values.ToArray(typeof(System.Type));
		}

		/// <summary>
		/// Converts the VARTYPE to a system type.
		/// </summary>
		public static System.Type GetType(short input)
		{				
			try
			{
				return GetType((VarEnum)Enum.ToObject(typeof(VarEnum), input));
			}
			catch
			{
				return typeof(object);
			}
		}

		/// <summary>
		/// Converts the VARTYPE to a system type.
		/// </summary>
		internal static System.Type GetType(VarEnum input)
		{				
			switch (input)
			{
				case VarEnum.VT_I1:                         return typeof(sbyte);
				case VarEnum.VT_UI1:                        return typeof(byte);
				case VarEnum.VT_I2:                         return typeof(short);
				case VarEnum.VT_UI2:                        return typeof(ushort);
				case VarEnum.VT_I4:                         return typeof(int);
				case VarEnum.VT_UI4:                        return typeof(uint);
				case VarEnum.VT_R4:                         return typeof(float);
				case VarEnum.VT_R8:                         return typeof(double);
				case VarEnum.VT_CY:                         return typeof(decimal);
				case VarEnum.VT_BOOL:                       return typeof(bool);
				case VarEnum.VT_DATE:                       return typeof(DateTime);
				case VarEnum.VT_BSTR:                       return typeof(string);
				case VarEnum.VT_ARRAY | VarEnum.VT_I1:      return typeof(sbyte[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI1:     return typeof(byte[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_I2:      return typeof(short[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI2:     return typeof(ushort[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_I4:      return typeof(int[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI4:     return typeof(uint[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_R4:      return typeof(float[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_R8:      return typeof(double[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_CY:      return typeof(decimal[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_BOOL:    return typeof(bool[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_DATE:    return typeof(DateTime[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_BSTR:    return typeof(string[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_VARIANT: return typeof(object[]);
				default:                                    return typeof(object);
			}
		}
		
		/// <summary>
		/// Converts the system type to a VARTYPE.
		/// </summary>
		public static short GetType(System.Type input)
		{
			return (short)GetVarType(input);
		}

		/// <summary>
		/// Converts the system type to a VARTYPE.
		/// </summary>
		internal static VarEnum GetVarType(System.Type input)
		{				
			if (input == null)               return VarEnum.VT_EMPTY;
			if (input == typeof(sbyte))      return VarEnum.VT_I1;
			if (input == typeof(byte))       return VarEnum.VT_UI1;
			if (input == typeof(short))      return VarEnum.VT_I2;
			if (input == typeof(ushort))     return VarEnum.VT_UI2;
			if (input == typeof(int))        return VarEnum.VT_I4;
			if (input == typeof(uint))       return VarEnum.VT_UI4;
			if (input == typeof(long))       return VarEnum.VT_I4;
			if (input == typeof(ulong))      return VarEnum.VT_UI4;
			if (input == typeof(float))      return VarEnum.VT_R4;
			if (input == typeof(double))     return VarEnum.VT_R8;
			if (input == typeof(decimal))    return VarEnum.VT_CY;
			if (input == typeof(bool))       return VarEnum.VT_BOOL;
			if (input == typeof(DateTime))   return VarEnum.VT_DATE;
			if (input == typeof(string))     return VarEnum.VT_BSTR;
			if (input == typeof(object))     return VarEnum.VT_EMPTY;
			if (input == typeof(sbyte[]))    return VarEnum.VT_ARRAY | VarEnum.VT_I1;
			if (input == typeof(byte[]))     return VarEnum.VT_ARRAY | VarEnum.VT_UI1;
			if (input == typeof(short[]))    return VarEnum.VT_ARRAY | VarEnum.VT_I2;
			if (input == typeof(ushort[]))   return VarEnum.VT_ARRAY | VarEnum.VT_UI2;
			if (input == typeof(int[]))      return VarEnum.VT_ARRAY | VarEnum.VT_I4;
			if (input == typeof(uint[]))     return VarEnum.VT_ARRAY | VarEnum.VT_UI4;
			if (input == typeof(long[]))     return VarEnum.VT_ARRAY | VarEnum.VT_I4;
			if (input == typeof(ulong[]))    return VarEnum.VT_ARRAY | VarEnum.VT_UI4;
			if (input == typeof(float[]))    return VarEnum.VT_ARRAY | VarEnum.VT_R4;
			if (input == typeof(double[]))   return VarEnum.VT_ARRAY | VarEnum.VT_R8;
			if (input == typeof(decimal[]))  return VarEnum.VT_ARRAY | VarEnum.VT_CY;
			if (input == typeof(bool[]))     return VarEnum.VT_ARRAY | VarEnum.VT_BOOL;
			if (input == typeof(DateTime[])) return VarEnum.VT_ARRAY | VarEnum.VT_DATE;
			if (input == typeof(string[]))   return VarEnum.VT_ARRAY | VarEnum.VT_BSTR;
			if (input == typeof(object[]))   return VarEnum.VT_ARRAY | VarEnum.VT_VARIANT;
			
			return VarEnum.VT_EMPTY;
		}
	}
}
