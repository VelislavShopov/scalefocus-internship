using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenamedFieldInRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "RefreshTokens",
                newName: "RefreshTokenValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenValue",
                table: "RefreshTokens",
                newName: "Token");
        }
    }
}
