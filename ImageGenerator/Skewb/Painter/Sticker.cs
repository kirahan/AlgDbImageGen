﻿using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleImageGenerator.Shared;

namespace PuzzleImageGenerator.Skewb.Painter
{
    class Sticker
    {
        public CoordPair[] Coordinates { get; private set; }
        public string Color { get; private set; }

        public Sticker(string color, CoordPair Start, double angle, bool cornerSize, SkewbImageProp properties)
        {
            /// <summary>
            /// Constructs corner.
            /// </summary>
            
            Color = color;
            SetCoord(Start, angle, cornerSize, properties);
        }

        public Sticker(string color, Sticker[] corners)
        {
            /// <summary>
            /// Constructs center from array of surrounding corners.
            /// </summary>

            Color = color;
            var tempCoordinates = new List<CoordPair>();

            foreach (var corner in corners)
            {
                tempCoordinates.Add(corner.Coordinates.Last());
            }

            Coordinates = tempCoordinates.ToArray();
        }
        private void SetCoord(CoordPair Start, double angle, bool cornerSize, SkewbImageProp properties)
        {
            var tempCoordinates = new List<CoordPair>();

            var cornerDist = properties.ShortFaceDist;
            tempCoordinates.Add(Start);

            tempCoordinates.Add(CoordPair.CartesianFromPolar(cornerDist, angle, Start));

            tempCoordinates.Add(CoordPair.CartesianFromPolar(cornerDist, angle - (cornerSize ? Math.PI * 120 / 180 : Math.PI * 60 / 180), Start));

            Coordinates = tempCoordinates.ToArray();
        }
    }
}
