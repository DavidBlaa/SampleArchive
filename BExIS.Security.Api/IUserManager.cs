﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using BExIS.Core.Persistence.Api;
using BExIS.Security.Entities;

namespace BExIS.Security.Api
{
    public interface IUserManager
    {
        #region Attributes



        #endregion


        #region Methods

        // C
        bool ChangePassword(string userName, string password, string newPassword, string machineKey, int minPasswordLength);
        bool ChangePasswordQuestionAndPasswordAnswer(string userName, string password, string newPasswordQuestion, string newPasswordAnswer, string machineKey);

        User Create(string userName, string email, string password, string passwordQuestion, string passwordAnswer, bool uniqueEmail, int minPasswordLength, string machineKey, out MembershipCreateStatus status, string comment = "", bool isApproved = false);

        // D
        bool Delete(string userName);
        bool Delete(User user);

        // F
        [Obsolete("Please use GetUsers() instead!")]
        IEnumerable<User> FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize);
        [Obsolete("Please use GetUsers() instead!")]
        IEnumerable<User> FindUserByName(string userNameToMatch, int pageIndex, int pageSize);

        // G
        IEnumerable<User> GetAllUsers(int pageIndex, int pageSize);

        string GetPassword(string userName, string passwordAnswer, int maxFailureAttempts, int failureAttemptsWindow, int minPasswordLength, string machineKey);

        User GetUserByEmail(string email);
        User GetUserById(Int64 id, bool isOnline = false);
        User GetUserByName(string userName, bool isOnline = false);

        string GetUserNameByEmail(string email);

        IEnumerable<User> GetUsers(int pageIndex, int pageSize, Expression<Func<User, bool>> expression);

        int GetUsersOnline();

        // R
        string ResetPassword(string userName, string passwordAnswer, int maxFailureAttempts, int failureAttemptsWindow, int minPasswordLength, string machineKey);


        // V
        bool ValidateUser(string userName, string password, int maxFailureAttempts, int failureAttemptsWindow, string machineKey);

        // U
        bool UnlockUser(string userName);

        User Update(User user);

        #endregion
    }

    public enum FailureType
    {
        Password = 1,
        PasswordAnswer = 2
    }
}