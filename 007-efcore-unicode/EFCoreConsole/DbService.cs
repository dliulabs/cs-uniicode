using System;
using System.Collections.Generic;
using System.Linq;
using EFCoreDbLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCoreConsole;

public class DbService {
    // IConfiguration _configuration;
    ApplicationDbContext _dbContext;

    // public DbService(IConfiguration configuration, ApplicationDbContext dbContext) {
    // _configuration = configuration;
    public DbService(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }

    public void SeedData() {
        var data = new List<EFCoreDbLib.Entities.MyTable> {
            new() { Param = "這是一些非英文的字串" },
            new() { Param = "أرصدة حسابات الأستاذ العام وقت  ١0:٥7:٢٤تاريخ ٢١.0٤.٢0٢١" },
            new() { Param = "DL-α-Tocopherol Acetate" }
        };
        _dbContext.AddRange(data);
        _dbContext.SaveChanges();
    }
    public void ListMyTable() {
        var data = _dbContext
            .MyTable
            .OrderBy(t => t.Param);

        foreach (var rec in data) {
            Console.WriteLine($"MyTable {rec.Param} with ID {rec.Id}");
        }
    }
}