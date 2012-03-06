using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Coding4Fun.Kinect.KinectService.MetroClient
{
    public static class MicrosoftStreamExtensions
    {
        public static IRandomAccessStream AsRandomAccessStream( this Stream stream )
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            var outputStream = randomAccessStream.GetOutputStreamAt( 0 );
            RandomAccessStream.CopyAsync( stream.AsInputStream(), outputStream );
            return randomAccessStream;
        }
    }
}
