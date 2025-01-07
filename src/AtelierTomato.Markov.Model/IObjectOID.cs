namespace AtelierTomato.Markov.Model
{
	public abstract class IObjectOID
	{
		public abstract ServiceType Service { get; }
		public abstract string? Instance { get; set; }
		public abstract IObjectOID Base();
		public abstract override string ToString();

		public override bool Equals(object? obj)
		{
			if (obj is not IObjectOID other) return false;
			return ToString() == other.ToString();
		}

		public override int GetHashCode() => ToString().GetHashCode();

		public static bool operator ==(IObjectOID? left, IObjectOID? right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (left is null || right is null) return false;
			return left.Equals(right);
		}

		public static bool operator !=(IObjectOID? left, IObjectOID? right) => !(left == right);
	}
}
