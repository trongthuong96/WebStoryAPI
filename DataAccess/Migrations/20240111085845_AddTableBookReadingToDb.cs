using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBookReadingToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    ChineseBookId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChapNumber = table.Column<short>(type: "smallint", nullable: false),
                    ChapTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChapterIndex = table.Column<short>(type: "smallint", nullable: false),
                    BookSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookReadings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookReadings_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookReadings_ChineseBooks_ChineseBookId",
                        column: x => x.ChineseBookId,
                        principalTable: "ChineseBooks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookReadings_BookId",
                table: "BookReadings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookReadings_ChineseBookId",
                table: "BookReadings",
                column: "ChineseBookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookReadings_UserId",
                table: "BookReadings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookReadings");

           }
    }
}
