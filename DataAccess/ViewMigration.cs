using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Holism.DataAccess
{
    public partial class ViewMigration : Migration
    {
        protected virtual string ViewName 
        {
            get
            {
                return this.GetType().Name;
            }
        }
        
        protected virtual string Query { get; }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                create view {ViewName}
                as
                {Query}
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"drop view {ViewName}");
        }
    }
}
