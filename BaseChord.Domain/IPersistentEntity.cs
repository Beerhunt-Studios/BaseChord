using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseChord.Domain;

public interface IPersistentEntity : IEntity
{
    public bool IsDeleted { get; }

    public void SetAsDeleted();
}
