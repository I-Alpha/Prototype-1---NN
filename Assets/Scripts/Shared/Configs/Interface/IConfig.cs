using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Borgs
{
    public interface IConfig
    {
        bool IsDirty { get; set; }
        void UpdateConfig();
    }

    public abstract class Config : ScriptableObject, IConfig
    {
        public bool IsDirty { get; set; }
        public abstract void UpdateConfig();
        public void OnValidate()
        {
            IsDirty = true;
        }
    }

}
