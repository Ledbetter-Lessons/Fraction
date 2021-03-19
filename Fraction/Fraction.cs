using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

	using This = Fraction;                                          //Used to have shorter code. :)

/// <summary>this class is like a type int, double, etc with built in functions</summary>
[Serializable, StructLayout(LayoutKind.Sequential)]
public class Fraction : IEquatable<Fraction>
{
	/// <summary>value used in trying to fix precision errors.</summary>
	public static double epsilon = 1e-12;

	/// <summary>numerator</summary>
	public double N { get; set; }
	/// <summary>denominator</summary>
	public double D { get; set; }

	#region constructor
	public Fraction(double numerator, double denominator)
	{
		if (denominator == 0)                                   //division by 0 is undefined
			throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));

		this.N = numerator;
		this.D = denominator;
	}

	public Fraction(double dbl)
	{
		Fraction F = fromDecimal(dbl);
		this.N = F.N;
		this.D = F.D;
	}

	public Fraction(string fraction)
	{
		Fraction F = fromString(fraction);
		this.N = F.N;
		this.D = F.D;
	}
	#endregion

	#region convert from
	/// <summary>private because you can use (Fraction = double) operator</summary>
	private static This fromDecimal(double dec)
	{
		double neg = 1;
		double dbl = dec;
		if (dbl < 0)
		{
			dbl = Math.Abs(dbl);                                // remove - sign
			neg = -1;                                           // store - in variable
		}
		var whole = Math.Truncate(dbl);                         // truncate gets integer part before decimal

		if (whole == dbl)                                       // there is no decimal part
			return new This(whole * neg, 1);

		double decpart = dbl - whole;                           //  0.#####
		string sdecpart = decpart.ToString().Substring(2);      // "0.#####"    (2 = sdecPart.IndexOf(".") + 1)

		double n = double.Parse(sdecpart);
		double d = Math.Pow(10, sdecpart.Length);
		n += d * whole;
		return _Simplify(n * neg, d);                           // probably could remove simplify... 
	}
	/// <summary>resturns a Fraction object from a string representation of a fraction. Ex: "1/3"</summary>
	private static This fromString(string fraction)
	{
		double n = 0;
		double d = 0;
		string[] s = fraction.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

		#region Exceptions

		if (s.Length != 2)
			throw new ArgumentException("Unrecognized fraction formated string. Should be 1/2 or similar.", nameof(fraction));

		if (!double.TryParse(s[0], out n))
			throw new ArgumentException("Unrecognized fraction formated string. Numerator is not a number.", nameof(fraction));

		if (!double.TryParse(s[1], out d))
			throw new ArgumentException("Unrecognized fraction formated string. Denominator is not a number.", nameof(fraction));

		if (n != Math.Truncate(n))
			throw new ArgumentException("string values can not be a decimal number", nameof(fraction));

		if (d != Math.Truncate(d))
			throw new ArgumentException("string values can not be a decimal number", nameof(fraction));

		#endregion

		return _Simplify(n, d);
	}
	#endregion

	#region √ root
	/// <summary>gets the Nth root. Square root N=2/1, cube root N=3/1....</summary>
	public void Root(This f)
	{
		Pow(new This(f.D, f.N));                                // invert numerator and denominator for power
	}
	#endregion

	#region ^ power

	/// <summary>Raies the internal numerator/denominator to a power</summary>
	/// <param name="power">power to raise base to</param>
	public void Pow(This power)
	{
		This b = new This(this.N, this.D);
		This p = _pow(b, power);
		this.N = p.N;
		this.D = p.D;
	}

	/// <summary>Gets a fraction raised to a power</summary>
	/// <param name="bas">base</param>
	/// <param name="pow">power to raise base to</param>
	/// <returns>result</returns>
	private static This _pow(This bas, This pow)
	{
		This b = new This(bas.N, bas.D);                        // so that b is not overwritten
		This p = new This(pow.N, pow.D);                        // so that b is not overwritten

		if (b.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(bas));

		if (p.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(pow));

		if (b.N == 0)                                           // 0 to any power = 0
			return new This(0, 1);

		if (p.N == 0)                                           // anything (except 0) to the power of 0 = 1
			return new This(1, 1);

		if (p.N * p.D < 0)                                      // check for negative exponent (- * - = +)
		{
			double t = b.N;                                     // if negative invert fraction
			b.N = b.D;
			b.D = t;
			p.N = Math.Abs(p.N);                                // make positive
			p.D = Math.Abs(p.D);                                // make positive
		}

		//check for negative sine in base (remove - to prevent Math.Pow errors)
		double neg = 1;
		if (b.N * b.D < 0)
			neg = -1;

		b.N = Math.Abs(b.N);
		b.D = Math.Abs(b.D);

		// get Nth root (Nth = pow.denominator)
		double n = Math.Pow(b.N, 1.0 / p.D);
		double d = Math.Pow(b.D, 1.0 / p.D);
		n = Math.Pow(n, p.N);
		d = Math.Pow(d, p.N);

		if (neg == -1)
		{
			if (p.N * p.D % 2 == 0)                             // multiplied base by itself an even number of times
			{
				neg = 1;                                        // so we can remove neg sign
			}
		}

		return _Simplify(n * neg, d);
	}

	#endregion

	#region GCD (greatest common divisor)
	/// <summary>gets the greatest common divisor of the numerator and denominator</summary>
	public double GCD()
	{
		return GCD(this.N, this.D);
	}

	private static double GCD(double numerator, double denominator)
	{
		double n = Math.Abs(numerator);     // assigned n and d to the answer numerator/denominator, as well as an
		double d = Math.Abs(denominator);   // empty integer, this is to make code more simple and easier to read
		double m;                           // m =min(n,d)

		m = Math.Min(n, d);

		for (double i = m; i >= 1; i--)     // assign i to equal to m, make sure if i is greater than or equal to 1, 
		{                                   // then take away from it										
			if (n % i == 0 && d % i == 0)   // if there us no remainder, then both divisible by i. 
			{                               // Since we start with m we know it is greatest												
				return i;                   // return the value of i (need to check for 0)
			}
		}

		return 1;                           //No GDC so return 1 since all numbers are divisible by 1
	}

	#endregion

	#region simplify
	/// <summary>Simplifies internal fraction. Ex:2/4 = 1/2.</summary>
	public void Simplify()
	{
		This f = _Simplify(this.N, this.D);
		this.N = f.N;
		this.D = f.D;
	}

	private static This _Simplify(double Numerator, double Denominator)
	{
		double n = Numerator;
		double d = Denominator;

		if (d == 0)                                             // division by 0 is undefined
			throw new ArgumentException("Denominator cannot be zero.", nameof(Denominator));

		if (n == 0)                                             // =0
			return new This(0, 1);

		double gcdNum = GCD(n, d);                              // assign an integer to the gcd value

		if (gcdNum != 0)
		{
			n = n / gcdNum;                                     // simpify by dividing by GCD
			d = d / gcdNum;
		}

		if (d < 0)
		{                                                       // makes denominator always positive 
			d = d * -1;                                         // this way negative is out front
			n = n * -1;                                         // -1/4 instead of 1/-4
		}

		return fixPrecision(new This(n, d));
	}
	#endregion

	#region fix precision error
	/// <summary>Attempst to fix precission errors</summary>
	static This fixPrecision(This fraction)
	{
		double p1 = fraction.N / fraction.D;
		double p2 = Convert.ToInt64(p1);

		if (Math.Abs(p1 - p2) < (Math.Abs(p1) * epsilon))       //rounding error....
		{
			Console.WriteLine("Compensation was made for rounding error: " + fraction.N + " / " + fraction.D);
			return new This(p2, 1.0);
		}

		return new This(fraction.N, fraction.D);
	}
	#endregion

	#region overrides

	#region HashCode

	[MethodImpl(MethodImplOptions.AggressiveInlining)]          // hashcode - allows use as keys in hash tables
	public override int GetHashCode()
	{ return (N / D).GetHashCode(); }

	#endregion

	#region Assigners =

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator double(This value)          // assign values - This
	{ return new This(value); }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator This(double value)          // assign values - double
	{ return new This(value); }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator This(string value)          // assign values - string
	{ return new This(value); }

	#endregion

	#region Comparitors <, >, <=, >=, ==, !=, Equals

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(This lhs, This rhs)          // !=
	{ return !(lhs == rhs); }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(This lhs, This rhs)           // <
	{ return lhs.N / lhs.D < rhs.N / rhs.D; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(This lhs, This rhs)           // >
	{ return lhs.N / lhs.D > rhs.N / rhs.D; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(This lhs, This rhs)          // <=
	{ return lhs.N / lhs.D <= rhs.N / rhs.D; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(This lhs, This rhs)          // >=
	{ return lhs.N / lhs.D >= rhs.N / rhs.D; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(This lhs, This rhs)          // == 
	{
		if (ReferenceEquals(lhs, null))
		{
			if (ReferenceEquals(rhs, null))
				return true;

			return false;
		}
		return lhs.Equals(rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]          // this = object
	public override bool Equals(object obj)
	{ return Equals((This)obj); }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(This obj)                                // this = This (implements interface)
	{
		if (ReferenceEquals(obj, null))
			return false;

		if (ReferenceEquals(this, obj))
			return true;

		if (GetType() != obj.GetType())
			return false;

		This fl = _Simplify(obj.N, obj.D);                      // simplify because 1/2 = 2/4
		This fr = _Simplify(this.N, this.D);

		if (fl.N == fr.N)
			if (fl.D == fr.D)
				return true;

		return false;
	}
	#endregion

	#region Add +, ++

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator +(This a)                       // + [+a] 
	{ return a; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator +(This lhs, This rhs)           // + [a+b] (also simplifies)
	{
		if (lhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(lhs));

		if (rhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(rhs));

		if (lhs.N == 0)
			return (new This(rhs.N, rhs.D));

		if (rhs.N == 0)
			return (new This(lhs.N, lhs.D));

		double commonDenominator = lhs.D * rhs.D;
		double numerator = lhs.N * rhs.D;
		numerator += rhs.N * lhs.D;
		return _Simplify(numerator, commonDenominator);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator ++(This a)                      // ++ [a=a+1] (also simplifies)
	{ return _Simplify(a.N + a.D, a.D); }

	#endregion

	#region Subtract -, --

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator -(This a)                       // - [-a]
	{ return new This(-a.N, a.D); }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator -(This lhs, This rhs)           // - [a-b] (also simplifies)
	{
		if (lhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(lhs));

		if (rhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(rhs));

		if (lhs.N == 0)
			return (new This(rhs.N * -1, rhs.D));

		if (rhs.N == 0)
			return (new This(lhs.N, lhs.D));

		double cd = lhs.D * rhs.D;
		double n = lhs.N * rhs.D;
		n += rhs.N * lhs.D * -1;
		return _Simplify(n, cd);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator --(This a)                      // --
	{ return _Simplify(a.N - a.D, a.D); }

	#endregion

	#region multiply *
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator *(This lhs, This rhs)           // (n1/d1) * (n2/d2) (also simplifies)
	{
		if (lhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(lhs));

		if (rhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(rhs));

		double n = lhs.N * rhs.N;
		double d = lhs.D * rhs.D;
		return _Simplify(n, d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator *(double a, This b)             // a * (n/d)
	{ return fromDecimal(a) * b; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator *(This a, double b)             // (n/d) * a
	{ return a * fromDecimal(b); }

	#endregion

	#region divide /
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator /(This lhs, This rhs)           // (n1/d1) / (n2/d2)
	{
		if (lhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(lhs));

		if (rhs.D == 0)
			throw new ArgumentException("Denominator cannot be zero.", nameof(rhs));

		if (rhs.N == 0)
			throw new DivideByZeroException();

		double numerator = lhs.N * rhs.D;                       //multiply by reciprical of 2nd fraction
		double denominator = lhs.D * rhs.N;
		return _Simplify(numerator, denominator);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator /(double lhs, This rhs)         // a / (n/d)
	{ return fromDecimal(lhs) / rhs; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator /(This lhs, double rhs)         // (n/d) / a
	{ return lhs / fromDecimal(rhs); }

	#endregion

	#region power ^

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator ^(This a, This b)               // (n1/d1) ^ (n2/d2)
	{
		return _pow(a, b);
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator ^(double a, This b)             // a ^ (n/d)
	{ return fromDecimal(a) ^ b; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static This operator ^(This a, double b)             // (n/d) ^ a
	{ return a ^ fromDecimal(b); }

	#endregion
	#endregion

	#region ToString and Print
	public override string ToString()
	{
		if (this.D == 1)
			return string.Format("{0}", this.N);
		if (this.D == 0)
			return string.Format("Undefined!");

		return string.Format("{0}/{1}", this.N, this.D);
	}

	#region print fraction
	/// <summary>Print fraction to console</summary>
	public void Print()
	{
		Console.WriteLine(this.ToString());
	}
	#endregion
	#endregion
}