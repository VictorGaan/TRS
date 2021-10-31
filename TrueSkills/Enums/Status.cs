using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueSkills.Enums
{
    public enum StatusSpecification
    {
        /// <summary>
        /// Подошёл
        /// </summary>
        Fit = 1,
        /// <summary>
        /// Не подошёл
        /// </summary>
        NotFit = 3,
        /// <summary>
        /// Не определить
        /// </summary>
        NotDetermined = 2
    }
}
