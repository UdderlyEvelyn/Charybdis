using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Science
{
    public class Element
    {
        public string Name { get; set; }
        public byte Number { get; set; }
        public double Mass { get; set; }
        public bool Metal { get; set; }
        public ElementCategory Category { get; set; }
        public string Symbol { get; set; }
        public Temperature MeltingPoint { get; set; }
        public Temperature BoilingPoint { get; set; }
        public Temperature FusionPoint { get; set; }
        public List<Element> FusionProducts { get; set; }
        public List<string> ArchaicNames { get; set; }

        public Element(string name, string symbol, byte number, double mass, double? meltingK = null, double? boilingK = null, double? fusionK = null, List<Element> fusionProducts = null)
        {
            Name = name;
            Symbol = symbol;
            Number = number;
            Mass = mass;
            MeltingPoint = meltingK.HasValue ? new Temperature(meltingK.Value) : null;
            BoilingPoint = boilingK.HasValue ? new Temperature(boilingK.Value) : null;
            FusionPoint = fusionK.HasValue ? new Temperature(fusionK.Value) : null;
            FusionProducts = fusionProducts;
        }

        public static Element Hydrogen = new Element("Hydrogen", "H", 1, 1.008, 13.99, 20.271, 13000000, new List<Element> { Helium });
        public static Element Helium = new Element("Helium", "He", 2, 4.002602, .95, 4.220, 100000000, new List<Element> { Carbon, Oxygen });
        public static Element Lithium = new Element("Lithium", "Li", 3, 6.94, 453.69, 1560);
        public static Element Beryllium = new Element("Beryllium", "Be", 4, 9.0121831, 1560, 2742);
        public static Element Boron = new Element("Boron", "B", 5, 10.81, 2349, 4200);
        public static Element Carbon = new Element("Carbon", "C", 6, 12.011, 3800, 4300, 500000000, new List<Element> { Oxygen, Neon, Magnesium, Sodium });
        public static Element Nitrogen = new Element("Nitrogen", "N", 7, 14.007, 63.15, 77.36);
        public static Element Oxygen = new Element("Oxygen", "O", 8, 15.999, 54.36, 90.20, 1500000000, new List<Element> { Magnesium, Silicon, Sulfur, Phosphorus });
        public static Element Fluorine = new Element("Fluorine", "F", 9, 18.998403163, 53.53, 85.03);
        public static Element Neon = new Element("Neon", "Ne", 10, 20.1797, 24.56, 27.07, 1200000000, new List<Element> { Oxygen, Magnesium });
        public static Element Sodium = new Element("Sodium", "Na", 11, 22.98976928, 370.87, 1156);
        public static Element Magnesium = new Element("Magnesium", "Mg", 12, 24.305, 923, 1363);
        public static Element Aluminum = new Element("Aluminum", "Al", 13, 26.9815385, 933.47, 2792);
        public static Element Silicon = new Element("Silicon", "Si", 14, 28.085, 1687, 3538, 3000000000, new List<Element> { Silicon, Sulfur, Argon, Calcium, Titanium, Chromium, Iron, Nickel });
        public static Element Phosphorus = new Element("Phosphorus", "P", 15, 30.973761998, 317.3, 550);
        public static Element Sulfur = new Element("Sulfur", "S", 16, 32.06, 388.36, 717.87);
        public static Element Chlorine = new Element("Chlorine", "Cl", 17, 35.45, 171.6, 239.11);
        public static Element Argon = new Element("Argon", "Ar", 18, 39.948, 83.8, 87.3);
        public static Element Potassium = new Element("Potassium", "K", 19, 39.0983, 336.53, 1032);
        public static Element Calcium = new Element("Calcium", "Ca", 20, 40.078, 1115, 1757);
        public static Element Scandium = new Element("Scandium", "Sc", 21, 44.955908, 1814, 3109);
        public static Element Titanium = new Element("Titanium", "Ti", 22, 47.867, 1941, 3560);
        public static Element Vanadium = new Element("Vanadium", "V", 23, 50.9415, 2183, 3680);
        public static Element Chromium = new Element("Chromium", "Cr", 24, 51.9961, 2180, 2944);
        public static Element Manganese = new Element("Manganese", "Mn", 25, 54.938044, 1519, 2334);
        public static Element Iron = new Element("Iron", "Fe", 26, 55.845, 1811, 3134);
        public static Element Cobalt = new Element("Cobalt", "Co", 27, 58.933194, 1768, 3200);
        public static Element Nickel = new Element("Nickel", "Ni", 28, 58.6934, 1728, 3186);
        public static Element Copper = new Element("Copper", "Cu", 29, 63.546, 1357.77, 2835);
        public static Element Zinc = new Element("Zinc", "Zn", 30, 65.38, 692.88, 1180);
        public static Element Gallium = new Element("Gallium", "Ga", 31, 302.9146, 2673, .371);
        public static Element Germanium = new Element("Germanium", "Ge", 32, 72.630, 1211.4, 3106);
        public static Element Arsenic = new Element("Arsenic", "As", 33, 74.921595, 1090, 887);
        public static Element Selenium = new Element("Selenium", "Se", 34, 78.971, 453, 958);
        public static Element Bromine = new Element("Bromine", "Br", 35, 79.904, 265.8, 332);
        public static Element Krypton = new Element("Krypton", "Kr", 36, 83.798, 115.79, 119.93);
        public static Element Rubidium = new Element("Rubidium", "Rb", 37, 85.4678, 312.46, 961);
        public static Element Strontium = new Element("Strontium", "Sr", 38, 87.62, 1050, 1655);
        public static Element Yttrium = new Element("Yttrium", "Y", 39, 88.90584, 1799, 3609);
        public static Element Zirconium = new Element("Zirconium", "Zr", 40, 91.224, 2128, 4682);
        public static Element Niobium = new Element("Niobium", "Nb", 41, 92.90637, 2750, 5017);
        public static Element Molybdenum = new Element("Molybdenum", "Mo", 42, 95.95, 2896, 4912);
        public static Element Technetium = new Element("Technetium", "Tc", 43, 98, 2430, 4538);
        public static Element Ruthenium = new Element("Ruthenium", "Ru", 44, 101.07, 2607, 4423);
        public static Element Rhodium = new Element("Rhodium", "Rh", 45, 102.9055, 2237, 3968);
        public static Element Palladium = new Element("Palladium", "Pd", 46, 106.42, 1828.05, 3236);
        public static Element Silver = new Element("Silver", "Au", 47, 107.8682, 1234.93, 2435);
        public static Element Cadmium = new Element("Cadmium", "Cd", 48, 112.414, 594.22, 1040);
        public static Element Indium = new Element("Indium", "In", 49, 114.818, 429.75, 2345);
        public static Element Tin = new Element("Tin", "Sn", 50, 118.710, 505.08, 2875);
        public static Element Antimony = new Element("Antimony", "Sb", 51, 121.76, 903.78, 1860);
        public static Element Tellurium = new Element("Tellurium", "Te", 52, 127.6, 722.66, 1261);
        public static Element Iodine = new Element("Iodine", "I", 53, 126.90447, 386.85, 457.4);
        public static Element Xenon = new Element("Xenon", "Xe", 54, 131.293, 161.4, 165.03);
        public static Element Caesium = new Element("Caesium", "Cs", 55, 132.90545196, 301.59, 944);
        public static Element Barium = new Element("Barium", "Ba", 56, 137.327, 1000, 2170);
        public static Element Lanthanum = new Element("Lanthanum", "La", 57, 138.90547, 1193, 3737);
        public static Element Cerium = new Element("Cerium", "Ce", 58, 140.116, 1068, 3716);
        public static Element Praseodymium = new Element("Praseodymium", "Pr", 59, 140.90766, 1208, 3793);
        public static Element Neodymium = new Element("Neodymium", "Nd", 60, 144.242, 1297, 3347);
        public static Element Promethium = new Element("Promethium", "Pm", 61, 145, 1315, 3273);
        public static Element Samarium = new Element("Samarium", "Sm", 62, 150.36, 1345, 2067);
        public static Element Europium = new Element("Europium", "Eu", 63, 151.964, 1099, 1802);
        public static Element Gadolinium = new Element("Gadolinium", "Gd", 64, 157.25, 1585, 3546);
        public static Element Terbium = new Element("Terbium", "Tb", 65, 158.92535, 1629, 3503);
        public static Element Dysprosium = new Element("Dysprosium", "Dy", 66, 162.5, 1680, 2840);
        public static Element Holmium = new Element("Holmium", "Ho", 67, 164.93033, 1734, 2993);
        public static Element Erbium = new Element("Erbium", "Er", 68, 167.259, 1802, 3141);
        public static Element Thulium = new Element("Thulium", "Tm", 69, 168.93422, 1818, 2223);
        public static Element Ytterbium = new Element("Ytterbium", "Yb", 70, 173.045, 1097, 1469);
        public static Element Lutetium = new Element("Lutetium", "Lu", 71, 174.9668, 1925, 3675);
        public static Element Hafnium = new Element("Hafnium", "Hf", 72, 178.49, 2506, 4876);
        public static Element Tantalum = new Element("Tantalum", "Ta", 73, 180.94788, 3290, 5731);
        public static Element Tungsten = new Element("Tungsten", "W", 74, 183.84, 3695, 5828);
        public static Element Rhenium = new Element("Rhenium", "Re", 75, 186.207, 3459, 5869);
        public static Element Osmium = new Element("Osmium", "Os", 76, 190.23, 3306, 5285);
        public static Element Iridium = new Element("Iridium", "Ir", 77, 192.217, 2719, 4701);
        public static Element Platinum = new Element("Platinum", "Pt", 78, 195.084, 2041.4, 4098);
        public static Element Gold = new Element("Gold", "Au", 79, 196.966569, 1337.33, 3129);
        public static Element Mercury = new Element("Mercury", "Hg", 80, 200.592, 234.43, 629.88);
        public static Element Thallium = new Element("Thallium", "Tl", 81, 204.38, 577, 1746);
        public static Element Lead = new Element("Lead", "Pb", 82, 207.2, 600.61, 2022);
        public static Element Bismuth = new Element("Bismuth", "Bi", 83, 208.98040, 544.7, 1837);
        public static Element Polonium = new Element("Polonium", "Po", 84, 209, 527, 1235);
        public static Element Astatine = new Element("Astatine", "At", 85, 210, 575, 610);
        public static Element Radon = new Element("Radon", "Rn", 86, 222, 202, 211.3);
        public static Element Francium = new Element("Francium", "Fr", 87, 223, 300, 950);
        public static Element Radium = new Element("Radium", "Ra", 88, 226, 973, 2010);
        public static Element Actinium = new Element("Actinium", "Ac", 89, 227, 1323, 3471);
        public static Element Thorium = new Element("Thorium", "Th", 90, 232.0377, 2115, 5061);
        public static Element Protactinium = new Element("Protactinium", "Pa", 91, 231.03588, 1841, 4300);
        public static Element Uranium = new Element("Uranium", "U", 92, 238.02891, 1405.3, 4404);
        public static Element Neptunium = new Element("Neptunium", "Np", 93, 237, 917, 4273);
        public static Element Plutonium = new Element("Plutonium", "Pu", 94, 244, 912.5, 3501);
        public static Element Americium = new Element("Americium", "Am", 95, 243, 1449, 2880);
        public static Element Curium = new Element("Curium", "Cm", 96, 247, 1613, 3383);
        public static Element Berkelium = new Element("Berkelium", "Bk", 97, 247, 1259, 2900);
        public static Element Californium = new Element("Californium", "Cf", 98, 252, 1173, 1743);
        public static Element Einsteinium = new Element("Einsteinium", "Es", 99, 252, 1133, 1269);
        public static Element Fermium = new Element("Fermium", "Fm", 100, 257, 1125);
        public static Element Mendelevium = new Element("Mendelevium", "Md", 101, 258, 1100);
        public static Element Nobelium = new Element("Nobelium", "No", 102, 259, 1100);
        public static Element Lawrencium = new Element("Lawrencium", "Lr", 103, 266, 1900);
        public static Element Rutherfordium = new Element("Rutherfordium", "Rf", 104, 267, 2400, 5800);
        public static Element Dubnium = new Element("Dubnium", "Db", 105, 268);
        public static Element Seaborgium = new Element("Seaborgium", "Sg", 106, 269);
        public static Element Bohrium = new Element("Bohrium", "Bh", 107, 270);
        public static Element Hassium = new Element("Hassium", "Hs", 108, 277);
        public static Element Meiterium = new Element("Meiterium", "Mt", 109, 278);
        public static Element Darmstadtium = new Element("Darmstatdtium", "Ds", 110, 281);
        public static Element Roentgenium = new Element("Roentgenium", "Rg", 111, 282);
        public static Element Copernicium = new Element("Copernicum", "Cn", 112, 285, null, 357);
        public static Element Nihonium = new Element("Nihonium", "Nh", 113, 286, 700, 1400);
        public static Element Flerovium = new Element("Flerovium", "Fl", 114, 289, null, 210);
        public static Element Moscovium = new Element("Moscovium", "Mc", 115, 290, 700, 1400);
        public static Element Livermorium = new Element("Livermorium", "Lv", 116, 293, 709, 1085);
        public static Element Tennessine = new Element("Tennessine", "Ts", 117, 294, 723, 883);
        public static Element Oganesson = new Element("Oganesson", "Og", 118, 294, null, 350);
        public enum ElementCategory : int
        {
            Metal,
            Metalloid,
            Nonmetal,
        }
    }
}
