using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSphere.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class OAuthPropertiesAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOAuth",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OAuthClient",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOAuth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OAuthClient",
                table: "Users");
        }
    }
}
