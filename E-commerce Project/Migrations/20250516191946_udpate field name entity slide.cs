using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_commerce_Project.Migrations
{
    /// <inheritdoc />
    public partial class udpatefieldnameentityslide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Slides",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Slides",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Slides",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Slides",
                newName: "FilePath");
        }
    }
}
