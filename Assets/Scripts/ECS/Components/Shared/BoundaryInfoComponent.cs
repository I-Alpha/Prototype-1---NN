using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Borgs
{
    public struct BoundaryInfoComponent : IComponentData
    {
        public BoundaryType boundaryType;
    }
}
