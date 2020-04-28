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
    }
}