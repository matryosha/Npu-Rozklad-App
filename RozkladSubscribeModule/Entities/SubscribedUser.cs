using System;
using System.Collections.Generic;
using System.Text;

namespace RozkladSubscribeModuleClient.Entities
{
    internal class SubscribedUser
    {
        public int TelegramId { get; set; }
        public int GroupExternalId { get; set; }
        public string FacultyShortName { get; set; }
    }
}
