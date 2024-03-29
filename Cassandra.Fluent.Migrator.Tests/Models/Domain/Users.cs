﻿namespace Cassandra.Fluent.Migrator.Tests.Models.Domain;

using System;
using System.Collections.Generic;
using Mapping.Attributes;

public class Users
{
    [PartitionKey]
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Language { get; set; }

    [Frozen]
    public IEnumerable<Address> Address { get; set; }
}