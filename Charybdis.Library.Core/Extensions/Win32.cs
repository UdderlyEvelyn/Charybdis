using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Charybdis.Library.Core
{
    public static class Win32
    {
        /// <summary>
        /// Returns the path of an existing file selected by the user in a standard dialog.
        /// </summary>
        /// <param name="filter">filter string for what filetypes to display in the dialog</param>
        /// <returns>the path to the selected file, or null if no file was selected</returns>
        public static string ChooseExistingFile(string filter = null)
        {
            var ofd = new OpenFileDialog { CheckFileExists = true }; //Create dialog, only allow existing files.
            if (filter != null) //If a filter was provided..
                ofd.Filter = filter; //Use it.
            /*Per MSDN documentation at https://msdn.microsoft.com/en-us/library/ms614336(v=vs.110).aspx
                *"In the current implementation, the derived classes (OpenFileDialog and SaveFileDialog) will only return true or false."
                *This means it's safe to cast the result from bool? to bool when using OpenFileDialog and SaveFileDialog.
                */ 
            return (bool)ofd.ShowDialog() ? ofd.FileName : null; //If a file was selected, return the path, otherwise return null.
        }

        /// <summary>
        /// Returns the path of a file selected by the user in a standard dialog.
        /// </summary>
        /// <param name="filter">filter string for what filetypes to display in the dialog</param>
        /// <returns>the path to the selected file, or null if no file was selected</returns>
        public static string ChooseFile(string filter = null)
        {
            var ofd = new OpenFileDialog(); //Create dialog.
            if (filter != null) //If a filter was provided..
                ofd.Filter = filter; //Use it.
            /*Per MSDN documentation at https://msdn.microsoft.com/en-us/library/ms614336(v=vs.110).aspx
                *"In the current implementation, the derived classes (OpenFileDialog and SaveFileDialog) will only return true or false."
                *This means it's safe to cast the result from bool? to bool when using OpenFileDialog and SaveFileDialog.
                */
            return (bool)ofd.ShowDialog() ? ofd.FileName : null; //If a file was selected, return the path, otherwise return null.
        }


        /// <summary>
        /// Returns the path of a file selected by the user in a standard dialog.
        /// </summary>
        /// <param name="filter">filter string for what filetypes to display in the dialog</param>
        /// <returns>the path to the selected file, or null if no file was selected</returns>
        public static string ChooseFile(bool toSave)
        {
            if (toSave)
            {
                var ofd = new SaveFileDialog(); //Create dialog.
                /*Per MSDN documentation at https://msdn.microsoft.com/en-us/library/ms614336(v=vs.110).aspx
                    *"In the current implementation, the derived classes (OpenFileDialog and SaveFileDialog) will only return true or false."
                    *This means it's safe to cast the result from bool? to bool when using OpenFileDialog and SaveFileDialog.
                    */
                return (bool)ofd.ShowDialog() ? ofd.FileName : null; //If a file was selected, return the path, otherwise return null.
            }
            else
                return ChooseFile();
        } 
    }
}
