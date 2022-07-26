using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrewMcdermott.NINA.DiscordAlert.Util {
    public static class FailureHelper 
    {
        public static IList<string> GetReasons(ISequenceItem sequenceItem) 
        {
            if(sequenceItem is IValidatable validatable && validatable.Issues.Count > 0) 
            {
                return validatable.Issues;
            }

            return new List<string> { "An unkown error occured" };
        }
    }
}
