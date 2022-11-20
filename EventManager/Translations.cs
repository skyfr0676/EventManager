using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager
{
    public class Translations : ITranslation
    {
        public string ScientistName { get; set; } = "Scientist";
        public string DBoyName { get; set; } = "DBoy";
    }
}
