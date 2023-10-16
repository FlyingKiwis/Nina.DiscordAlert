using NINA.Core.Model;
using NINA.Sequencer.SequenceItem;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace NINA.DiscordAlert.DiscordAlertSequenceItems {

    /// <summary>
    /// A test method for causing an error.  To enable uncomment the below lines
    /// </summary>

    //[ExportMetadata("Name", "ERROR")]
    //[ExportMetadata("Description", "This causes an error")]
    //[ExportMetadata("Category", "Discord Alert")]
    //[Export(typeof(ISequenceItem))]
    //[JsonObject(MemberSerialization.OptIn)]
    public class Error : SequenceItem {

        [ImportingConstructor]
        public Error() { }

        public override object Clone() {
            return new Error();
        }

        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            throw new Exception("This is a test error");
        }

        public override string ToString() {
            return "Error";
        }
    }
}
