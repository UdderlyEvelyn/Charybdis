using System;
using System.Linq;
using System.Collections.Generic;

namespace Charybdis.Library.Core
{
	/// <summary>
	/// Contains functions for processing data into other types of data, or performing generic/common actions on data - includes many convenience wrappers to make code more readable.
	/// </summary>
	public static class Numeric
	{
		public static Type[] Types = new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };
		public static Type[] IntegerTypes = new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong) };
		public static Type[] SignedIntegerTypes = new[] { typeof(sbyte), typeof(short), typeof(int), typeof(long) };
		public static Type[] UnsignedIntegerTypes = new[] { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
		public static Type[] FloatingPointTypes = new[] { typeof(float), typeof(double), typeof(decimal) };
		
		#region Missing Methods In .NET
 
		//Methods For SByte

        /// <summary>
        /// Computes the sum of a sequence of <c>sbyte</c> values.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OverflowException"/>
        public static SByte Sum(this IEnumerable<SByte> e)
        {
            return checked((SByte)e.Sum<SByte>(n => n));
        }

		//End Methods For SByte
 
		//Methods For Byte

        /// <summary>
        /// Computes the sum of a sequence of <c>byte</c> values.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OverflowException"/>
        public static Byte Sum(this IEnumerable<Byte> e)
        {
            return checked((Byte)e.Sum<Byte>(n => n));
        }

		//End Methods For Byte
 
		//Methods For Int16

        /// <summary>
        /// Computes the sum of a sequence of <c>short</c> values.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OverflowException"/>
        public static Int16 Sum(this IEnumerable<Int16> e)
        {
            return checked((Int16)e.Sum<Int16>(n => n));
        }

		//End Methods For Int16
 
		//Methods For UInt16

        /// <summary>
        /// Computes the sum of a sequence of <c>ushort</c> values.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OverflowException"/>
        public static UInt16 Sum(this IEnumerable<UInt16> e)
        {
            return checked((UInt16)e.Sum<UInt16>(n => n));
        }

		//End Methods For UInt16

		#endregion

		#region Numeric Types
 
        public static SByte DotProduct(this SByte[] a1, SByte[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<SByte, SByte, SByte>(a2, (n1, n2) => checked((SByte)(n1 * n2))).Sum();
        }

		public static SByte Wrap(this SByte value, SByte delta, SByte min, SByte max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (SByte)nv;
        }
 
        public static Byte DotProduct(this Byte[] a1, Byte[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<Byte, Byte, Byte>(a2, (n1, n2) => checked((Byte)(n1 * n2))).Sum();
        }

		public static Byte Wrap(this Byte value, Byte delta, Byte min, Byte max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (Byte)nv;
        }
 
        public static Int16 DotProduct(this Int16[] a1, Int16[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<Int16, Int16, Int16>(a2, (n1, n2) => checked((Int16)(n1 * n2))).Sum();
        }

		public static Int16 Wrap(this Int16 value, Int16 delta, Int16 min, Int16 max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (Int16)nv;
        }
 
        public static UInt16 DotProduct(this UInt16[] a1, UInt16[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<UInt16, UInt16, UInt16>(a2, (n1, n2) => checked((UInt16)(n1 * n2))).Sum();
        }

		public static UInt16 Wrap(this UInt16 value, UInt16 delta, UInt16 min, UInt16 max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (UInt16)nv;
        }
 
        public static Int32 DotProduct(this Int32[] a1, Int32[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<Int32, Int32, Int32>(a2, (n1, n2) => checked((Int32)(n1 * n2))).Sum();
        }

		public static Int32 Wrap(this Int32 value, Int32 delta, Int32 min, Int32 max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (Int32)nv;
        }
 
        public static Int64 DotProduct(this Int64[] a1, Int64[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<Int64, Int64, Int64>(a2, (n1, n2) => checked((Int64)(n1 * n2))).Sum();
        }

		public static Int64 Wrap(this Int64 value, Int64 delta, Int64 min, Int64 max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (Int64)nv;
        }
 
        public static Single DotProduct(this Single[] a1, Single[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<Single, Single, Single>(a2, (n1, n2) => checked((Single)(n1 * n2))).Sum();
        }

		public static Single Wrap(this Single value, Single delta, Single min, Single max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (Single)nv;
        }
 
        public static Double DotProduct(this Double[] a1, Double[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<Double, Double, Double>(a2, (n1, n2) => checked((Double)(n1 * n2))).Sum();
        }

		public static Double Wrap(this Double value, Double delta, Double min, Double max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (Double)nv;
        }
 
        public static Decimal DotProduct(this Decimal[] a1, Decimal[] a2)
        {
            if (a1.Length != a2.Length)
                throw new InvalidOperationException("The two arrays must be of equal length to form a valid dot product.");
            return a1.Zip<Decimal, Decimal, Decimal>(a2, (n1, n2) => checked((Decimal)(n1 * n2))).Sum();
        }

		public static Decimal Wrap(this Decimal value, Decimal delta, Decimal min, Decimal max)
        {
            var nv = value + delta;
            if (nv > max)
                nv -= max;
            else if (nv < min)
                nv += max;
            return (Decimal)nv;
        }

		#endregion

		#region Integers (Signed & Unsigned)

		//Methods For SByte

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this SByte i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For SByte

		//Methods For Byte

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this Byte i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For Byte

		//Methods For Int16

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this Int16 i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					if (i > 999) //More than 3 digits?
						return 4;
					else
						return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For Int16

		//Methods For UInt16

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this UInt16 i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					if (i > 999) //More than 3 digits?
						if (i > 9999) //More than 4 digits?
							return 5;
						else
							return 4;
					else
						return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For UInt16

		//Methods For Int32

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this Int32 i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					if (i > 999) //More than 3 digits?
						if (i > 9999) //More than 4 digits?
							if (i > 99999) //More than 5 digits?
								if (i > 999999) //More than 6 digits?
									if (i > 9999999) //More than 7 digits?
										if (i > 99999999) //More than 8 digits?
											return 9;
										else
											return 8;
									else
										return 7;
								else
									return 6;
							else
								return 5;
						else
							return 4;
					else
						return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For Int32

		//Methods For UInt32

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this UInt32 i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					if (i > 999) //More than 3 digits?
						if (i > 9999) //More than 4 digits?
							if (i > 99999) //More than 5 digits?
								if (i > 999999) //More than 6 digits?
									if (i > 9999999) //More than 7 digits?
										if (i > 99999999) //More than 8 digits?
											if (i > 999999999) //More than 9 digits?
												return 10;
											else
												return 9;
										else
											return 8;
									else
										return 7;
								else
									return 6;
							else
								return 5;
						else
							return 4;
					else
						return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For UInt32

		//Methods For Int64

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this Int64 i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					if (i > 999) //More than 3 digits?
						if (i > 9999) //More than 4 digits?
							if (i > 99999) //More than 5 digits?
								if (i > 999999) //More than 6 digits?
									if (i > 9999999) //More than 7 digits?
										if (i > 99999999) //More than 8 digits?
											if (i > 999999999) //More than 9 digits?
												if (i > 9999999999) //More than 10 digits?
													if (i > 99999999999) //More than 11 digits?
														if (i > 999999999999) //More than 12 digits?
															if (i > 9999999999999) //More than 13 digits?
																if (i > 99999999999999) //More than 14 digits?
																	if (i > 999999999999999) //More than 15 digits?
																		if (i > 9999999999999999) //More than 16 digits?
																			if (i > 99999999999999999) //More than 17 digits?
																				if (i > 999999999999999999) //More than 18 digits?
																					return 19; //Max for long/ulong, which is the largest this method supports.
																				else
																					return 18;
																			else
																				return 17;
																		else
																			return 16;
																	else
																		return 15;
																else
																	return 14;
															else
																return 13;
														else
															return 12;
													else 
														return 11;
												else
													return 10;
											else
												return 9;
										else
											return 8;
									else
										return 7;
								else
									return 6;
							else
								return 5;
						else
							return 4;
					else
						return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For Int64

		//Methods For UInt64

		//This method is generated by a template in Numeric.tt - be sure to edit it there and keep in mind that it affects multiple numeric types.
		/// <summary>
		/// Get the digit count of an integer.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		//We are using a bunch of nested conditions instead of a loop for performance reasons. - UdderlyEvelyn
		public static int GetDigitLength(this UInt64 i)
		{
			if (i > 9) //More than 1 digit?
				if (i > 99) //More than 2 digits?
					if (i > 999) //More than 3 digits?
						if (i > 9999) //More than 4 digits?
							if (i > 99999) //More than 5 digits?
								if (i > 999999) //More than 6 digits?
									if (i > 9999999) //More than 7 digits?
										if (i > 99999999) //More than 8 digits?
											if (i > 999999999) //More than 9 digits?
												if (i > 9999999999) //More than 10 digits?
													if (i > 99999999999) //More than 11 digits?
														if (i > 999999999999) //More than 12 digits?
															if (i > 9999999999999) //More than 13 digits?
																if (i > 99999999999999) //More than 14 digits?
																	if (i > 999999999999999) //More than 15 digits?
																		if (i > 9999999999999999) //More than 16 digits?
																			if (i > 99999999999999999) //More than 17 digits?
																				if (i > 999999999999999999) //More than 18 digits?
																					return 19; //Max for long/ulong, which is the largest this method supports.
																				else
																					return 18;
																			else
																				return 17;
																		else
																			return 16;
																	else
																		return 15;
																else
																	return 14;
															else
																return 13;
														else
															return 12;
													else 
														return 11;
												else
													return 10;
											else
												return 9;
										else
											return 8;
									else
										return 7;
								else
									return 6;
							else
								return 5;
						else
							return 4;
					else
						return 3;
				else
					return 2;
			else
				return 1;
		}
		
		//End Methods For UInt64

		#endregion
		
		#region Integers (Signed Only)

		//Methods For SByte

		//End Methods For SByte

		//Methods For Int16

		//End Methods For Int16

		//Methods For Int32

		//End Methods For Int32

		//Methods For Int64

		//End Methods For Int64

		#endregion

		#region Integers (Unsigned Only)

		//Methods For Byte

		//End Methods For Byte

		//Methods For UInt16

		//End Methods For UInt16

		//Methods For UInt32

		//End Methods For UInt32

		//Methods For UInt64

		//End Methods For UInt64

		#endregion

		#region Floating Point

		//Methods For Single

		//End Methods For Single

		//Methods For Double

		//End Methods For Double

		//Methods For Decimal

		//End Methods For Decimal

		#endregion
	}
}