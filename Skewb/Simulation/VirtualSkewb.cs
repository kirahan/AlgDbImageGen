﻿using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleImageGenerator.Shared;

namespace PuzzleImageGenerator.Skewb.Simulation
{
    class VirtualSkewb
    {

        Corner[] Corners;
        Center[] Centers;
        Dictionary<char, Move> Moves = new Dictionary<char, Move>();
        private string ColorScheme = "wrgyob";

        public VirtualSkewb(SkewbImageConfiguration configs)
        {
            CleanSkewb();
            DefineMoves();
            if (configs.Moves != null)
                preformAlg(configs.Moves);       
            else if (configs.Case != null)
                preformAlg(configs.Case, reverse: true);

            if (configs.StickerDefs == null)
                SetDefs(configs);
        }
        private void CleanSkewb()
        {
            Corners = Corner.PieceNames.Select(z => new Corner(z)).ToArray();
            Centers = Center.PieceNames.Select(z => new Center(z)).ToArray();
        }
        private void DefineMoves()
        {
            foreach (var move in Move.Set)
                Moves.Add(move, new Move(move));
            foreach (var rotation in Rotation.Set)
                Moves.Add(rotation, new Rotation(rotation));
        }
        private char GetColorFromFace(char face)
        {
            var count = 0;
            foreach (var faceVal in Center.PieceNames)
            {
                if (face == char.Parse(faceVal))
                    return (ColorScheme[count]);
                count++;
            }
            return '*';
        }
        private void PreformMove(char moveChar, bool reverse)
        {
            var move = Moves[moveChar];
            if (reverse)
            {
                Centers.ReverseCycle(move.Centers);
                Corners.ReverseCycle(move.Corners[0].Take(3).ToArray());
                foreach (var Index in move.Corners[0].Take(3))
                {
                    Corners[Index].Rotate(clockwise: true);
                }
                Corners[move.Corners[0].Last()].Rotate(clockwise: false);
            }
            else
            {
                Centers.Cycle(move.Centers);
                Corners.Cycle(move.Corners[0].Take(3).ToArray());
                foreach (var Index in move.Corners[0].Take(3))
                {
                    Corners[Index].Rotate(clockwise: false);
                }
                Corners[move.Corners[0].Last()].Rotate(clockwise: true);
            }
        }
        private void preformRotation(char rotationChar, bool reverse, bool twice)
        {
            var rotation = Moves[rotationChar];
            if (reverse)
            {
                Centers.ReverseCycle(rotation.Centers);
                foreach (var cycle in rotation.Corners)
                {
                    Corners.ReverseCycle(cycle);
                    if (rotationChar != 'y')
                        for (int index = 0; index < cycle.Length; index++)
                            Corners[cycle[index]].Rotate(index % 2 == 1);

                }
            }
            else
            {
                for (int i = 0; i < (twice ? 2 : 1); i++)
                {
                    Centers.Cycle(rotation.Centers);
                    foreach (var cycle in rotation.Corners)
                    {
                        Corners.Cycle(cycle);
                        if (rotationChar != 'y')
                            for (int index = 0; index < cycle.Length; index++)
                                Corners[cycle[index]].Rotate(index % 2 == 1);
                    }
                }
            }
        }
        private void preformAlg(string Alg, bool reverse = false)
        {
            Alg = Alg.Replace(" ", "");
            var moves = new List<string>();
            string move = "";
            foreach (var character in Alg)
            {
                if (Moves.Keys.Contains(character) && move.Length > 0)
                {
                    moves.Add(move);
                    move = "";
                }
                move += character;

            }
            moves.Add(move);
            if (reverse)
                moves.Reverse();
            foreach (var action in moves)
            {
                if (Move.Set.Contains(action[0]))
                    PreformMove(action[0], reverse ? action.Length != 2: action.Length == 2);
                else if (Rotation.Set.Contains((action[0])))
                    preformRotation(action[0], reverse ? !action.Contains('\'') : action.Contains('\''), action.Contains('2'));
            }
        }
        private void SetDefs(SkewbImageConfiguration config)
        {
            var defs = "";
            foreach (string face in Center.PieceNames)
            {
                var CornerIndices = Corner.GetStickerOrder(char.Parse(face));
                foreach (var cornerIndex in CornerIndices)
                {
                    var Faces = Corner.PieceNames[cornerIndex];
                    for (int i = 0; i < Faces.Length; i++)
                    {
                        if (char.Parse(face) == Faces[i])
                        {
                            defs += GetColorFromFace(Corners[cornerIndex].GetFace(i));
                        }
                    }
                }
                for (int i = 0; i < Center.PieceNames.Length; i++)
                {
                    if (face == Center.PieceNames[i])
                        defs += GetColorFromFace(char.Parse(Centers[i].PieceName));
                }
            }
            config.StickerDefs = defs;
        }
    }
}