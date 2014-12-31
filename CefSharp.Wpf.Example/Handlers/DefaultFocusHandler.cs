using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CefSharp.Wpf.Example.Handlers
{
    public class DefaultFocusHandler : IFocusHandler
    {
        private FrameworkElement _prevControl;
        private FrameworkElement _nextControl;

        public DefaultFocusHandler(FrameworkElement prevControl, FrameworkElement nextControl)
        {
            _prevControl = prevControl;
            _nextControl = nextControl;
        }

        public void OnGotFocus()
        {
            
        }

        public bool OnSetFocus(CefFocusSource source)
        {
            return false;
        }

        public void OnTakeFocus(bool next)
        {
            if(next)
                _nextControl.Dispatcher.Invoke(new Action(() => _nextControl.Focus()));
            else
                _prevControl.Dispatcher.Invoke(new Action(() => _prevControl.Focus()));
        }
    }
}
