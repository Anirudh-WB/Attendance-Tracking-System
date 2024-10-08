﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATS.Hubs
{
    public class AtsHubs : Hub
    {
        public async Task SendItemUpdate(long userId, DateTime AttendanceLogTime, string CheckType)
        {
            await Clients.All.SendAsync("ReceiveItemUpdate", userId, AttendanceLogTime, CheckType);
        }

        public async Task SendUserUpdate(string Email, string Password, string ContactNo)
        {
            await Clients.All.SendAsync("ReceiveUserUpdate", Email, Password, ContactNo);
        }

        public async Task SendEmployeeUpdate(long userId, string EmployeeCode, string FirstName, string LastName, long DesignationId, long GenderId, string ProfilePic)
        {
            await Clients.All.SendAsync("ReceiveEmployeeUpdate", userId, EmployeeCode, FirstName, LastName, DesignationId, GenderId, ProfilePic);
        }

        public async Task SendSignUpUpdate(string FirstName, string LastName, string Email, string ContactNo,string Password, string ProfilePic)
        {
            await Clients.All.SendAsync("ReceiveSignUpUpdate", FirstName, LastName, Email, ContactNo,Password, ProfilePic);
        }
        
        public async Task SendUpdateEncoding(long Id, long userId, string FirstName, string LastName, string ProfilePic, byte[] FaceEncoding)
        {
            await Clients.All.SendAsync("ReceiveUpdateEncoding", Id, userId, FirstName, LastName, ProfilePic, FaceEncoding);
        }

    }
}
