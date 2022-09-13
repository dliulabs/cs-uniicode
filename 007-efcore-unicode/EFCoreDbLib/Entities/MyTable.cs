using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDbLib.Entities;

public class MyTable {
    public long Id { get; set; }

    [Unicode(true)]
    public string Param { get; set; }
}