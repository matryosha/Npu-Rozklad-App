﻿using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Tests
{
    internal static class Helpers
    {
        internal static List<SubscribedUser> CreateUsers(
            int count,
            int groupExternalId,
            string facultyShortName) {
            List<SubscribedUser> resultList = new List<SubscribedUser>(count);

            for (int i = 1; i <= count; i++)
            {
                resultList.Add(new SubscribedUser(
                    int.Parse($"{i}{i + i}{i * i}{i}"),
                    groupExternalId,
                    int.Parse($"{i}{i + i}{i * i}{i}"),
                    facultyShortName));
            }

            return resultList;
        }
    }
}
