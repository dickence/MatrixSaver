﻿using System;
using FerretLib.SFML;
using SFML.Graphics;
using SFML.Window;

namespace MatrixScreen
{
    internal class ChronoDisplay : IEntity
    {
        private Text _text;
        private Color _color;
        private ChronoEventArgs _chrono;

        public ChronoDisplay()
        {
            _text = new Text {
                Font = new Font(@"data\lekton.ttf"),
                CharacterSize = 14,
                Position = new Vector2f(30, 30),
            };            
        }

        public void Render(RenderTarget target)
        {
            _text.DisplayedString += "\nCanvas: " + target.Size.X + "x" + target.Size.Y;
            target.Draw(_text);
        }

        public void Update(ChronoEventArgs chronoArgs)
        {
            _chrono = chronoArgs;

            _text.DisplayedString = string.Format("Dδ{0:0.00000}\n{1:#####0.00}FPS",
                _chrono.Delta, _chrono.Fps);
            var x = (byte)(DateTime.Now.Millisecond % 255);
            _color = new Color(x,255,x);
        }        
    }
}