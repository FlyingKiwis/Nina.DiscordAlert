using NINA.Core.Model;
using NINA.Sequencer.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {
    public class ExecuteDetails 
    {
        public ISequenceContainer Context { get; }
        public IProgress<ApplicationStatus> Progress { get; }
        public CancellationToken Token { get; }

        public ExecuteDetails(ISequenceContainer context, IProgress<ApplicationStatus> progress, CancellationToken token) {
            Context = context;
            Progress = progress;
            Token = token;
        }
    }
}
