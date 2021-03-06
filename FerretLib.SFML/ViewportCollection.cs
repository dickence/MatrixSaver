using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using KeyEventArgs = SFML.Window.KeyEventArgs;

namespace FerretLib.SFML
{
    public class ViewPortCollection : IEnumerable<ViewPort>
    {
        public List<ViewPort> ViewPorts
        {
            get;
            protected set;
        }
        public Rectangle WorkingArea
        {
            get;
            protected set;
        }

        public ViewPortCollection(bool isFullScreen, bool isMultiMonitor)
        {
            ViewPorts = new List<ViewPort>();

            if (isMultiMonitor)
            {
                int index = 0;
                foreach (var screen in Screen.AllScreens)
                {
                    ViewPorts.Add(new ViewPort(this, screen, index++, isFullScreen));
                }
            }
            else
            {
                ViewPorts.Add(new ViewPort(this, Screen.PrimaryScreen, 0, isFullScreen));
            }

            foreach (var viewPort in ViewPorts) {
                viewPort.Window.KeyPressed += (o, e) => {
                    if (KeyPressed != null)
                        KeyPressed(viewPort, e);
                };
                viewPort.Window.KeyReleased += (o, e) => {
                    if (KeyReleased != null)
                        KeyReleased(viewPort, e);
                };
                viewPort.Window.MouseMoved += (o, e) => {
                    if (MouseMoved != null)
                        MouseMoved(viewPort, e);
                };
                viewPort.Window.MouseButtonPressed += (o, e) => {
                    if (MouseButtonPressed != null)
                        MouseButtonPressed(viewPort, e);
                };
                viewPort.Window.MouseButtonReleased += (o, e) => {
                    if (MouseButtonReleased != null)
                        MouseButtonReleased(viewPort, e);
                };
                viewPort.Window.MouseWheelMoved += (o, e) => {
                    if (MouseWheelMoved != null)
                        MouseWheelMoved(viewPort, e);
                };
            }

            WorkingArea = GetWorkingArea(ViewPorts);
        }

        public Vector2i CursorPosition()
        {
            return new Vector2i(
                Mouse.GetPosition().X - WorkingArea.Left,
                Mouse.GetPosition().Y - WorkingArea.Top
            );
        }

        private Rectangle GetWorkingArea(List<ViewPort> viewPorts)
        {
            var result = new Rectangle {
                X = viewPorts.Select(x => x.WorkingArea.X).Min(),
                Y = viewPorts.Select(x => x.WorkingArea.Y).Min()
            };
            result.Height = viewPorts.Select(x => x.WorkingArea.Bottom).Max() - result.Y;
            result.Width = viewPorts.Select(x => x.WorkingArea.Right).Max() - result.X;
            return result;
        }

        public Vector2f GetLocalCoordinates(Vector2i globalCoordinates, ViewPort viewport)
        {
            if (viewport.ID == 0)
                return new Vector2f(
                    globalCoordinates.X - -(viewport.WorkingArea.Left),
                    globalCoordinates.Y - -(viewport.WorkingArea.Top)
                    );
            int x = globalCoordinates.X - viewport.WorkingArea.Left;
            int y = globalCoordinates.Y - viewport.WorkingArea.Top;

            return new Vector2f(x, y);
        }

        #region IEnumerable support
        public IEnumerator<ViewPort> GetEnumerator()
        {
            return ViewPorts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public void HandleEvents()
        {
            ViewPorts.ForEach(x => x.Window.DispatchEvents());
        }

        #region Event goodness
        public delegate void KeyPressedHandler(ViewPort source, KeyEventArgs args);
        public delegate void KeyReleasedHandler(ViewPort source, KeyEventArgs args);
        public delegate void MouseMovedHandler(ViewPort source, MouseMoveEventArgs args);
        public delegate void MouseButtonPressedHandler(ViewPort source, MouseButtonEventArgs args);
        public delegate void MouseButtonReleasedHandler(ViewPort source, MouseButtonEventArgs args);
        public delegate void MouseWheelMovedHandler(ViewPort source, MouseWheelEventArgs args);

        public event KeyPressedHandler KeyPressed;
        public event KeyReleasedHandler KeyReleased;
        public event MouseMovedHandler MouseMoved;
        public event MouseButtonPressedHandler MouseButtonPressed;
        public event MouseButtonReleasedHandler MouseButtonReleased;
        public event MouseWheelMovedHandler MouseWheelMoved;
        #endregion

        // shitty hack
        Shape hack = new RectangleShape(new Vector2f(0,0)){FillColor = new Color(0,0,0,0)};        

        public void Draw(RenderTexture canvas)
        {                      
            var sprite = new Sprite(canvas.Texture)
            {
                Position = new Vector2f()
            };

            foreach (var viewport in ViewPorts)
            {
                viewport.Window.Clear(Color.Black);
                var rect = new IntRect(
                    viewport.Window.Position.X - WorkingArea.Left,
                    viewport.Window.Position.Y - WorkingArea.Top,
                    viewport.WorkingArea.Width,
                    viewport.WorkingArea.Height);
                sprite.TextureRect = rect;
                viewport.Window.Draw(sprite, RenderStates.Default);                
            }

            ViewPorts.ForEach(x => x.Window.Draw(hack));
            ViewPorts.ForEach(x => x.Window.Display());
        }
    }
}