﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using Xeptions;


namespace Tarteeb.Api.Models.Users.Exceptions
{
    public partial class FailedUserDependencyValidationException : Xeption
    {
        public FailedUserDependencyValidationException(Exception innerException)
            : base(message: "Failed user dependency validation error occurred,fix errors and try again.",
                  innerException) 
        { }
    }
}
