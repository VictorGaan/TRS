using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueSkills.Interfaces
{
    public interface IAsyncInitialization
    {
        public Task Initialization { get; set; }
    }
}
