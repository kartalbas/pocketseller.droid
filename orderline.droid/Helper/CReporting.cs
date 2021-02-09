using System.IO;
using Android.Content;
using Android.OS;
using Android.Print;
using Android.Print.Pdf;
using Android.Runtime;
using Java.IO;

namespace pocketseller.droid.Helper
{
    public class GenericPrintAdapter : PrintDocumentAdapter
    {
        public string File { get; set; }
        public Context PrintContext { get; set; }
        public PrintedPdfDocument PrintedDocument { get; set; }

        public GenericPrintAdapter(Context objPrintContext, string strFile)
        {
            File = strFile;
            PrintContext = objPrintContext;
        }

        public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
        {
            PrintedDocument = new PrintedPdfDocument(PrintContext, newAttributes);

            var printInfo = new PrintDocumentInfo.Builder(File).SetContentType(PrintContentType.Document).Build();

            callback.OnLayoutFinished(printInfo, true);
        }

        public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
        {
            var javaStream = new FileOutputStream(destination.FileDescriptor);
            var outputStreamInvoker = new OutputStreamInvoker(javaStream);

            using (var memoryStream = new MemoryStream())
            {
                FileStream fileStream = new FileStream(File, FileMode.Open, FileAccess.Read);
                fileStream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                outputStreamInvoker.Write(bytes, 0, bytes.Length);
            }

            callback.OnWriteFinished(pages);
        }
    }
}
