using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bloggi.Migrations
{
    /// <inheritdoc />
    public partial class addUploadedImageURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrlOutServer",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrlOutServer",
                table: "Posts");
        }
    }
}
