

namespace SW.VideoProcessingService.TacticalSolutionApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    public class MessageReader
    {
        public void Read(string message)
        {
            int dots =  message.Split('.').Length;
            Thread.Sleep(dots * 1000);
        }
    }
}
