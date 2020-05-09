using System;

namespace NpuRozklad.Core.Entities
{
    public abstract class TypeIdTrait : IEquatable<TypeIdTrait>
    {
        protected TypeIdTrait(string typeId)
        {
            TypeId = typeId ?? throw new ArgumentNullException();
        }

        public virtual string TypeId { get; }
        
        public bool Equals(TypeIdTrait other)
        {
            return other != null && TypeId == other.TypeId;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeIdTrait val && Equals(val);
        }

        public override int GetHashCode()
        {
            return TypeId.GetHashCode();
        }
        
        public static bool operator == (TypeIdTrait obj1, TypeIdTrait obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.TypeId == obj2.TypeId;
        }

        public static bool operator !=(TypeIdTrait obj1, TypeIdTrait obj2)
        {
            return !(obj1 == obj2);
        }
    }
}