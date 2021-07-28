using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Holism.DataAccess
{
    public partial class EnumMigration<T> : Migration where T : Enum
    {
        protected virtual string TableName { get; }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: TableName,
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Key = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{TableName}", x => x.Id);
                    table.UniqueConstraint($"IX_{TableName}_Unique_Key", x => x.Key);
                });
            var type = typeof(T);
            var names = type.GetEnumNames();
            foreach(string @name in names)
            {
                var value = (int)Enum.Parse(type, @name);
                migrationBuilder.InsertData(
                    table: TableName,
                    columns: new string[] { "Id", "Key" },
                    values: new object[] { value, @name }
                );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(TableName);
        }
    }
}
