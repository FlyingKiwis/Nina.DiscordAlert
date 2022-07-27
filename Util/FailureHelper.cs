using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Validations;
using System.Collections.Generic;

namespace NINA.DiscordAlert.Util {
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
