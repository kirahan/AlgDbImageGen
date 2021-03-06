﻿using System.Collections.Generic;

namespace PuzzleImageGenerator.Pyra
{
    public class PyraImageGenerator
    {
        public static string Generate(IDictionary<string, string> input)
        {
            return new Painter.SvgGenerator(new PyraImageConfiguration(input))
                .GetSvgText();
        }
    }
}

