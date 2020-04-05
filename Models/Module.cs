using System;
using System.Diagnostics;
namespace TaskManager.Models
{
    internal class Module
    {
        #region Fields

        private readonly ProcessModule _module;

        #endregion

        public string Name
        {
            get { return _module.ModuleName; }
        }

        public string Filepath
        {

            get
            {
                try
                {
                    return _module.FileName;
                }
                catch (Exception e) //because of security
                {
                    return "Access denied";
                }
            }

        }

        internal Module(ProcessModule module)
        {
            _module = module;
        }
    }
}
