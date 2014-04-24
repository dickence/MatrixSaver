using System.Collections;
using System.Collections.Generic;
using SFML.Window;

namespace FerretLib.SFML
{
    public class ViewPortCollection : IEnumerable<ViewPort>
    {

        public List<ViewPort> ViewPorts { get; protected set; }
        public System.Drawing.Rectangle WorkingArea { get; protected set; }

        public ViewPortCollection(bool isFullScreen, bool isMultiMonitor)
        {
            ViewPorts = new List<ViewPort>();

            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                ViewPorts.Add(new ViewPort(screen, isFullScreen));                
                if (!isMultiMonitor) break;
            }

            foreach (var viewPort in ViewPorts)
            {
                viewPort.Window.KeyPressed += (o, e) => { if(KeyPressed!=null)KeyPressed(viewPort, e); };
                viewPort.Window.KeyReleased += (o, e) => { if (KeyReleased != null)KeyReleased(viewPort, e); };
                viewPort.Window.MouseMoved += (o, e) => { if (MouseMoved != null)MouseMoved(viewPort, e); };
                viewPort.Window.MouseButtonPressed += (o, e) => { if (MouseButtonPressed != null)MouseButtonPressed(viewPort, e); };
                viewPort.Window.MouseButtonReleased += (o, e) => { if (MouseButtonReleased != null)MouseButtonReleased(viewPort, e); };
                viewPort.Window.MouseWheelMoved += (o, e) => { if (MouseWheelMoved != null)MouseWheelMoved(viewPort, e); };
            }            
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
            ViewPorts.ForEach(x=>x.Window.DispatchEvents());  
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
        
    }
}