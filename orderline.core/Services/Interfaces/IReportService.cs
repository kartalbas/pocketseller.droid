using FlexCel.Core;
using pocketseller.core.ModelsAPI;
using System.Collections.Generic;

namespace pocketseller.core.Services.Interfaces
{
    public interface IReportService
    {
        string CreateReport(Order order, string strTemplate, string strTargetPdf);
        string CreateLocalReport(Order order, string strTemplate, string strTargetPdf);
        ExcelFile CreateExcelReport(Order order);
        string ExportReportAsPdf(List<ExcelFile> excelReports, string destinationPdfFile);
    }
}
