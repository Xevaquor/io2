using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IO2.Services
{
    class PromptService
    {
        public bool? YesNoCancel(string message)
        {
            var result = MessageBox.Show(message, "IO2", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }

        public bool OkCancel(string msg)
        {
            return MessageBox.Show(msg, "IO2", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }
    }
}
