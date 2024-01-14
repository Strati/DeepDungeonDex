using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepDungeonDex.Data
{
    public interface IRepoData<T>
    {
        T Clone();
    }
}
