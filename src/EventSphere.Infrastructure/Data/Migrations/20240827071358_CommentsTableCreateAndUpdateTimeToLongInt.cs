using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSphere.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommentsTableCreateAndUpdateTimeToLongInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the default values for UpdatedAt and CreatedAt columns
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "now()");

            // Perform the conversion using raw SQL as this part is not directly supported by EF Core
            migrationBuilder.Sql(
                @"ALTER TABLE ""Comments"" ALTER COLUMN ""UpdatedAt"" TYPE bigint 
                USING EXTRACT(EPOCH FROM ""UpdatedAt"")::bigint");

            migrationBuilder.Sql(
                @"ALTER TABLE ""Comments"" ALTER COLUMN ""CreatedAt"" TYPE bigint 
                USING EXTRACT(EPOCH FROM ""CreatedAt"")::bigint");

            // Set new default values if needed (optional)
            migrationBuilder.AlterColumn<long>(
                name: "UpdatedAt",
                table: "Comments",
                type: "bigint",
                nullable: false,
                defaultValueSql: "extract(epoch from now())::bigint");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedAt",
                table: "Comments",
                type: "bigint",
                nullable: false,
                defaultValueSql: "extract(epoch from now())::bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert columns back to DateTime (timestamp with time zone)
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
