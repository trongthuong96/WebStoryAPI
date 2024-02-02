using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changeCommentOnDeleteCasecade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                      name: "FK_Comments_Comments_ParentId",
                      table: "Comments");

            migrationBuilder.AddForeignKey(
             name: "FK_Comments_Comments_ParentId",
             table: "Comments",
             column: "ParentId",
             principalTable: "Comments",
             principalColumn: "Id",
             onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                         name: "FK_Comments_Comments_ParentId",
                         table: "Comments");


            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments",
                column: "ParentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
