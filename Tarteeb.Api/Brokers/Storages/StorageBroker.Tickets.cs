﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
 
  
using Tarteeb.Api.Models.Tickets;
 
namespace Tarteeb.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Ticket> Tickets { get; set; }

        public async ValueTask<Ticket> InsertTicketAsync(Ticket ticket) =>
            await InsertAsync(ticket);
      

     }
}
