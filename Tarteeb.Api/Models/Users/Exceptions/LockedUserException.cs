﻿using System;
using Xeptions;

namespace Tarteeb.Api.Models.Users.Exceptions
{
    public class LockedUserException :Xeption
    {
        public LockedUserException(Exception innerException)
            :base(message:"User is locked ,please try again.",innerException)
        {}
    }
}
