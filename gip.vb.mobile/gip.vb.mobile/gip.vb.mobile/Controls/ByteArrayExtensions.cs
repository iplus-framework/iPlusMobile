using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gip.vb.mobile.Controls
{
    public static class ByteArrayExtensions
    {
        public static async Task<ImageSource> AsBitmapImageAsync(this byte[] byteArray)
        {
            ImageSource imageSource = null;
            await Task.Run(() => {
                imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
            }
            );
            return imageSource;
        }

        public static ImageSource AsBitmapImage(this byte[] byteArray)
        {
            var imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
            return imageSource;
        }

        //public static async Task<Uri> WriteTempSignature(this byte[] byteArray, string fileName = null)
        //{
        //    string filePath = "";
        //    await Task.Run(() => {
        //            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //            var signaturePath = Path.Combine(documentsPath, "TempSignatures");
        //            if (!System.IO.Directory.Exists(signaturePath))
        //            {
        //                System.IO.Directory.CreateDirectory(signaturePath);
        //            }
        //            filePath = Path.Combine(signaturePath, fileName);
        //            System.IO.File.WriteAllBytes(filePath, byteArray);
        //       }
        //    );
        //    return new System.Uri(String.Format("file://{0}", filePath));
        //}

        public static Uri WriteTempSignature(this byte[] byteArray, string fileName = null)
        {
            string filePath = "";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var signaturePath = Path.Combine(documentsPath, "TempSignatures");
            if (!System.IO.Directory.Exists(signaturePath))
            {
                System.IO.Directory.CreateDirectory(signaturePath);
            }
            filePath = Path.Combine(signaturePath, fileName);
            System.IO.File.WriteAllBytes(filePath, byteArray);
            return new System.Uri(String.Format("file://{0}", filePath));
        }


        //public static async Task EmptyTempSignaturesFolder()
        //{
        //    await Task.Run(() => {
        //            try
        //            {
        //                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //                var signaturePath = Path.Combine(documentsPath, "TempSignatures");
        //                if (System.IO.Directory.Exists(signaturePath))
        //                {
        //                    System.IO.Directory.Delete(signaturePath,true);
        //                }
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }
        //    );
        //}

        public static void EmptyTempSignaturesFolder()
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var signaturePath = Path.Combine(documentsPath, "TempSignatures");
                if (System.IO.Directory.Exists(signaturePath))
                {
                    System.IO.Directory.Delete(signaturePath, true);
                }
            }
            catch (Exception)
            {
            }
        }

        //public static async Task<byte[]> AsByteArray(string filename, HandballReporter.shared.Model.ImageRotation rotation)
        //{
        //    byte[] byteArr = null;
        //    IXBitmapHelper iBitmapHelper = DependencyService.Get<IXBitmapHelper>();
        //    byteArr = await iBitmapHelper.AsByteArray(filename, rotation);
        //    //await Task.Run(async() => {
        //    //        IBitmapHelper iBitmapHelper = DependencyService.Get<IBitmapHelper>();
        //    //        byteArr = iBitmapHelper.AsByteArray(filename, rotation);
        //    //    }
        //    //);
        //    return byteArr;
        //}
    }
}
