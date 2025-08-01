﻿using pocketseller.core.ModelsAPI;
using pocketseller.core.ModelsPS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pocketseller.core.Services.Interfaces
{
    public interface IRestService
    {
        Task<IList<MailResponse>> GetMails();
        Task<bool> DeleteMail(string messageId);
        Task<bool> InsertDeletedOpenPayment(string accountNumber, string documentNumber);

        Task<bool> SendMail(string from, string to, string subject, string body);

        Task<IList<EMails>> GetOpMails();
        Task<FacturaData> GetFacturaData(int orderNumber);
        Task<IList<Stock>> GetAllStocks();

        Task<bool> Test();
        Task<string> GetMobileNumber(string username, string password, string sourcename);
        Task<bool> ChangePassword(string username, string oldPassword, string newPassword, string newPasswordConfirm);
        Task<Tuple<string, string, bool>> GetMobileToken(string username, string mobile, string token, string sourcename);
    }
}