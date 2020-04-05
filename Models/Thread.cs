using System;
using System.Diagnostics;

namespace TaskManager.Models
{
    internal class Thread
    {
        #region Fields

        private readonly ProcessThread _thread;

        #endregion

        #region Properties

        public int Id
        {
            get { return _thread.Id; }
        }

        public ThreadState State
        {

            get
            {
                return _thread.ThreadState;
                
            }

        }

        public DateTime StartingTime
        {
            get { return _thread.StartTime; }
        }

        #endregion

        internal Thread(ProcessThread thread)
        {
            _thread = thread;
        }
    }
}
