using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alquimia.Services.QuizLogic
{
    public static class SuperFamilyMapping
    {
        public static readonly Dictionary<string, List<string>> Groups = new()
        {
            ["Floral"] = new() { "Floral", "Almizclado", "Empolvado", "Aldehídico" },
            ["Amaderada"] = new() { "Amaderado", "Terroso", "Ahumado" },
            ["Fresca"] = new() { "Cítrico", "Fresca", "Herbal", "Marino", "Frutal", "Mentolado", "Alcanforado", "Hierbas aromáticas" },
            ["Oriental"] = new() { "Ámbar", "Especiado", "Gourmand" }
        };
    }
}
