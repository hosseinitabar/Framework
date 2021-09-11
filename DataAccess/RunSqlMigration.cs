using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Holism.DataAccess
{
    public partial class RunSqlMigration : Migration
    {
        
        protected virtual string Query { get; }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"{Query}");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql(@$"drop view {ViewName}");
        }
    }
}
