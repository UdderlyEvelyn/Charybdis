using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Contains static multipurpose data and enums.
    /// </summary>
    [ExcludeFromCodeCoverage] //Don't need to test data for code coverage.
    public static class Data
    {
        public static List<string> Months = new List<string>
        {
            null, //for failure
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
        };

        public static List<char> Vowels = new List<char>
        {
            'a', 'e', 'i', 'o', 'u', 'y',
        };

        public static List<string> SameWhenPlural = new List<string>
        {
            "bison",
            "buffalo",
            "deer",
            "salmon",
            "sheep",
            "squid",
            "swine",
            "trout",
            "blues", //as in the music
            "counsel",
            "moose",
            "data",
            "information",
            "graffiti",
            "paparazzi",
            "spaghetti",
            //Japanese
            "bento",
            "otaku",
            "samurai",
        };

        public static List<string> SameWhenPluralSuffixes = new List<string>
        {
            "craft",
            "ies",
            "ese",
            "ses",
        };

        public static Dictionary<string, string> SpecialCasePlurals = new Dictionary<string, string>
        {
            //True Exceptions
            { "roof", "roofs" }, //Archaic "rooves" isn't invalid, but it's not used these days.
            { "person", "people" },
            { "die", "dice" },
            //Old English/Germanic
            { "child", "children" },
            { "ox", "oxen" },
            //Apophonic
            { "foot", "feet" },
            { "goose", "geese" },
            { "louse", "lice" },
            { "mouse", "mice" },
            { "dormouse", "dormice" },
            { "tooth", "teeth" },
            { "man", "men" }, //A bit faster than going doing an EndsWith check that would catch it intended for words ending with "man".
            //Greek
            { "automaton", "automata" },
            { "criterion", "criteria" },
            { "phenomenon", "phenomena" },
            { "polyhedron", "polyhedra" },
            //Italian
            { "cello", "celli" },
        };

        //For performance purposes it's faster to do a lookup than to do something like:
        //char c = '0'; 
        //int result = int.Parse(c.ToString());
        public static Dictionary<char, int> Digits = new Dictionary<char, int>
        {
            { '0', 0 },
            { '1', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
        };

        public static Letter GreekAlpha = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Alpha",
            NativeName = "Άλφα",
            AlternateNames = new string[] { "Alfa" },
            Glyph = 'Α',
            LowerGlyph = 'α',
            AlternateGlyphs = new char[] { 'Ᾱ', 'ᾱ', 'Ᾰ', 'ᾰ', 'ά', 'ὰ', 'ᾶ', 'ᾳ' },
            NumericalValue = 1,
        };

        public static Letter GreekBeta = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Beta",
            NativeName = "βῆτα",
            AlternateNames = new string[] { "βήτα" },
            Glyph = 'Β',
            LowerGlyph = 'β',
            EnglishEquivalent = 'B',
            NumericalValue = 2,
        };

        public static Letter GreekGamma = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Gamma",
            NativeName = "Γάμμα",
            Glyph = 'Γ',
            LowerGlyph = 'γ',
            EnglishEquivalent = 'G',
            NumericalValue = 3,
        };

        public static Letter GreekDelta = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Delta",
            NativeName = "Δέλτα",
            Glyph = 'Δ',
            LowerGlyph = 'δ',
            EnglishEquivalent = 'D',
            NumericalValue = 4,
        };

        public static Letter GreekEpsilon = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Epsilon",
            NativeName = "Έψιλον",
            Glyph = 'Ε',
            LowerGlyph = 'ε',
            AlternateGlyphs = new char[] { 'ϵ' },
            EnglishEquivalent = 'E',
            NumericalValue = 5,
        };

        public static Letter GreekDigamma = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Digamma",
            NativeName = "Διγάμμα", //Guesswork.
            AlternateNames = new string[] { "waw", "wau", "episēmon", "stigma" },
            Glyph = 'Ϝ',
            LowerGlyph = 'ϝ',
            AlternateGlyphs = new char[] { 'Ͷ', 'ͷ' }, //Pamphylian variations, unicode points shared with Arcadian Tsan.
            NumericalGlyph = 'ϛ', //Called Stigma in this context, but they're really the same letter in many ways.
            NumericalValue = 6,
        };

        public static Letter GreekStigma = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Stigma",
            NativeName = "ϛίγμα", //Guesswork using itself as symbol as is common in letter names.
            AlternateNames = new string[] { "στίγμα" }, //Guesswork without using the digraph.
            Glyph = 'Ϛ',
            LowerGlyph = 'ϛ',
            NumericalValue = 6,
        };

        public static Letter GreekZeta = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Zeta",
            NativeName = "ζήτα",
            Glyph = 'Ζ',
            LowerGlyph = 'ζ',
            EnglishEquivalent = 'Z',
            NumericalValue = 7,
        };

        public static Letter GreekEta = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Eta",
            NativeName = "ήτα",
            AlternateNames = new string[] { "ἦτα", "Heta" }, //Heta evolved into Eta but both coexisted for a bit in some areas.
            Glyph = 'Η',
            LowerGlyph = 'η',
            EnglishEquivalent = 'H',
            NumericalValue = 8,
        };

        public static Letter GreekTheta = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Theta",
            NativeName = "θήτα",
            AlternateNames = new string[] { "θῆτα," },
            Glyph = 'Θ',
            LowerGlyph = 'θ',
            AlternateGlyphs = new char[] { 'ϑ' },
            //English Equivalent "Th"
            NumericalValue = 9,
        };

        public static Letter GreekIota = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Iota",
            NativeName = "Ιώτα",
            Glyph = 'Ι',
            LowerGlyph = 'ι',
            EnglishEquivalent = 'I',
            NumericalValue = 10,
        };

        public static Letter GreekKappa = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Kappa",
            NativeName = "κάππα",
            Glyph = 'Κ',
            LowerGlyph = 'κ',
            AlternateGlyphs = new char[] { 'ϰ' },
            EnglishEquivalent = 'K',
            NumericalValue = 20,
        };

        public static Letter GreekLambda = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Lambda",
            NativeName = "Λάμδα",
            AlternateNames = new string[] { "Λάμβδα" },
            Glyph = 'Λ',
            LowerGlyph = 'λ',
            EnglishEquivalent = 'L',
            NumericalValue = 30,
        };

        public static Letter GreekMu = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Mu",
            NativeName = "μυ",
            AlternateNames = new string[] { "μι", "μῦ" },
            Glyph = 'Μ',
            LowerGlyph = 'μ',
            EnglishEquivalent = 'M',
            NumericalValue = 40,
        };

        public static Letter GreekNu = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Nu",
            NativeName = "Νι",
            AlternateNames = new string[] { "Νῦ" },
            Glyph = 'Ν',
            LowerGlyph = 'ν',
            EnglishEquivalent = 'N',
            NumericalValue = 50,
        };

        public static Letter GreekXi = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Xi",
            NativeName = "ξι",
            Glyph = 'Ξ',
            LowerGlyph = 'ξ',
            EnglishEquivalent = 'X',
            NumericalValue = 60,
        };

        public static Letter GreekOmicron = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Omicron",
            NativeName = "Όμικρον",
            AlternateNames = new string[] { "Όὖ" },
            Glyph = 'Ο',
            LowerGlyph = 'ο',
            EnglishEquivalent = 'O',
            NumericalValue = 70,
        };

        public static Letter GreekPi = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Pi",
            NativeName = "Π",
            Glyph = 'Π',
            LowerGlyph = 'π',
            EnglishEquivalent = 'P',
            NumericalValue = 80,
        };

        public static Letter GreekQoppa = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Qoppa",
            NativeName = "Ϙοππα",
            AlternateNames = new string[] { "Koppa" },
            Glyph = 'Ϙ',
            LowerGlyph = 'ϙ',
            EnglishEquivalent = 'Q',
            NumericalGlyph = 'ϟ',
            NumericalValue = 90,
        };

        public static Letter GreekRho = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Rho",
            NativeName = "ῥῶ",
            Glyph = 'Ρ',
            LowerGlyph = 'ρ',
            AlternateGlyphs = new char[] { 'ϱ' },
            EnglishEquivalent = 'R',
            NumericalValue = 100,
        };

        public static Letter GreekSigma = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Sigma",
            NativeName = "σίγμα",
            AlternateNames = new string[] { "san", "σαν", "tsan", "τσαν" }, //"σαν" & "τσαν" being an assumption.
            Glyph = 'Σ',
            LowerGlyph = 'σ',
            AlternateGlyphs = new char[] { 'ς', 'Ϲ', 'ϲ', 'Ϻ', 'ϻ', 'Ͷ', 'ͷ' }, //'ς' is used if the s is at the end of a word.
            EnglishEquivalent = 'S',
            NumericalValue = 200,
        };

        public static Letter GreekTau = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Tau",
            NativeName = "ταυ",
            Glyph = 'Τ',
            LowerGlyph = 'τ',
            EnglishEquivalent = 'T',
            NumericalValue = 300,
        };

        public static Letter GreekUpsilon = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Upsilon",
            NativeName = "ύψιλον",
            Glyph = 'Υ',
            LowerGlyph = 'υ',
            EnglishEquivalent = 'Y', //Also V, U, W.
            NumericalValue = 400,
        };

        public static Letter GreekPhi = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Phi",
            NativeName = "ϕι",
            AlternateNames = new string[] { "φεῖ" },
            Glyph = 'Φ',
            LowerGlyph = 'φ',
            //English Equivalent "Ph"
            NumericalValue = 500, //Or 500,000?
        };

        public static Letter GreekChi = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Chi",
            NativeName = "χῖ",
            Glyph = 'Χ',
            LowerGlyph = 'χ',
            //English Equivalent Officially "Ch" or "Kh", Informally 'H' or 'X'
            NumericalValue = 600,
        };

        public static Letter GreekPsi = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Psi",
            NativeName = "Ψι",
            Glyph = 'Ψ',
            LowerGlyph = 'ψ',
            //English Equivalent "Ps"
            NumericalValue = 700,
        };

        public static Letter GreekOmega = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Omega",
            NativeName = "Ωμέγα",
            AlternateNames = new string[] { "ō", "ὦ" },
            Glyph = 'Ω',
            LowerGlyph = 'ω',
            EnglishEquivalent = 'O', //More correctly "Aw"
            NumericalValue = 800,
        };

        public static Letter GreekSampi = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Sampi", //Possibly from "san pi", or "like pi" as a visual description            
            NativeName = "σαμπι", //Guesswork
            AlternateNames = new string[] { "enacosis", "sincope", "o charaktir" },
            Glyph = 'ϡ',
            //English Equivalent "Ss" or "Ts" (uncertain history)
            NumericalValue = 900,
        };

        //Added to Greek to write Bactrian, likely a reintroduction or survival of San, which may be a regional variation or competitor of Sigma.
        //Do not confuse with the Germanic letter Thorn.
        public static Letter GreekSho = new Letter
        {
            Alphabet = Alphabet.Greek,
            Name = "Sho", //This name was invented in 2002 for computer encoding.
            NativeName = "Ϸ",
            Glyph = 'Ϸ',
            LowerGlyph = 'ϸ',
            //English Equivalent "Sh"
        };

        public static List<Letter> GreekLetters = new List<Letter>
        {
            GreekAlpha,
            GreekBeta,
            GreekGamma,
            GreekDelta,
            GreekEpsilon,
            GreekDigamma,
            GreekStigma,
            GreekZeta,
            GreekEta,
            GreekTheta,
            GreekIota,
            GreekKappa,
            GreekLambda,
            GreekMu,
            GreekNu,
            GreekXi,
            GreekOmicron,
            GreekPi,
            GreekQoppa,
            GreekRho,
            GreekSigma,
            GreekTau,
            GreekUpsilon,
            GreekPhi,
            GreekChi,
            GreekPsi,
            GreekOmega,
            GreekSampi,
            GreekSho,
        };

        public static List<string> VowelSyllables = new List<string>()
        {
            "aa",
            "ae",
            "ai",
            "ao",
            "au",
            "ay",
            "ea",
            "ee",
            "ei",
            "eo",
            "eu",
            "ey",
            "ia",
            "ie",
            "ii",
            "io",
            "iu",
            "iy",
            "oa",
            "oe",
            "oi",
            "oo",
            "ou",
            "oy",
            "ua",
            "ue",
            "ui",
            "uo",
            "uu",
            "uy",
            "ya",
            "ye",
            "yi",
            "yo",
            "yu",
            "yy",
        };

        public static List<string> ConsonantSyllables = new List<string>()
        {
            #region Pronounceable Double Consonants
            "bb",
            "cc",
            "dd",
            "ff",
            "gg",
            "kk",
            "ll",
            "mm",
            "nn",
            "pp",
            "qq",
            "rr",
            "ss",
            "tt",
            "vv",
            "xx",
            "zz",
            #endregion
            "bh",
            "ch",
            "dh",
            "kh",
            "th",
            "sh",
            "bt",
            "pt",
            "st",
            "mn",
        };

        public static List<char> RomanLetters = new List<char>()
        {
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
        };
    }

    public enum LogTarget
    {
        None,
        Database,
        EventLog,
        LogFile,
    }

    public enum Severity : int
    {
        Information = 1,
        Warning = 2,
        Error = 3,
    }

    public struct Letter
    {
        public char Glyph;
        public char LowerGlyph;
        public char[] AlternateGlyphs;

        public char EnglishEquivalent;

        public string Name;
        public string NativeName;
        public string[] AlternateNames;

        public Alphabet Alphabet;

        public char NumericalGlyph;
        public int NumericalValue;
    }

    public enum Alphabet : int
    {
        Greek,
        Roman,
        Coptic,
        Cyrillic,
        Hebrew,
        Latin,
        Phoenician,
        //etc..
    }
}
