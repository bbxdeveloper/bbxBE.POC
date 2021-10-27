using System;

namespace bbxBE.POC.Domain.Common
{
    public abstract class BaseEntity
    {
        public virtual Guid Id { get; set; }
    }
}