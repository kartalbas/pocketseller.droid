using Android.App;
using FlexCel.Core;
using FlexCel.Pdf;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Java.Lang;
using pocketseller.core.Models;
using pocketseller.core.ModelsAPI;
using pocketseller.core.Services.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace pocketseller.droid.Services
{
    public class ReportService : IReportService
    {
        public string CreateReport(Order order, string templateFile, string targetPdfName)
        {
            try
            {
                ExcelFile excelReport = ReadExcelFileFromAsset(templateFile);

                using (var report = new FlexCelReport(true))
                {
                    report.AddTable<Order>("Order", new List<Order> { order });
                    report.AddTable<Orderdetail>("Orderdetails", order.Orderdetails);
                    report.AddTable<OpenPayment>("OpenPayments", order?.FacturaData?.OpenPayments);
                    report.AddTable<PaymentSheet>("PaymentSheets", order?.FacturaData?.PaymentSheets);
                    report.AddRelationship("Order", "Orderdetails", "Id", "OrderId");
                    report.AddRelationship("Order", "OpenPayments", "Adressnumber", "Adressnumber");
                    report.AddRelationship("Order", "PaymentSheets", "Adressnumber", "AccountNumber");
                    report.Run(excelReport);
                }

                var pdfFile = ExportReportAsPdf(excelReport, targetPdfName);
                return pdfFile;
            }
            catch (Exception exception)
            {
                throw new Exception($"Order {order?.Docnumber} could not be created with FlexCelReport! Error: {exception?.Message}");
            }
        }

        public string CreateLocalReport(Order order, string templateFile, string targetPdfName)
        {
            try
            {
                ExcelFile excelReport = ReadExcelFileFromAsset(templateFile);

                using (var report = new FlexCelReport(true))
                {
                    report.AddTable<Order>("Order", new List<Order> { order });
                    report.AddTable<Orderdetail>("Orderdetails", order.Orderdetails);
                    report.AddRelationship("Order", "Orderdetails", "Id", "OrderId");
                    report.Run(excelReport);
                }

                var pdfFile = ExportReportAsPdf(excelReport, targetPdfName);
                return pdfFile;
            }
            catch (Exception exception)
            {
                throw new Exception($"Order {order?.Docnumber} could not be created with FlexCelReport! Error: {exception?.Message}");
            }
        }

        public ExcelFile CreateExcelReport(Order order)
        {
            try
            {
                ExcelFile result = CreateExcelReport(order.Response);
                using (var report = new FlexCelReport(true))
                {
                    report.AddTable<Order>("Order", new List<Order> { order });
                    report.AddTable<Orderdetail>("Orderdetails", order.Orderdetails);
                    report.AddTable<OpenPayment>("OpenPayments", order?.FacturaData?.OpenPayments);
                    report.AddTable<PaymentSheet>("PaymentSheets", order?.FacturaData?.PaymentSheets);
                    report.AddRelationship("Order", "Orderdetails", "Id", "OrderId");
                    report.AddRelationship("Order", "OpenPayments", "Adressnumber", "Adressnumber");
                    report.AddRelationship("Order", "PaymentSheets", "Adressnumber", "AccountNumber");
                    report.Run(result);
                }

                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Order {order?.Docnumber} could not be created with FlexCelReport! Error: {exception?.Message}");
            }
        }

        public string ExportReportAsPdf(List<ExcelFile> excelReports, string destinationPdfFile)
        {
            try
            {
                var destinationPdfFileFullPath = CreateFullPath(destinationPdfFile);
                DeleteIfFileExists(destinationPdfFileFullPath);

                using (FlexCelPdfExport pdf = new FlexCelPdfExport())
                {
                    pdf.AllowOverwritingFiles = true;

                    using (FileStream pdfstream = new FileStream(destinationPdfFileFullPath, FileMode.Create))
                    {
                        pdf.BeginExport(pdfstream);
                        var counter = 0;
                        foreach (var excel in excelReports)
                        {
                            pdf.FontMapping = TFontMapping.ReplaceAllFonts;
                            pdf.PageLayout = TPageLayout.Outlines;
                            pdf.Workbook = excel;
                            pdf.ExportAllVisibleSheets(true, (counter++).ToString());
                        }
                        pdf.EndExport();
                    }
                }

                return destinationPdfFileFullPath;
            }
            catch (Exception objException)
            {
                throw new Exception($"Created Excel File coult not be exported to {destinationPdfFile}! Error: {objException.Message}");
            }
        }

        private string ExportReportAsPdf(ExcelFile excelTemplate, string destinationPdfFile)
        {
            try
            {
                var destinationPdfFileFullPath = CreateFullPath(destinationPdfFile);
                DeleteIfFileExists(destinationPdfFileFullPath);

                using (var pdf = new FlexCelPdfExport(excelTemplate, true))
                {
                    using (var fileStream = new FileStream(destinationPdfFileFullPath, FileMode.Create))
                    {
                        pdf.BeginExport(fileStream);
                        pdf.FontMapping = TFontMapping.ReplaceAllFonts;
                        pdf.PageLayout = TPageLayout.Outlines;
                        pdf.ExportAllVisibleSheets(false, "OrderSheet");
                        pdf.EndExport();
                    }
                }

                return destinationPdfFileFullPath;
            }
            catch (Exception objException)
            {
                throw new Exception($"Created Excel File coult not be exported to {destinationPdfFile}! Error: {objException.Message}");
            }
        }

        private ExcelFile CreateExcelReport(string templateFile)
        {
            try
            {
                ExcelFile excelReport = ReadExcelFileFromAsset(templateFile);
                return excelReport;
            }
            catch (Exception exception)
            {
                throw new Exception($"Orderscould not be created with FlexCelReport! Error: {exception?.Message}");
            }
        }

        private FlexCelPdfExport CreatePdfReport(ExcelFile excelTemplate)
        {
            try
            {
                var pdf = new FlexCelPdfExport(excelTemplate, true);
                return pdf;
            }
            catch (Exception objException)
            {
                throw new Exception($"Created Excel File coult not be created as PDF! Error: {objException.Message}");
            }
        }

        private ExcelFile ReadExcelFileFromAsset(string orderTemplate)
        {
            try
            {
                ExcelFile excelFile = new XlsFile(true);
                using (var assetStream = Application.Context.Assets.Open(orderTemplate))
                {
                    using (var tempMemStream = new MemoryStream())
                    {
                        assetStream.CopyTo(tempMemStream);
                        tempMemStream.Position = 0;
                        excelFile.Open(tempMemStream);
                    }
                }

                return excelFile;
            }
            catch (Exception exception)
            {
                throw new Exception($"File {orderTemplate} could not be read! Error: {exception.Message}");
            }
        }

        private string CreateFullPath(string fileName)
        {
            string personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var fullPath = Path.Combine(personalFolder, fileName);
            return fullPath;
        }

        private void DeleteIfFileExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}