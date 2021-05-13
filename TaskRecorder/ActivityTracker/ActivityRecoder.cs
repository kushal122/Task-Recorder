using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskRecorder.Data;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.ActivityTracker
{
    public class ActivityRecoder : IActionFilter
    {
        private readonly ConnectionDBContext db;
        public ActivityRecoder(ConnectionDBContext db)
        {
            this.db = db;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var data = "";
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var url = $"{controllerName}/{actionName}";

            if (!string.IsNullOrWhiteSpace(context.HttpContext.Request.QueryString.Value))
            {
                data = context.HttpContext.Request.QueryString.Value;
            }
            else
            {
                var userData = context.ActionArguments.FirstOrDefault();
                var stringUserData = JsonConvert.SerializeObject(userData);

                data = stringUserData;
            }
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            string userName;
            if (context.HttpContext.User.Claims.Count() > 0)
            {
                userName = context.HttpContext.User.Identity.Name;
                StoreUserActivity(data, url, userName, ipAddress);
            }
            else
            {
                userName = "Null";
                StoreNonUserActivity(data, url, ipAddress);
            }           
        }
        public void StoreUserActivity(string data, string url, string userName, string ipAddress)
        {
            var userActivity = new UserActivity
            {
                Data = data,
                Url = url,
                UserName = userName,
                IpAddress = ipAddress
            };
            db.UserActivity.Add(userActivity);
            db.SaveChanges();
        }
        public void StoreNonUserActivity(string data, string url, string ipAddress)
        {
            var userActivity = new NonUserActivity
            {
                Data = data,
                Url = url,
                IpAddress = ipAddress
            };
            db.NonUserActivity.Add(userActivity);
            db.SaveChanges();
        }
    }
}
