using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTranslator.Command
{
    public class CommandArgs
    {
        public object CommandParameter { get; set; } = new object();
        public EventArgs EventArgs { get; set; } = new EventArgs();
    }
}
