using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MSAppDataUwpWenView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            MyWebView.ScriptNotify += MyWebView_ScriptNotify;
            Uri url = MyWebView.BuildLocalStreamUri("MyTag", "/Test/HomePage.html");
            StreamUriWinRTResolver myResolver = new StreamUriWinRTResolver();
            MyWebView.NavigateToLocalStreamUri(url, myResolver);
        }

        private void MyWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var path = ApplicationData.Current.LocalFolder.Path;
        }
    }

    /// <summary>
    /// Sample URI resolver object for use with NavigateToLocalStreamUri
    /// This sample uses the local storage of the package as an example of how to write a resolver.
    /// </summary>
    public sealed class StreamUriWinRTResolver : IUriToStreamResolver
    {
        /// <summary>
        /// The entry point for resolving a Uri to a stream.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new Exception();
            }
            string path = uri.AbsolutePath;
            // Because of the signature of this method, it can't use await, so we
            // call into a separate helper method that can use the C# await pattern.
            return getContent(path).AsAsyncOperation();
        }

        /// <summary>
        /// Helper that maps the path to package content and resolves the Uri
        /// Uses the C# await pattern to coordinate async operations
        /// </summary>
        private async Task<IInputStream> getContent(string path)
        {
            // We use a package folder as the source, but the same principle should apply
            // when supplying content from other locations
            try
            {
                Uri localUri = new Uri("ms-appdata:///Local" + path);
                StorageFile f = await StorageFile.GetFileFromApplicationUriAsync(localUri);
                IRandomAccessStream stream = await f.OpenAsync(FileAccessMode.Read);
                return stream.GetInputStreamAt(0);
            }
            catch (Exception) { throw new Exception("Invalid path"); }
        }
    }
}