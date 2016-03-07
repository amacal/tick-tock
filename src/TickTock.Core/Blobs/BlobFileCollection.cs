using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TickTock.Core.Blobs
{
    public class BlobFileCollection : IEnumerable<BlobFile>
    {
        public IEnumerable<BlobFile> Items { get; set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator<BlobFile> IEnumerable<BlobFile>.GetEnumerator()
        {
            return Items.OfType<BlobFile>().GetEnumerator();
        }
    }
}