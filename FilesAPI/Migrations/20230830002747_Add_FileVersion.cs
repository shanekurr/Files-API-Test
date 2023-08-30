using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilesAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_FileVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "FileVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "FileVersions");
        }
    }
}
